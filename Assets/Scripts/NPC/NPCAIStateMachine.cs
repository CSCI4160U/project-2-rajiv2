using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public enum NPCState
{
    Idle, Walking, InDanger, Chatting, Dead
}
[RequireComponent(typeof(NPC))]
public class NPCAIStateMachine : MonoBehaviour
{
    [SerializeField] private NPCState currentState = NPCState.Idle;

    [Header("Walking")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int waypointIndex = 0;
    [SerializeField] private bool walkingLoop = true;
    [SerializeField] private float closeEnoughDistance;

    [Header("Threat in Sight")]
    [SerializeField] private Transform threat = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private Transform[] consolePositions = null;

    [SerializeField] private LayerMask playerLayers;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private float farEnoughDistanceFromThreat;

    [Header("Chatting")]
    [SerializeField] private Transform eye;
    [SerializeField] private Transform friend = null;
    [SerializeField] private LayerMask npcLayers;
    [SerializeField] private float chattingRange = 5f;
    [SerializeField] private int numChattingAnimations = 5;
    [SerializeField] private float lastChatTime = 0f;
    [SerializeField] private float chatCoolDown = 20f;

    [Header("During Emergencies")]
    [SerializeField] private float useDistance;
    [SerializeField] private float runSpeed;
    [SerializeField] private AlarmSystem alarmSystem;

    private NavMeshAgent agent = null;

    private bool mustEnterCode = false;
    private bool alarmActivated = false;
    private float walkingSpeed;
    private NPC npc;

    private void Start()
    {

        animator.speed = 1f;
        agent = GetComponent<NavMeshAgent>();
        npc = GetComponent<NPC>();


        if (npc.isChatter)
        {
            currentState = NPCState.Chatting;
        }
        else if (npc.isWalker)
        {
            currentState = NPCState.Walking;
        }
        else
        {
            currentState = NPCState.Idle;
        }
            
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


    private void ResetWayPoint()
    {
        agent.enabled = true;
        waypointIndex = 0;
        agent.SetDestination(waypoints[waypointIndex].position);
    }

    public NPCState GetState()
    {
        return currentState;
    }
    public void SetState(NPCState newState)
    {
        if (currentState == newState)
            return;
        currentState = newState;

        if (newState == NPCState.Walking)
        {
            // resume patrol
            agent.enabled = true;
            waypointIndex = 0;
        }
        else if (newState == NPCState.Idle || newState == NPCState.Chatting)
        {
            agent.enabled = false;
            animator.SetFloat("Forward", 0.0f);
        }
        else if (newState == NPCState.InDanger)
        {
            animator.ResetTrigger("tookDamage");
        }
        else if (newState == NPCState.Dead)
        {
            // disable navigation and play death animation
            agent.enabled = false;
            animator.SetFloat("Forward", 0.0f);
            animator.SetBool("isDead", true);
            animator.ResetTrigger("isEmergency");
            mustEnterCode = false;
        }
    }
    public void SetThreat(Transform threat)
    {
        this.threat = threat;
    }

    public void SetFriend(Transform friend)
    {
        this.friend = friend;
    }

    private void Update()
    {
        if (currentState == NPCState.Dead)
        {
            return;
        }
        else if (currentState == NPCState.Walking)
        {
            Walk();
        }
        else if (currentState == NPCState.InDanger)
        {
            RunAwayFromThreat();

            // activate running
            animator.SetTrigger("isEmergency");
        }
        else if (currentState == NPCState.Chatting)
        {
            if (Time.time > (lastChatTime + chatCoolDown))
            {
                animator.ResetTrigger("Chat");
                ResetWayPoint();
                SetState(NPCState.Walking);
                Walk();
            }
            Chat();
        }

        if (mustEnterCode)
        {
            EnterCode();
        }

        if (alarmActivated || mustEnterCode || currentState == NPCState.InDanger)
        {
            agent.speed = runSpeed;
        }
        else
        {
            animator.ResetTrigger("isEmergency");
            agent.speed = walkingSpeed;
        }
    }

    private void EnterCode()
    {

        // run to alarm pad
        Transform closestConsole = consolePositions[0];

        // finding closest console
        for (int i = 0; i < consolePositions.Length; i++)
        {
            if (Mathf.Abs(Vector3.Distance(closestConsole.position, agent.transform.position)) >
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
    private void Walk()
    {

        Vector3 destination = waypoints[waypointIndex].position;
        float distanceToTarget = Vector3.Distance(agent.transform.position, destination);

        if (distanceToTarget < closeEnoughDistance)
        {
            waypointIndex++;
            // loop, if desired
            if (waypointIndex >= waypoints.Length)
            {
                if (walkingLoop)
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

    private void Chat()
    {
        // friend can be player or NPC

        NPC npc;
        Player player;

        //float distanceToTarget = Vector3.Distance(agent.transform.position, player.transform.position);
        Vector3 direction = friend.transform.position - agent.transform.position;
        direction.y = 0.0f;

        if (direction.magnitude < chattingRange)
        {
            agent.enabled = false; // don't walk
            animator.SetFloat("Forward", 0.0f);
            animator.SetInteger("ChatNum", Random.Range(1, numChattingAnimations));
            animator.SetTrigger("Chat");

            // we have previously started attacking

            // turn toward the friend
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.1f);
        }
        else if (direction.magnitude < chattingRange + 15)
        {

            // we are not close enough to attack, but are close enough to detect
            // navigate toward the target
            agent.enabled = true; // walk toward destination
            agent.speed = walkingSpeed;
            agent.SetDestination(friend.position);
            animator.SetFloat("Forward", agent.velocity.magnitude);

        }

    }

    private void RunAwayFromThreat()
    {
        //float distanceToTarget = Vector3.Distance(agent.transform.position, player.transform.position);
        Vector3 direction = threat.transform.position - agent.transform.position;
        direction.y = 0.0f;

        if (direction.magnitude > farEnoughDistanceFromThreat)
        {
            ResetWayPoint();
            //agent.enabled = false; // don't walk
            //animator.SetFloat("Forward", 0.0f);
            //animator.SetInteger("AttackNum", Random.Range(1, numAttackAnimations));
            //animator.SetTrigger("Attack");

            // we have previously started attacking

            // turn away the target
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.1f);
        }
        else
        {

            // we are not close enough to attack, but are close enough to detect
            // navigate toward the target
            agent.enabled = true; // walk toward destination
            agent.speed = runSpeed;
            agent.Move(threat.position + new Vector3(-farEnoughDistanceFromThreat, 0,-farEnoughDistanceFromThreat));
            animator.SetFloat("Forward", agent.velocity.magnitude);

        }

    }
    //private void Chat()
    //{
    //        animator.SetTrigger("Chat");

    //        // TODO: Do raycast to calculate damage
    //        //RaycastHit hit;

    //        //// if npc near another npc
    //        //if (Physics.Raycast(eye.position, eye.forward, out hit, chattingRange, npcLayers))
    //        //{
    //        //    Vector3 targetDirection = (hit.transform.position - eye.position).normalized;
    //        //    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
    //        //    transform.rotation = Quaternion.Slerp(eye.rotation, targetRotation, 0.5f);
    //        //}
    //}
}