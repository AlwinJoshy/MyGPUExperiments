using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameTestScript : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L)){
            
            MiniGamePlayerDeathManager.instance.CalculateHitDamage();

            // Clrl + K + C (commenting)
            // Ctrl + K + U (uncommenting)
            // Shift + Alt + A (commenting multipple line)
            // MiniGameErrand newErrand = new MiniGameErrand();
            // MiniGamePortProperty collectionPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere).AddComponent<MiniGamePortProperty>();
            // collectionPoint.transform.position = Vector3.right * 3 + Vector3.up *-1f;
            // collectionPoint.portName = "Test Port Sphere";
            // newErrand.collectFromPoint = collectionPoint;
            // MiniGameErrandManager.instance.SetErrand(newErrand);
            
        }
    }

    void OnDrawGizmos() {

    }

}
