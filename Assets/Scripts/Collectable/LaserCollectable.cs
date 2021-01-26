using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserCollectable : Collectable
{
    public override CollectableType collectableType => CollectableType.AMMO;
}
