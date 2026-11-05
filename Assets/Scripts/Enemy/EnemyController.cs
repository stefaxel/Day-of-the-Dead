using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Basic Settings")]
    [SerializeField] private float speed;
    [SerializeField] private float stopDistance;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Color overworldColour;
    [SerializeField] private Color underworldColour;

    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;
    private Vector2 direction;
    private int currentPoint = 0;
    private bool movingForward = true;
    private bool isFrozenInUnderworld = false;

    private void OnEnable()
    {
        WorldEventManager.ChangeWorld += ChangeWorld;
    }

    private void OnDisable()
    {
        WorldEventManager.ChangeWorld -= ChangeWorld;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

        sprite.color = overworldColour;
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (!isFrozenInUnderworld)
        {
            WaypointNavigation();
        }
        
    }

    private void FixedUpdate()
    {
        if (!isFrozenInUnderworld)
        {
            rb2d.velocity = direction * speed;
        }
        else if (isFrozenInUnderworld)
        {
            rb2d.velocity = Vector2.zero;
        }
    }


    private void WaypointNavigation()
    {
        float distance = Vector2.Distance(transform.position, waypoints[currentPoint].position);

        if(distance <= stopDistance)
        {
            AdvanceToNextWaypoint();
        }
        else
        {
            direction = ((Vector2)waypoints[currentPoint].position - (Vector2)transform.position).normalized;
        }
    }

    private void AdvanceToNextWaypoint()
    {
        if(currentPoint >= waypoints.Length - 1)
        {
            movingForward = false;
        }
        else if(currentPoint <= 0)
        {
            movingForward = true;
        }

        if (movingForward)
        {
            currentPoint++;
        }
        else
        {
            currentPoint--;
        }
    }

    private void ChangeWorld(bool changeWorld)
    {
        if (changeWorld)
        {
            sprite.color = underworldColour;
            isFrozenInUnderworld = true;

            direction = Vector2.zero;
        }
        else
        {
            sprite.color = overworldColour;
            isFrozenInUnderworld = false;
        }
    }
}
