using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private Transform targetTransform;
    private float lookForTargetTimer;
    private float lookForTargetTimerMax = .2f;
    private Vector3 moveDir;
    private HealthSystem healthSystem;
    private Vector3 rotationVector;

    public static Enemy Instance { get; private set; }
    public static Enemy Create(Vector3 position)
    {
        Transform pfEnemy = Resources.Load<Transform>("pfEnemy");
        Transform enemyTransform = Instantiate(pfEnemy, position, Quaternion.identity);

        Enemy enemy = enemyTransform.GetComponent<Enemy>();

        return enemy;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        
        rigidbody2d = GetComponent<Rigidbody2D>();
        if (BuildingManager.Instance.GetHQBuilding() != null)
        {
            targetTransform = BuildingManager.Instance.GetHQBuilding().transform;


            moveDir = (targetTransform.position - transform.position).normalized;
            healthSystem = GetComponent<HealthSystem>();
            healthSystem.OnDied += HealthSystem_OnDied;

            lookForTargetTimer = Random.Range(0f, lookForTargetTimerMax);


            rotationVector = new Vector3(-0.0f, 0.0f, -0.5f);
            //transform.LookAt(rotationVector);
            transform.right = targetTransform.position - transform.position;
            //Quaternion rotation = Quaternion.LookRotation(targetTransform.position - transform.position, Vector3.up);
            //transform.rotation = rotation;
            //transform.rotation = Quaternion.LookRotation((targetTransform.position - transform.position));
            Debug.Log("rotation " + rotationVector);
            lookForTargetTimer = Random.Range(0f, lookForTargetTimerMax);
        }
    }


    private void HealthSystem_OnDied(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        HandleMovement();
        HandleTargeting();        

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Building building = collision.gameObject.GetComponent<Building>();
        if( building != null)
        {
            HealthSystem healthSystem = building.GetComponent<HealthSystem>();
            healthSystem.Damage(10);
            Destroy(gameObject);

        }
    }


    private void HandleMovement()
    {
        if (targetTransform != null)
        {
            Vector3 moveDir = (targetTransform.position - transform.position).normalized;
            transform.right = targetTransform.position - transform.position;
            float moveSpeed = 6f;
            rigidbody2d.velocity = moveDir * moveSpeed;
        }
        else
        {
            rigidbody2d.velocity = Vector2.zero;
        }
    }

    private void HandleTargeting()
    {
        lookForTargetTimer -= Time.deltaTime;
        if (lookForTargetTimer < 0f)
        {
            lookForTargetTimer += lookForTargetTimerMax;
            LookForTargets();
        }
    }

    private void LookForTargets()
    {
        float targetMaxRadius = 10f;
        Collider2D[] collider2DArray = Physics2D.OverlapCircleAll(transform.position, targetMaxRadius);

        foreach (Collider2D collider2D in collider2DArray)
        {
            Building building = collider2D.GetComponent<Building>();
            if (building != null)
            {
                // Is a building!
                if (targetTransform == null)
                {
                    targetTransform = building.transform;
                }
                else
                {
                    if (Vector3.Distance(transform.position, building.transform.position) <
                        Vector3.Distance(transform.position, targetTransform.position))
                    {
                        // Closer!
                        targetTransform = building.transform;
                    }
                }
            }
        }

        if (targetTransform == null)
        {   if(BuildingManager.Instance.GetHQBuilding() != null)
            {
                // Found no targets within range!
                targetTransform = BuildingManager.Instance.GetHQBuilding().transform;
            }

        }
    }




}
