using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MiniGameGlobalRef
{

    #region General_Game_Data
    public static Stack<GameState> gameState = new Stack<GameState>();
    public static Transform inActiveObjectParent;
    #endregion

    #region UI and Canvas
    public static MiniGameUI gameUI;
    public static SerializedProperty uiEditLocation;
    #endregion

    #region playerData refrences
    public static Transform playerShip;
    public static MiniGamePlayerStat playerStat;
    public static Rigidbody playerRB;
    public static int heartCount;

    public static void StopPlayerMovement()
    {
        playerRB.velocity = Vector3.zero;
    }

    #endregion

    #region bullet and ammm
    public static Queue<BezierProjectile> allBalls = new Queue<BezierProjectile>();
    public static float gunStrikeHeight;
    public static int ballStrikeSurfaceType;
    #endregion

    #region Items_and_Status
    public static MiniGamePirateGoodsLib pirateGoodsLib;
    public static MiniGameItemLib itemLib;
    public static MiniGameStatusLib statusLib;
    public static bool itemChanged = true;
    public static bool statusChanged = true;
    #endregion


    #region Util_Functions
    public static int GenerateRandomID()
    {
        return UnityEngine.Random.Range(1000, 9999);
    }

    public static bool BeInControl()
    {
        return (gameState.Peek() == GameState.neutral || gameState.Peek() == GameState.combat);
    }

    public static bool CanPromptShop()
    {
        return (gameState.Peek() == GameState.neutral || gameState.Peek() == GameState.shop || gameState.Peek() == GameState.shop);
    }

    public static bool CanPromptTalk()
    {
        return (gameState.Peek() == GameState.neutral);
    }

    public static void SetGameState(GameState newGameState)
    {
        gameState.Push(newGameState);
    }

    public static void ResetGameState()
    {
        gameState.Pop();
    }

    #endregion

    public enum GameState
    {
        neutral,
        combat,
        shop,
        talk,
        over
    }

}
