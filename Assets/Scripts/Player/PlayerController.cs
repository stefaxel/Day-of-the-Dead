using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private Color overworldColour;
    [SerializeField] private Color underworldColour;

    private Vector2 inputDirection;
    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;
    private bool isUnderworld;

    private void OnEnable()
    {
        WorldEventManager.ChangeWorld += OnWorldChange;
    }

    private void OnDisable()
    {
        WorldEventManager.ChangeWorld -= OnWorldChange;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        isUnderworld = false;
        sprite.color = overworldColour;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Movement();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        inputDirection = context.ReadValue<Vector2>().normalized;
    }

    public void OnWorldShift(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isUnderworld = !isUnderworld;

            if (isUnderworld)
            {
                Debug.Log("World will be shifted to the underworld");
                sprite.color = underworldColour;
                WorldEventManager.OnChangeWorld(isUnderworld);
            }
            else
            {
                Debug.Log("World will be shifted back to the overworld");
                sprite.color = overworldColour;
                WorldEventManager.OnChangeWorld(isUnderworld);
            }
        }
    }

    private void Movement()
    {
        rb2d.velocity = inputDirection * moveSpeed;
    }

    private void OnWorldChange(bool underworldActive)
    {
        isUnderworld = underworldActive;
        sprite.color = isUnderworld ? underworldColour : overworldColour;
    }
}
