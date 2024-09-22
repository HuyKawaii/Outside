using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinNoiseTest : MonoBehaviour
{
    public float x;
    public float y;
    public void UpdateValue()
    {
        Debug.Log(Mathf.PerlinNoise(x, y) * 2 - 1);
    }
}
