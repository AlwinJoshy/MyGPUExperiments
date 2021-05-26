using UnityEngine;

public class RenderFoam : MonoBehaviour
{
    [SerializeField] bool isSetup;
    void Start()
    {
        Setup();
    }

    public void Setup()
    {
        if (!isSetup)
        {
            RenderFormManager.instance.AddRenderer(GetComponent<Renderer>());
            isSetup = true;
        }
    }
}
