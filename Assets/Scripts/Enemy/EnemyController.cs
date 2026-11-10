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

    [Header("State Machine Settings")]
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float chaseDistance;
    [SerializeField] private float catchDistance;
    private EnemyState currentState;
    private Transform playerTransform;
    private Transform lastWaypoint;

    [Header("Attack Settings")]
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackCooldown = 1.5f;
    private float attackCooldownTimer;

    private Rigidbody2D rb2d;
    private SpriteRenderer sprite;

    private Vector2 direction;

    private int currentPoint = 0;

    private bool movingForward = true;

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

        currentState = EnemyState.Patrol;
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }

        FindTarget();

        HandleStateBehaviour();

        if (IsInAttackRange())
        {
            if(currentState == EnemyState.Patrol ||
                currentState == EnemyState.Chase)
            {
                currentState = EnemyState.OverworldAttack;
            }
            else if(currentState == EnemyState.Stop)
            {
                currentState = EnemyState.UnderworldAttack;
            }
        }

    }

    private void FixedUpdate()
    {
        // Make sure that enemy can still move when going to the chase state
        if (currentState == EnemyState.Patrol || currentState == EnemyState.Chase
        || currentState == EnemyState.Return || currentState == EnemyState.OverworldAttack)
        {
            rb2d.velocity = direction * speed;
        }
        else if (currentState == EnemyState.Stop || currentState == EnemyState.UnderworldAttack)
        {
            rb2d.velocity = Vector2.zero; // Frozen/Underworld enemy stays completely still
        }
    }

    private void HandleStateBehaviour()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                WaypointNavigation();
                break;

            case EnemyState.Chase:
                float distanceFromWaypoint = Vector2.Distance(transform.position, lastWaypoint.position);

                if(distanceFromWaypoint <= chaseDistance)
                {
                    ChasePlayer();
                }
                else
                {
                    currentState = EnemyState.Return;
                }
                break;

            case EnemyState.Return:
                ReturnToWaypoint();
                break;

            case EnemyState.UnderworldAttack:
                Attack();
                break;

            case EnemyState.OverworldAttack:
                Attack();
                break;

        }
    }

    private void FindTarget()
    {
        // Using the a layer mask to find the collider and transform of the target
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);

        if(playerCollider != null && currentState == EnemyState.Patrol)
        {
            Transform player = playerCollider.transform;
            
            if(player != null)
            {
                lastWaypoint = waypoints[currentPoint];
                playerTransform = player;
                currentState = EnemyState.Chase;
            }
        }
    }

    private void ChasePlayer()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position); ;

        if (distanceToPlayer <= detectionRadius)
        {
            direction = (playerTransform.position - transform.position).normalized;
        }

        if (distanceToPlayer <= catchDistance)
        {
            direction = Vector2.zero;
        }

    }

    private void Attack()
    {
        if(currentState == EnemyState.OverworldAttack)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position); ;

            if (distanceToPlayer <= attackRange)
            {
                direction = (playerTransform.position - transform.position).normalized;
            }

            if (distanceToPlayer <= catchDistance)
            {
                direction = Vector2.zero;
            }
        }
        else if(currentState == EnemyState.UnderworldAttack)
        {
            direction = Vector2.zero;
        }

        if(attackCooldownTimer <= 0)
        {
            DealDamageToPlayerInRange();
            attackCooldownTimer = attackCooldown;
        }
    }

    private void DealDamageToPlayerInRange()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);

        if (playerCollider != null)
        {
            IHealth playerHealth = playerCollider.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.DealDamage(attackDamage);
                Debug.Log($"Dealing: {attackDamage} points of damage");
            }
        }
    }

    private bool IsInAttackRange()
    {
        if (playerTransform == null) return false;

        float distanceFromPlayer = Vector2.Distance(transform.position, playerTransform.position); ;

        if(distanceFromPlayer <= attackRange)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ReturnToWaypoint()
    {
        float distanceFromWaypoint = Vector2.Distance(transform.position, lastWaypoint.position);

        if (distanceFromWaypoint <= stopDistance)
        {
            currentState = EnemyState.Patrol;
        }

        direction = ((Vector2)lastWaypoint.position - (Vector2)transform.position).normalized;
    }


    private void WaypointNavigation()
    {
        float distance = Vector2.Distance(transform.position, waypoints[currentPoint].position);

        if (distance <= stopDistance)
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
        if (currentPoint >= waypoints.Length - 1)
        {
            movingForward = false;
        }
        else if (currentPoint <= 0)
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
            currentState = EnemyState.Stop;

            direction = Vector2.zero;
        }
        else
        {
            sprite.color = overworldColour;
            currentState = EnemyState.Patrol;
        }
    }
}
