using UnityEngine;

[CreateAssetMenu(fileName = "New Lighting Preset", menuName = "Scriptable Objects/Lighting Preset")]
public class LightingPreset : ScriptableObject
{
    public Gradient directionalLightColor;
    public Gradient ambientColor;
    public Gradient fogColor;
}
