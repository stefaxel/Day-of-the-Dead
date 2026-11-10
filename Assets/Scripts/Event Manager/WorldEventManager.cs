using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldEventManager
{
    public static event Action<bool> ChangeWorld;
    public static void OnChangeWorld(bool changeWorld) => ChangeWorld?.Invoke(changeWorld);

    public static event Action<int, int> ChangeHealthbarUI;
    public static void OnChangeHealthbarUI(int current, int max) => ChangeHealthbarUI.Invoke(current, max);

    public static event Action<float, float> ChangeSoulTimerUI;
    public static void OnChangeSoulTimerUI(float current, float max) => ChangeSoulTimerUI.Invoke(current, max);
}
