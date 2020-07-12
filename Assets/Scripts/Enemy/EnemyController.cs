using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float initialHealth;
    public float movementSpeed;

    [Header("Idle")]
    public float timeInIdle;
    public float idleToShootTime;

    [Header("Patrol")]
    public float patrolToShootTime;
    public float sightAngle;
    public float sightDistance;

    [Header("Shoot")]
    public GameObject bullet;
    public Transform firePoint;
    public float minTimeBetweenShots;
    public float maxTimePlayerOffSight;
    public float bulletForce;
    public float bulletDamage;

    private NavMeshAgent agent;
    private PlayerController player;

    private GameObject[] patrolTargets;

    private float health;

    private bool onTransition;
    private float currentTime;

    private enum States
    {
        INITIAL,
        IDLE,
        PATROL,
        SHOOT
    }

    private States currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindObjectOfType<PlayerController>();

        patrolTargets = GameObject.FindGameObjectsWithTag("PatrolAI");

        SetInitialState();
    }

    private void Update()
    {
        switch (currentState)
        {
            case States.INITIAL:
                UpdateInitialState();
                break;
            case States.IDLE:
                UpdateIdleState();
                break;
            case States.PATROL:
                UpdatePatrolState();
                break;
            case States.SHOOT:
                UpdateShootState();
                break;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit))
        {
            Debug.Log(hit.collider.gameObject);
        }
    }

    #region Initial
    private void SetInitialState()
    {
        currentState = States.INITIAL;

        onTransition = false;
        currentTime = 0f;
        agent.isStopped = true;

        health = initialHealth;
    }

    private void UpdateInitialState()
    {
        //Wait until round starts
        SetPatrolState();
    }
    #endregion

    #region Idle
    private void SetIdleState()
    {
        currentState = States.IDLE;

        onTransition = false;
        currentTime = 0f;
        agent.isStopped = true;
    }

    private void UpdateIdleState()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, player.transform.position - transform.position, out hit) && !onTransition)
        {
            if(hit.collider.gameObject == player)
            {
                StartCoroutine(TransitionToShoot(idleToShootTime));
                return;
            }
        }

        currentTime += Time.deltaTime;

        if(currentTime >= timeInIdle && !onTransition)
        {
            SetPatrolState();
            return;
        }
    }
    #endregion

    #region Patrol
    private void SetPatrolState()
    {
        currentState = States.PATROL;

        onTransition = false;
        currentTime = 0f;
        agent.isStopped = false;

        SearchNewPatrolTarget();
    }

    private void UpdatePatrolState()
    {
        if (!agent.hasPath && agent.pathStatus == NavMeshPathStatus.PathComplete && !onTransition)
        {
            SetIdleState();
            return;
        }

        if(Vector3.Distance(transform.position, player.transform.position) < sightDistance && !onTransition)
        {
            if (Vector3.Angle(transform.forward, player.transform.position - transform.position) <= sightAngle)
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit))
                {
                    if (hit.collider.gameObject == player)
                    {
                        StartCoroutine(TransitionToShoot(patrolToShootTime));
                        return;
                    }
                }
            }
        }
    }

    private void SearchNewPatrolTarget()
    {
        int idx;
        if (health == initialHealth)
        {
            patrolTargets = patrolTargets.OrderBy((target) => (target.transform.position - transform.position).sqrMagnitude).ToArray();
            idx = Random.Range(1, 4);
        }
        else
        {
            patrolTargets = patrolTargets.OrderBy((target) => (target.transform.position - player.transform.position).sqrMagnitude).ToArray();
            idx = Random.Range(0, 3);
        }

        agent.SetDestination(patrolTargets[idx].transform.position);
    }
    #endregion

    #region Shoot
    private void SetShootState()
    {
        currentState = States.SHOOT;

        onTransition = false;
        currentTime = 0f;
        agent.isStopped = true;

        Vector3 nullYPlayerPos = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);

        transform.LookAt(nullYPlayerPos);

        Shoot();
    }

    private void UpdateShootState()
    {
        currentTime += Time.deltaTime;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit))
        {
            if (hit.collider.gameObject == player)
            {
                if (currentTime >= minTimeBetweenShots)
                {
                    SetShootState();
                    return;
                }
            }
            else if (currentTime >= maxTimePlayerOffSight)
            {
                SetPatrolState();
                return;
            }
        }
        else if (currentTime >= maxTimePlayerOffSight)
        {
            SetPatrolState();
            return;
        }
    }

    private IEnumerator TransitionToShoot(float time)
    {
        agent.isStopped = true;
        onTransition = true;
        yield return new WaitForSeconds(time);
        SetShootState();
    }
    #endregion

    public void Hit(float damage)
    {
        health -= damage;

        if(currentState != States.SHOOT)
        {
            SetShootState();
        }
    }

    private void Shoot()
    {
        GameObject currentBullet = Instantiate(bullet, firePoint.position, transform.rotation);
        currentBullet.GetComponent<Rigidbody>().AddForce(currentBullet.transform.forward * bulletForce, ForceMode.Impulse);
        Physics.IgnoreCollision(currentBullet.GetComponent<Collider>(), GetComponent<Collider>());
    }
}
