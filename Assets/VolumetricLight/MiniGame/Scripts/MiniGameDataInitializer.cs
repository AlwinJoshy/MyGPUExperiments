using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameDataInitializer : MonoBehaviour
{
    [SerializeField] MiniGameItemLib itemLib;
    [SerializeField] MiniGameStatusLib statusLib;
    [SerializeField] MiniGamePirateGoodsLib pirateGoodsLib;
    [SerializeField] Transform inActiveObjectParent;

    void Awake()
    {
        MiniGameGlobalRef.itemLib = itemLib;
        MiniGameGlobalRef.statusLib = statusLib;
        MiniGameGlobalRef.pirateGoodsLib = pirateGoodsLib;
        MiniGameGlobalRef.SetGameState(MiniGameGlobalRef.GameState.neutral);
        MiniGameGlobalRef.inActiveObjectParent = inActiveObjectParent;
    }

}
