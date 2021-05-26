using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameAtmosphereControl : MonoBehaviour
{
    public Material displayMaterial;
    public float updationRate;

    public LocationAtmosphere[] atmospheres;

    float nextUpdationDate;

    Vector3 avrageCentre;
    float pointEffectTotal;
    float fogStrength;
    Color combinedColor;

    private void Start()
    {
        for (int i = 0; i < atmospheres.Length; i++)
        {
            avrageCentre += atmospheres[i].atmospherePosition.position;
        }
        avrageCentre /= atmospheres.Length;

        for (int i = 0; i < atmospheres.Length; i++)
        {
            atmospheres[i].distanceToAvrageCentre = (atmospheres[i].atmospherePosition.position - avrageCentre).magnitude;
        }

    }

    void Update()
    {
        if (nextUpdationDate < Time.time)
        {
            CalculateData();
            ApplyColorShift();
            nextUpdationDate = Time.time + 1 / updationRate;
        }
    }

    void CalculateData()
    {
        pointEffectTotal = 0;
        fogStrength = 0;
        combinedColor = Color.black;
        for (int i = 0; i < atmospheres.Length; i++)
        {
            float pointContribution = Mathf.Max(1 - ((atmospheres[i].atmospherePosition.position - MiniGameGlobalRef.playerShip.position).magnitude / atmospheres[i].distanceToAvrageCentre), 0);
            fogStrength += atmospheres[i].fogDencity * pointContribution;
            combinedColor += atmospheres[i].atmosphereColor * pointContribution;
            pointEffectTotal += pointContribution;
        }
        combinedColor /= pointEffectTotal;
        fogStrength /= pointEffectTotal;
    }

    void ApplyColorShift()
    {
        displayMaterial.SetColor("_FogColor", combinedColor);
        displayMaterial.SetFloat("_FogCtrl", fogStrength);
    }

    [System.Serializable]
    public struct LocationAtmosphere
    {
        public Transform atmospherePosition;
        public Color atmosphereColor;
        public float fogDencity;
        public float exponentialValue;
        public float distanceToAvrageCentre;
    }

}
