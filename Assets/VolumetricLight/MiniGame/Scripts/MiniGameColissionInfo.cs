using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameColissionInfo : MonoBehaviour
{
    [SerializeField] float hitEffectThrushold;
    [SerializeField] UnityEvent onHitAction;

    void OnCollisionEnter(Collision other)
    {
        if (other.relativeVelocity.sqrMagnitude > hitEffectThrushold)
        {
            onHitAction.Invoke();
        }
    }
}
