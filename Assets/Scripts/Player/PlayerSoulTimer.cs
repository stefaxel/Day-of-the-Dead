using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoulTimer : MonoBehaviour
{
    [Header("Soul Drain Settings")]
    [SerializeField] private float maxSoulTime;
    [SerializeField] private float cooldownDuration;

    private float currentSoulTime;
    private float currentCooldownTime = 0f;

    private bool isUnderworld = false;
    private bool isRecharging = false;
    private Coroutine activeSoulRoutine;

    public bool CanShift
    {
        get { return !isUnderworld && !isRecharging; }
    } 

    private void OnEnable()
    {
        WorldEventManager.ChangeWorld += HandleWorldChange;
    }

    private void OnDisable()
    {
        WorldEventManager.ChangeWorld -= HandleWorldChange;
    }

    private void Start()
    {
        currentSoulTime = maxSoulTime;
    }

    private void Update()
    {
        // Tick down the recharge cooldown whilst in the overworld
        if (isRecharging)
        {
            currentCooldownTime -= Time.deltaTime;
            if(currentCooldownTime <= 0f)
            {
                isRecharging = false;
                Debug.Log("Soul Energy Fully Recharged");
            }
        }
    }

    private void HandleWorldChange(bool underworldActive)
    {
        isUnderworld = underworldActive;

        if (isUnderworld)
        {
            // Reset soul energy timer when entering the underworld
            currentSoulTime = maxSoulTime;

            // Start the coroutine if it equals null, prevent multiple coroutines running
            if (activeSoulRoutine == null)
            {
                activeSoulRoutine = StartCoroutine(SoulTimer());
            }
        }
        else
        {
            // Condition to allow exiting the underworld early
            if (activeSoulRoutine != null)
            {
                StopCoroutine(activeSoulRoutine);
                activeSoulRoutine = null;
            }

            StartCooldown();
        }

    }

    private void StartCooldown()
    {
        isRecharging = true;
        currentCooldownTime = cooldownDuration;
    }

    private IEnumerator SoulTimer()
    {
        // Drains soul time and forces a return when time hits 0
        while(currentSoulTime > 0)
        {
            currentSoulTime -= Time.deltaTime;

            yield return null;
        }

        // Timer expired and clears the coroutine, forces player back to the overworld
        activeSoulRoutine = null;
        WorldEventManager.OnChangeWorld(false);
    }
}
