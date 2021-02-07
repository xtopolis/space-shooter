using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiMatterCollectable : Collectable
{
    public override CollectableType collectableType => CollectableType.ANTIMATTER;
}
