using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoatSoundEffects : MonoBehaviour
{
    [SerializeField] AudioSource waterMoveSound;
    [SerializeField] Buoyancy boatFloatControl;

    [SerializeField] Rigidbody rb;

    [SerializeField] float control, velocityCtrl, attenuation, foamLoudness;
    float recFactor = 0;

    void Update()
    {
        waterMoveSound.volume = Mathf.Pow((boatFloatControl.strainOnWaterEffect/control) * Mathf.Min(rb.velocity.sqrMagnitude/ velocityCtrl, 1), attenuation) * foamLoudness;
    }
}
