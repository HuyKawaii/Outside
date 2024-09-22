using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    #region || ----- Singleton ----- ||
    public static LobbyManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(this);
    }
    #endregion

    public NetworkVariable<int> numberOfPlayer;
    public NetworkVariable<int> worldSeed;

    private const int maxPlayer = 7;
    [SerializeField] private GameObject playerPrefab;

    public delegate void ConnectionCompleteCallback();
    public ConnectionCompleteCallback onInitialConnectionComplete;
    public ConnectionCompleteCallback onClientCharacterCreationComplete;

    public delegate void NetworkDespawnCallback();
    public NetworkDespawnCallback onNetworkDespawnCallback;

    public delegate void LobbyCallback(string message);
    public LobbyCallback onLobbyCreatedCallback;
    public LobbyCallback onLobbyCreatedFailCallback;
    public LobbyCallback onLobbyJoinCallback;

    public delegate void JoinRoomFailCallback(string errorMessage);
    public NetworkDespawnCallback onJoinRoomFailCallback;


    public List<GameObject> playerList;

    private Vector3 lobbyOffset = new Vector3(-148f, 22.4f, 150f);
    private async void Start()
    {
        int now = DateTime.Now.Millisecond;
        InitializationOptions options = new InitializationOptions();
        options.SetProfile(now.ToString());
        await UnityServices.InitializeAsync(options);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Player logged in with id: " + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        NetworkManager.NetworkConfig.ConnectionApproval = true;
        NetworkManager.Singleton.OnConnectionEvent += HandleConnectionEvent;
        NetworkManager.Singleton.OnConnectionEvent += HandleDisconnectionEvent;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
        NetworkManager.ConnectionApprovalCallback = ApproveConnection;
        //NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneChange;
    }

    //private void HandleSceneChange(SceneEvent sceneEvent)
    //{
    //    Debug.Log("Scene event");
    //    if (sceneEvent.SceneEventType == SceneEventType.LoadComplete)
    //    {
    //        if (!IsServer)
    //        {
    //            GameManager.Instance.StartGameAsClient();
    //        }
    //    }
    //}

    private void ApproveConnection(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (GameManager.Instance.isGameStarted)
        {
            response.Approved = false;
            response.Reason = ConstContainer.GameStartedMessage;
        }
        else
        {
            response.Approved = true;
        }
    }

    private void HandleConnectionEvent(NetworkManager networkManager, ConnectionEventData eventData)
    {
        if (IsServer && eventData.EventType == ConnectionEvent.ClientConnected)
        {
            if (eventData.ClientId == NetworkManager.Singleton.LocalClientId)
                playerList.Add(SpawnPlayer(eventData.ClientId));
            else
            {
                Debug.Log("Handling connection event");
                List<PlayerData> availableCharacterData = new List<PlayerData>();
                for (int i = 0; i < GameManager.Instance.playerDataArray?.Length; i++)
                {
                    if (!GameManager.Instance.IsCharacterDataTaken(i))
                    {
                        availableCharacterData.Add(GameManager.Instance.playerDataArray[i]);
                    }
                }
                
                SendGameDataToClientRpc(availableCharacterData.ToArray(), RpcTarget.Single(eventData.ClientId, RpcTargetUse.Temp));

            }
        }
    }

    private void HandleDisconnectionEvent(NetworkManager networkManager, ConnectionEventData eventData)
    {
        if (eventData.EventType == ConnectionEvent.ClientDisconnected)
        {
            if (IsServer)
            {
                if (GameManager.Instance.isGameStarted)
                    GameSavingManager.Instance.SaveGameData();
                if (NetworkManager.Singleton.ConnectedClients[eventData.ClientId].PlayerObject != null)
                playerList.Remove(NetworkManager.Singleton.ConnectedClients[eventData.ClientId].PlayerObject.gameObject);
            }
            else
            {
                if (NetworkManager.Singleton.DisconnectReason == ConstContainer.GameStartedMessage)
                {
                    GameMenuUIManager.Instance.CloseLoadingScreen();
                    GameMenuUIManager.Instance.DisplayErrorMessage(ConstContainer.GameStartedErrorMessage);
                }
            }
        }
    }

    private void OnClientDisconnectCallback(ulong id)
    {
        Debug.Log("Access denied: " + NetworkManager.Singleton.DisconnectReason);
    }

    private void HandleSceneEvent(SceneEvent sceneEvent)
    {
        //if (!IsServer) return;
        //Debug.Log("Scene event activated");
        //if (sceneEvent.SceneEventType == SceneEventType.LoadComplete && sceneEvent.SceneName == ConstContainer.MenuScene && GameManager.Instance.isGameStarted)
        //{
        //    Debug.Log("Scene event load 1");
        //    DisconnectClientRpc(RpcTarget.Single(sceneEvent.ClientId, RpcTargetUse.Temp));
        //}
        //if (sceneEvent.SceneEventType == SceneEventType.LoadEventCompleted && sceneEvent.SceneName == ConstContainer.MenuScene && GameManager.Instance.isGameStarted)
        //{
        //    Debug.Log("Scene event load all");
        //    NetworkManager.Singleton.Shutdown();
        //    GameManager.Instance.isGameStarted = false;
        //}
    }

    private GameObject SpawnPlayer(ulong clientId)
    {
        UpdatePlayerCount();

        int temp = numberOfPlayer.Value - 1;
        Vector3 position = new Vector3(0f, 0f, (int)(temp / 2) * (-1) ^ temp) + lobbyOffset;
        Vector3 rotation = new Vector3(0, -90, 0);

        GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.Euler(rotation));
        playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);

        ClientCharacterSpawnedRpc(RpcTarget.Single(clientId, RpcTargetUse.Temp));

        base.OnNetworkSpawn();

        return playerObject;
    }

    public override void OnNetworkDespawn()
    {
        if (onNetworkDespawnCallback != null)
            onNetworkDespawnCallback();

        GameManager.Instance.ExitGame();

        if (GameMenuUIManager.Instance != null)
        {
            GameMenuUIManager.Instance.OpenMainMenu();
            GameMenuUIManager.Instance.CloseLoadingScreen();
        }

        base.OnNetworkDespawn();
    }

    public async void CreateLobby()
    {
        playerList = new List<GameObject>();

        if (GameManager.Instance.isPlayingOnline)
        {
            string roomCode = await CreateRelay();
            if (roomCode == null)
            {
                if (onLobbyCreatedFailCallback != null)
                    onLobbyCreatedFailCallback(ConstContainer.LobbyCreatedFailMessage);
                return;
            }

            if (onLobbyCreatedCallback != null)
                onLobbyCreatedCallback(roomCode);
        }
        else
        {
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;

            if (onLobbyCreatedCallback != null)
                onLobbyCreatedCallback("");
        }
    }

    public async Task<bool> JoinGameAsClient(string gameCode)
    {
        if (!await JoinRelay(gameCode))
            return false;
        
        if (onLobbyJoinCallback != null)
            onLobbyJoinCallback(gameCode);
        return true;
    }

    private async Task<string> CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayer);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.OnSceneEvent += HandleSceneEvent;

            return joinCode;

        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return null;
        }
    }

    private async Task<bool> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            return true;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            Debug.Log("WrongCode");
            return false;
        }
        catch (ArgumentNullException e)
        {
            Debug.Log(e);
            Debug.Log("Empty field");
            return false;
        }
    }

    //[Rpc(SendTo.Server)]
    //private void SpawnPlayerObjectServerRpc(ulong playerId, Vector3 position)
    //{
    //    Debug.Log("Try spawning");
    //    GameObject playerObject = Instantiate(playerPrefab, position, Quaternion.identity);
    //    playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId);
    //}

    [Rpc(SendTo.SpecifiedInParams)]
    private void ClientCharacterSpawnedRpc(RpcParams rpcParams)
    {
        if (onClientCharacterCreationComplete != null)
        {
            onClientCharacterCreationComplete();
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void SendGameDataToClientRpc(PlayerData[] playerDataArray, RpcParams rpcParams)
    {
        GameManager.Instance.SetPlayerDataForClient(playerDataArray);
        if (onInitialConnectionComplete != null)
        {
            Debug.Log("Running initial connection callback");
            onInitialConnectionComplete();
        }
    }

    public void ClientSelectCharacterData(int characterDataIndex)
    {
        ClientSelectCharacterDataRpc(NetworkManager.Singleton.LocalClientId, characterDataIndex);
    }

    [Rpc(SendTo.Server)]
    public void ClientSelectCharacterDataRpc(ulong clientId, int characterDataIndex)
    {
        playerList.Add(SpawnPlayer(clientId));
        GameManager.Instance.ExistingCharacterDataTaken(characterDataIndex);
    }

    public void ExitLobby()
    {
        NetworkManager.Singleton.Shutdown();
    }

    private void UpdatePlayerCount(ulong id = 0)
    {
        numberOfPlayer.Value = NetworkManager.Singleton.ConnectedClients.Count;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void DisconnectClientRpc(RpcParams rpcParams)
    {
        NetworkManager.Singleton.Shutdown();
    }

}
