using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure : Interactalble
{
    [SerializeField]
    protected StructureSetting structureReference; 

    public StructureSetting GetStructureReference()
    {
        return structureReference;
    }

    public void SetStructureReference(StructureSetting structureSetting)
    {
        structureReference = structureSetting;
    }
}
