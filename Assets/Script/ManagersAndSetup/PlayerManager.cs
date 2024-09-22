using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager Instance;
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
    }
    #endregion

    [SerializeField] private GameObject playerPrefab;

    public Vector3 respawnPoint
    { get; private set; }
    public GameObject player;

    private float spawnCheckRayDistance = 55.0f;
    private Vector3 spawnCheckRayPoint = new Vector3(0, 50.0f, 0);
   

    private void Start()
    {
        player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;

        player.GetComponent<PlayerController>().GameStartInitialization();

        GameManager.Instance.gameLoadComplete += GetRespawnPoint;
        GameManager.Instance.gameLoadComplete += OnGameReadyPlayerInitialization;

    }

    private void OnDestroy()
    {
        GameManager.Instance.gameLoadComplete -= GetRespawnPoint;
        GameManager.Instance.gameLoadComplete -= OnGameReadyPlayerInitialization;
    }

    public void RespawnPlayer()
    {
        player.GetComponent<CharacterController>().enabled = false;
        player.transform.position = respawnPoint;
        player.GetComponent<CharacterController>().enabled = true;
        player.GetComponent<PlayerCombat>().RevivePlayer();
        Debug.Log("Respawn player");
    }

    private void GetRespawnPoint()
    {
        RaycastHit hit;
        Ray ray = new Ray(spawnCheckRayPoint, Vector3.down);
        Debug.DrawRay(transform.position, Vector3.down * spawnCheckRayDistance);
        while (!Physics.Raycast(ray, out hit, spawnCheckRayDistance, ~SetupSingleton.Instance.groundLayer))
        {
            spawnCheckRayPoint = spawnCheckRayPoint * 2;
            spawnCheckRayDistance = spawnCheckRayDistance * 2;
            Debug.Log("Cant get spawn point. Trying again");
            ray = new Ray(spawnCheckRayPoint, Vector3.down);
        }

        respawnPoint = hit.point + Vector3.up * 2;
        Debug.Log("Setting player spawn point: " + respawnPoint + "on object: " + hit.collider.name);
    }

    private void OnGameReadyPlayerInitialization()
    {
        if (!LobbyManager.Instance.IsServer || SaveSelectionManager.Instance.selectedWorldIndex >= 0)
        {
            Debug.Log("Got here");
            PlayerData playerData = GameManager.Instance.GetLocalPlayerData();
            if (!playerData.IsEmpty())
            {
                Debug.Log("Setting player to saved point");
                player.transform.position = UtilFunc.ConvertFloatToVector3(playerData.playerPosition);
                player.transform.rotation = Quaternion.Euler(UtilFunc.ConvertFloatToVector3(playerData.playerRotation));
                player.GetComponent<PlayerStats>().SetHealth(playerData.playerHealth);
                player.GetComponent<PlayerController>().isGameStarted = true;
                if (player.GetComponent<PlayerStats>().health <= 0)
                    RespawnPlayer();
                return;
            }
        }

        player.transform.position = respawnPoint;
        player.GetComponent<PlayerController>().isGameStarted = true;
        Debug.Log("Setting player to spawn point: " + respawnPoint);
        //Debug.Log("Player: " + player.transform.position);
        //Debug.Log("Game ready");
    }
}
