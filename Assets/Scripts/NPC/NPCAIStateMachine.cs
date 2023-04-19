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
    [SerializeField] private float farEnoughDistanceFromThreat = 30f;

    [Header("Chatting")]
    [SerializeField] private Transform eye;
    [SerializeField] private Transform friend = null;
    [SerializeField] private LayerMask npcLayers;
    [SerializeField] private float chattingRange = 5f;
    [SerializeField] private int numChattingAnimations = 5;
    [SerializeField] private float lastChatTime = 0f;
    [SerializeField] private float chatCoolDown = 20f;
    private NPC chatMate;

    [Header("Idle")]
    [SerializeField] private float lastIdleTime = 0f;
    [SerializeField] private float idleCoolDown = 5f;

    [Header("During Emergencies")]
    [SerializeField] private float useDistance = 2f;
    [SerializeField] private float runSpeed = 8f;
    [SerializeField] private AlarmSystem alarmSystem;

    private NavMeshAgent agent = null;

    private bool mustEnterCode = false;
    private bool alarmActivated = false;
    private float walkingSpeed;
    private NPC npc;

    private void Start()
    {
        currentState = NPCState.Walking;
        animator.speed = 1f;
        agent = GetComponent<NavMeshAgent>();
        npc = GetComponent<NPC>();

        if (agent != null)
        {
            waypointIndex = 0;
            ResetWayPoint();
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
        if (agent != null && waypoints.Length > 0)
        { 
            agent.SetDestination(waypoints[waypointIndex].position);
        }
    }

    public NPCState GetState()
    {
        return currentState;
    }
    public void SetState(NPCState newState)
    {
        animator.ResetTrigger("Chat");
        animator.ResetTrigger("tookDamage");
        animator.ResetTrigger("isEmergency");

        if (currentState == newState)
            return;
        currentState = newState;

        if (newState == NPCState.Walking)
        {
            // resume patrol
            agent.enabled = true;
        }
        else if (newState == NPCState.InDanger)
        {
            agent.enabled = true;

        }
        else if (newState == NPCState.Idle)
        {
            agent.enabled = false;
            animator.SetFloat("Forward", 0.0f);
            lastIdleTime = Time.time;
        }
        else if (newState == NPCState.Chatting)
        {
            agent.enabled = false;
            animator.SetFloat("Forward", 0.0f);
            lastChatTime = Time.time;
        }

        else if (newState == NPCState.Dead)
        {
            // disable navigation and play death animation
            agent.enabled = false;
            animator.SetFloat("Forward", 0.0f);
            animator.SetBool("isDead", true);
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
        else if (currentState == NPCState.InDanger)
        {
            // activate running
            animator.SetTrigger("isEmergency");
            RunAwayFromThreat();

        }
        else if (currentState == NPCState.Idle)
        {
            if (Time.time > (lastIdleTime + idleCoolDown))
            {
                SetState(NPCState.Walking);
                ResetWayPoint();
                Walk();
            }
        }
        else if (currentState == NPCState.Walking)
        {
            Walk();
        }
        else if (currentState == NPCState.Chatting)
        {
            if (Time.time > (lastChatTime + chatCoolDown))
            {
                StartCoroutine(npc.ResetFriendliness());
                SetState(NPCState.Idle);
            }
            else
            {
                Chat();
            }

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
        // TODO: Use these to check whether or not NPC is talking with a corpse lol
        chatMate = friend.GetComponent<NPC>();

        //chatMate.GetChatRequest(npc);

        NPCAIStateMachine chatMateAIStateMachine = chatMate.GetComponent<NPCAIStateMachine>();

        if (chatMate.isDead)
        {
            // panic
            SetThreat(chatMate.transform);
            SetState(NPCState.InDanger);

            return;
        }
        // if chat mate is threatened
        else if (chatMateAIStateMachine.GetState() == NPCState.InDanger)
        {
            // be threatened as well
            SetThreat(chatMateAIStateMachine.threat);
            SetState(NPCState.InDanger);

            return;
        }

        

        // if chat mate and npc are willing to engage in conversation
        if(chatMate.isFeelingFriendly && npc.isFeelingFriendly)
        {

            chatMateAIStateMachine.SetFriend(npc.transform);
            chatMateAIStateMachine.SetState(NPCState.Chatting);

            //float distanceToTarget = Vector3.Distance(agent.transform.position, player.transform.position);
            Vector3 direction = friend.transform.position - agent.transform.position;
            direction.y = 0.0f;

            if (direction.magnitude <= chattingRange)
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
        else
        {
            SetState(NPCState.Idle);
        }

    }

    private void RunAwayFromThreat()
    {
        //float distanceToTarget = Vector3.Distance(agent.transform.position, player.transform.position);
        Vector3 direction = threat.transform.position - agent.transform.position;
        direction.y = 0.0f;
        if (direction.magnitude > farEnoughDistanceFromThreat)
        {
            SetState(NPCState.Idle);
        }
        else
        {
            Vector3 destination = threat.position - new Vector3(farEnoughDistanceFromThreat, 0, farEnoughDistanceFromThreat);
            //// turn away the target
            //Quaternion lookRotation = Quaternion.LookRotation(destination);
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 0.1f);

            // we are not close enough to attack, but are close enough to detect
            // navigate away from threat
            agent.enabled = true; // walk toward destination
            agent.speed = runSpeed;
            agent.SetDestination(destination);
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