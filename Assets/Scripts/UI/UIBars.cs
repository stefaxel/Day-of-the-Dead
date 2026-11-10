using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBars : MonoBehaviour
{
    [Header("UI Bars")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider SoulTimerSlider;

    private void Start()
    {
        // Set sliders to 100% full right when the scene loads
        if (healthSlider != null) healthSlider.value = 1f;
        if (SoulTimerSlider != null) SoulTimerSlider.value = 1f;
    }

    private void OnEnable()
    {
        WorldEventManager.ChangeHealthbarUI += UpdateHealthbar;
        WorldEventManager.ChangeSoulTimerUI += UpdateSoulTimerSlider;
    }

    private void OnDisable()
    {
        WorldEventManager.ChangeHealthbarUI -= UpdateHealthbar;
        WorldEventManager.ChangeSoulTimerUI -= UpdateSoulTimerSlider;
    }

    private void UpdateHealthbar(int current, int max)
    {
        healthSlider.value = (float)current / max;
    }

    private void UpdateSoulTimerSlider(float current, float max)
    {
        SoulTimerSlider.value = current / max;
    }
}
