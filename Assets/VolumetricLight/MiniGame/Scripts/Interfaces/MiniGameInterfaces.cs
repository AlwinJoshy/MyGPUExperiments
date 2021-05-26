using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Callable
{
    string GetName();
    bool CheckName(string name);
    void RunEvent(Transform objRef);
}
