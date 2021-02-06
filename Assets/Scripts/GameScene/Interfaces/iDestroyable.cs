using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IDestroyable
{
    event Action OnWillDestroy;
}
