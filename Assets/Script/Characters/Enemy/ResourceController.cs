using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceController : EnemyController
{
    public ResourcesSettings resourceReference;

    protected override void Awake()
    {
        return;
    }
    protected override void Start()
    {
        return;
    }

    protected override void Update()
    {
        return;
    }

    public void SetReference(ResourcesSettings resourceReference)
    {
        this.resourceReference = resourceReference;
    }

    public ResourcesSettings GetReferencfe()
    {
        return this.resourceReference;
    }
}
