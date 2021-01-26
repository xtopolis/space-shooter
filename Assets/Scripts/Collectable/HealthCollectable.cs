using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectable : Collectable
{
    public override CollectableType collectableType => CollectableType.HEALTH;
}
