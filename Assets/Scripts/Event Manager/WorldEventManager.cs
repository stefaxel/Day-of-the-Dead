using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventManager
{
    public static event Action<bool> ChangeWorld;
    public static void OnChangeWorld(bool changeWorld) => ChangeWorld?.Invoke(changeWorld);
}
