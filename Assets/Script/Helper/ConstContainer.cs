using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ConstContainer
{
    public const string GameScene = "GameScene";
    public const string SampleScene = "SampleScene";
    public const string MenuScene = "MenuScene";
    public const string LobbyCreatedFailMessage = "Cant create lobby. Check connection and try again later";
    public const string DeleteWorldConfirmationMessage = "Are you sure, you want to delete this world";
    public const string GameStartedMessage = "Game started";
    public const string GameStartedErrorMessage = "Can't join lobby. Game already started";
    public enum SceneList
    {
        GameScene,
        SampleScene
    }
}
