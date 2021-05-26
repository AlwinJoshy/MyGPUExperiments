using UnityEngine;
using UnityEngine.Events;

public class MiniGameCallables : MonoBehaviour, Callable
{
    public string callableName;
    public UnityEventObjectRefrence unityEvent;

    public void Start() {
        MiniGameCallableEngine.instance.AddCallable(this);
    }

    public string GetName(){
        return callableName;
    }

    public bool CheckName(string name)
    {
        if(this.callableName == name) return true;
        else return false;
    }

    public void RunEvent(Transform objRef)
    {
       unityEvent.Invoke(objRef);
    }

}

[System.Serializable]
public class UnityEventObjectRefrence : UnityEvent<Transform>{}
