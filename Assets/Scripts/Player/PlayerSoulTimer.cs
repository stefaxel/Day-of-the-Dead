using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoulTimer : MonoBehaviour
{
    [Header("Soul Drain Settings")]
    [SerializeField] private float maxSoulTime;
    private float currentSoulTime;
    private Coroutine activeSoulRoutine;

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

    private void HandleWorldChange(bool underworldActive)
    {
        if (underworldActive)
        {
            currentSoulTime = maxSoulTime;

            if (activeSoulRoutine == null)
            {
                activeSoulRoutine = StartCoroutine(SoulTimer());
            }
        }
        else
        {
            if (activeSoulRoutine != null)
            {
                StopCoroutine(activeSoulRoutine);
                activeSoulRoutine = null;
            }
        }

    }

    private IEnumerator SoulTimer()
    {
        while(currentSoulTime > 0)
        {
            currentSoulTime -= Time.deltaTime;

            Debug.Log($"Current underworld time left: {currentSoulTime.ToString()}");

            yield return null;
        }

        activeSoulRoutine = null;
        WorldEventManager.OnChangeWorld(false);
    }
}
