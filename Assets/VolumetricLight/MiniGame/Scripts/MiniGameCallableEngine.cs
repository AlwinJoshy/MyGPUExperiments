using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameCallableEngine : MonoBehaviour
{
    public static MiniGameCallableEngine instance;
    List<Callable> allCallables = new List<Callable>();

    void Awake() {
        instance = this;
    }

    public void Run(string callableName, Transform objRef){
        for (int i = 0; i < allCallables.Count; i++)
        {
            if(allCallables[i].CheckName(callableName)){
                allCallables[i].RunEvent(objRef);
                break;
            }
        }
    }

    public void AddCallable(Callable newCallable){
        allCallables.Add(newCallable);
    }

    public void RemoveCallable(Callable ejectCallable){
        allCallables.Remove(ejectCallable);
    }

}
