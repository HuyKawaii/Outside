using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item List", menuName = "List/New Structure List")]
public class StructureListSO : ScriptableObject
{
    public List<StructureSetting> structureList;
}
