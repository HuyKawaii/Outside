using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilFunc
{
    static System.Random rnd = new System.Random(GameManager.Instance.worldSeed);

    public static bool Roll(float chance)
    {
        float ran = rnd.Next(0, 100) / (float)100;
        return ran < chance;
    }

    public static float GetHorizontalSpeed(Vector3 speed)
    {
        return Mathf.Sqrt(speed.x * speed.x + speed.z * speed.z);
    }

    public static Vector3 GetRectCenter(RectTransform rectTransform)
    {
        Vector3[] cornerArray = new Vector3[4];
        rectTransform.GetWorldCorners(cornerArray);
        Vector3 temp = Vector3.zero;
        foreach (Vector3 corner in cornerArray)
        {
            temp += corner;
        }
        return temp / 4;
    }

    public static float[] ConvertVector3ToFloat(Vector3 value)
    {
        float[] result = { value.x, value.y, value.z };
        return result;
    }

    public static Vector3 ConvertFloatToVector3(float[] value)
    {
        Vector3 result = new Vector3(value[0], value[1], value[2]);
        return result;
    }

    public static float[] ConvertVector4ToFloat(Vector4 value)
    {
        float[] result = { value.x, value.y, value.z , value.w};
        return result;
    }
}
