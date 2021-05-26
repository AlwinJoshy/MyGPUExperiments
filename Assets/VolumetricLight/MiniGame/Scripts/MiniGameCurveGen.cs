using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MiniGameCurveGen
{
    static Vector3 temp = Vector3.zero;
    static List<Vector3>
    tempPointListInput = new List<Vector3>(),
    tempPointListOutput = new List<Vector3>();
    

    public static Vector3 GetPointOnCurve(Curve pointCurve, float t)
    {
        bool done = false;
        tempPointListInput.Clear();
        tempPointListInput.AddRange(pointCurve.pointList);
        tempPointListOutput.Clear();

        int count = 5;

        while (!done)
        {
            count--;

            for (int i = 0; i < tempPointListInput.Count - 1; i++)
            {
                temp = tempPointListInput[i + 1] - tempPointListInput[i];
                tempPointListOutput.Add(tempPointListInput[i] + temp * t);
            }

            if(tempPointListOutput.Count == 1)done = true;
            
            else{
                tempPointListInput.Clear();
                tempPointListInput.AddRange(tempPointListOutput);
                tempPointListOutput.Clear();
            }

        }

        return tempPointListOutput[0];
    }


    public class Curve
    {
        public Vector3[] pointList;
        public float approxLength;

        public Vector3 GetPositionAtT(float t){
            return MiniGameCurveGen.GetPointOnCurve(this, t);
        }

    }
}
