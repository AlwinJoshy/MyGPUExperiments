using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameCaller : MonoBehaviour
{
    public string callerName;
    public Transform transformRef;

    public void CallCallable(){
        MiniGameCallableEngine.instance.Run(callerName, transformRef);
    }
}
