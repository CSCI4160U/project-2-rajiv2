using UnityEngine;
using UnityEngine.AI;
public enum EnemyState
{
    Patrolling, Alerted, TargetVisible, Dead
}
[RequireComponent(typeof(Enemy))]
public class EnemyAIStateMachine : MonoBehaviour
{
    [SerializeField]
    private EnemyState currentState = EnemyState.Patrolling;
    [Header("Patrolling")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int waypointIndex = 0;
    [SerializeField] private bool patrolLoop = true;
    [SerializeField] private float closeEnoughDistance;
    [Header("Alerted")]
    [SerializeField] private float lastAlertTime = 0.0f;
    [SerializeField] private float alertCooldown = 8.0f;
    [SerializeField] private Vector3 lastKnownTargetPosition;
    [Header("Target in Sight")]
    [SerializeField] private float lastShootTime = 0.0f;
    [SerializeField] private float shootCooldown = 1.0f;
    [SerializeField] private Transform target = null;
    [SerializeField]  private Animator animator = null;
    [SerializeField] private Transform[] consolePositions = null;
    [Header("Attacking")]
    [SerializeField] private float attackDistance;
    [Header("During Emergencies")]
    [SerializeField] private float useDistance;
    
    [SerializeField] private float runSpeed;
    [SerializeField] private AlarmSystem alarmSystem;
    private NavMeshAgent agent = null;
    
    private bool mustEnterCode = false;
    private bool mustAttack = false;
    private bool alarmActivated = false;
    private float walkingSpeed;
    private Enemy enemy;

    private void Start()
    {
        currentState = EnemyState.Patrolling;
        animator.speed = 1f;
        agent = GetComponent<NavMeshAgent>();
        enemy = GetComponent<Enemy>();
        if (agent != null && waypoints.Length > 0)
        {
            agent.SetDestination(waypoints[waypointIndex].position);
            walkingSpeed = agent.speed;
        }
    }
    public void Pause()
    {
        agent.enabled = false;
        animator.speed = 0f;
    }
    public void Resume()
    {
        agent.enabled = true;
        animator.speed = 1f;
    }
    public EnemyState GetState()
    {
        return currentState;
    }
    public void SetState(EnemyState newState)
    {
        if (currentState == newState)
            return;
        currentState = newState;
        if (newState == EnemyState.Patrolling)
        {
            // resume patrol
            agent.enabled = true;
            waypointIndex = 0;
        }
        else if (newState == EnemyState.Alerted)
        {
            // investigate the last known position

            //agent.enabled = true;
            //agent.SetDestination(lastKnownTargetPosition);
            // remember when we were alerted
            lastAlertTime = Time.time;
        }
        else if (newState == EnemyState.TargetVisible)
        {

            // shoot at the player
            //agent.enabled = false;
            if (enemy.isShooter)
            {
                animator.SetFloat("Forward", 0.0f);
            }
            
            lastKnownTargetPosition = target.transform.position;
        }
        else if (newState == EnemyState.Dead)
        {
            // disable navigation and play death animation
            agent.enabled = false;
            animator.SetFloat("Forward", 0.0f);
            animator.SetBool("Dead", true);
        }
    }
    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    public void SetLastKnownTargetPosition(Vector3
    lastKnownTargetPosition)
    {
        this.lastKnownTargetPosition = lastKnownTargetPosition;
    }
    private void Update()
    {
        if (currentState == EnemyState.Dead)
        {
            return;
        }
        else if (currentState == EnemyState.Patrolling)
        {
            Patrol();
        }
        else if (currentState == EnemyState.Alerted)
        {
            // check for timeout
            if (Time.time > (lastAlertTime + alertCooldown))
            {
                SetState(EnemyState.Patrolling);
                Patrol();
            }
            else
            {
                Alert();
            }
        }
        else if (currentState == EnemyState.TargetVisible)
        {
            // if hostile
            if (enemy.isShooter)
            {
                Shoot();
            }
            else if (enemy.isBrawler)
            {
                mustAttack = true;
                
            }
            else if (!alarmActivated)
            {
                mustEnterCode = true;
            }
            else
            {
                // Run Away
            }
            
            // activate running
            animator.SetTrigger("isEmergency");
        }

        if (mustEnterCode)
        {
            EnterCode();
        }

        if (mustAttack)
        {
            ChaseAndAttackTarget();
        }

        if (alarmActivated || mustEnterCode || mustAttack)
        {
            agent.speed = runSpeed;
        }
        else
        {
            agent.speed = walkingSpeed;
        }
    }

    private void ChaseAndAttackTarget()
    {
        Player player = target.GetComponent<Player>();

        float distanceToTarget = Vector3.Distance(agent.transform.position, player.transform.position);
        Debug.Log("dista: " + distanceToTarget);
        if (distanceToTarget < attackDistance)
        {
            animator.SetFloat("Forward", 0.0f);
            // Randomly do attack
            animator.SetTrigger("Punch");
            //animator.SetTrigger("Kick");
            
            if(player != null)
            {
                // deal damage
                player.TakeMeleeHit(enemy);
            }
            

        }
        else
        {
            agent.SetDestination(target.position);
            animator.SetFloat("Forward", agent.velocity.magnitude);
            agent.speed = runSpeed;
        }
    }
    private void EnterCode()
    {
        
        // run to alarm pad
        Transform closestConsole = consolePositions[0];

        // finding closest console
        for(int i = 0; i < consolePositions.Length; i++)
        {
            if(Mathf.Abs(Vector3.Distance(closestConsole.position, agent.transform.position)) > 
                Mathf.Abs(Vector3.Distance(consolePositions[i].position, agent.transform.position)))
            {
                closestConsole = consolePositions[i];
            }
        }
        
        float distanceToTarget = Vector3.Distance(agent.transform.position, closestConsole.position);
        if (distanceToTarget <= useDistance)
        {
            animator.SetTrigger("EnterCode");
            animator.SetFloat("Forward", 0.0f);
            agent.transform.rotation = Quaternion.Euler(agent.transform.rotation.x, closestConsole.eulerAngles.y, agent.transform.rotation.z);
            agent.speed = walkingSpeed;


            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Enter Code") &&
                !animator.IsInTransition(0) &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7)
            {
                // if animation done, alarm begins (set off lights and alarm)
                
                if (alarmSystem != null)
                {
                    alarmSystem.gameObject.SetActive(true);
                    alarmSystem.Activate();
                    animator.ResetTrigger("EnterCode");
                    // enable red light component (use HDRP if possible to get lights done properly)
                }

                mustEnterCode = false;
                alarmActivated = true;

                // game over
            }


        }
        else
        {
            agent.SetDestination(closestConsole.position);
            animator.SetFloat("Forward", agent.velocity.magnitude);
        }

        

        
    }
    private void Patrol()
    {
        mustAttack = false;
        
        Vector3 destination = waypoints[waypointIndex].position;
        float distanceToTarget = Vector3.Distance(agent.transform.position, destination);

        if (distanceToTarget < closeEnoughDistance)
        {
            waypointIndex++;
            // loop, if desired
            if (waypointIndex >= waypoints.Length)
            {
                if (patrolLoop)
                {
                    waypointIndex = 0;

                }
                else
                {
                    animator.SetFloat("Forward", 0.0f);
                    return;
                }
            }
            agent.SetDestination(waypoints[waypointIndex].position);
        }
        animator.SetFloat("Forward", agent.velocity.magnitude);
    }
    private void Alert()
    {
        float distanceToTarget = Vector3.Distance(agent.transform.position, lastKnownTargetPosition);
        if (distanceToTarget < closeEnoughDistance)
        {
            // stay here
            animator.SetFloat("Forward", 0.0f);
            // TODO: Look around animation
        }
        else
        {
            animator.SetFloat("Forward", agent.velocity.magnitude);
        }
    }
    private void Shoot()
    {
        if (Time.time > (lastShootTime + shootCooldown))
        {
            Vector3 targetDirection = (target.transform.position -
            transform.position).normalized;
            Quaternion targetRotation =
            Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation,
            targetRotation, 0.15f);
            lastShootTime = Time.time;
            animator.SetTrigger("Shoot");
            // TODO: Do raycast to calculate damage
        }

    }
}