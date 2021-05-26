using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavePointTester : MonoBehaviour
{
    void Update()
    {
        transform.position = SeaWaveProvider.instance.WavePointHeight(transform.position);
    }
}
