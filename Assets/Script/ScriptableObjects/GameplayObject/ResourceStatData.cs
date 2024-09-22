using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StatData", menuName = "Combat/OreStatData")]
public class ResourceStatData : CharacterStatData
{
    public Tool.ToolType toolType;
    public int toolLevel;
}
