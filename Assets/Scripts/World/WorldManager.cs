using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [SerializeField] private Color overworldColour;
    [SerializeField] private Color underworldColour;

    private SpriteRenderer groundSprite;

    private void Start()
    {
        groundSprite = GetComponent<SpriteRenderer>();

        groundSprite.color = overworldColour;
    }

    private void OnEnable()
    {
        WorldEventManager.ChangeWorld += ChangeWorld;
    }

    private void OnDisable()
    {
        WorldEventManager.ChangeWorld -= ChangeWorld;
    }

    private void ChangeWorld(bool changeWorld)
    {
        if (changeWorld)
        {
            Debug.Log("In World manager, change to the underworld");
            groundSprite.color = underworldColour;
        }
        else
        {
            Debug.Log("In World manager, change to the overworld");
            groundSprite.color = overworldColour;
        }
    }
}
