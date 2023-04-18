using System.Collections;
using UnityEngine;

[RequireComponent(typeof(NPCAIStateMachine))]
public class NPCFieldOfView : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private GameObject eye;
    [Range(0, 360)] [SerializeField] private float fovAngle = 60f;
    [SerializeField] private float visionRadius = 10f;

    [SerializeField] private LayerMask threatLayer;
    [SerializeField] private LayerMask npcLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float interactionScanDelay = 0.25f;

    private NPCAIStateMachine stateMachine;

    private void Start()
    {
        stateMachine = GetComponent<NPCAIStateMachine>();
        StartCoroutine(KeepSearchingForInteractions(interactionScanDelay));
    }
    public void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        // draw colour by state
        Vector3 focusPos = eye.transform.position + eye.transform.forward * visionRadius;
        focusPos.y = eye.transform.position.y;
        if (stateMachine != null)
        {
            if (stateMachine.GetState() == NPCState.InDanger)
            {
                Gizmos.color = Color.red;
            }
            else if (stateMachine.GetState() == NPCState.Chatting)
            {
                Gizmos.color = Color.blue;
            }
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(eye.transform.position, 0.2f);
        Gizmos.DrawWireSphere(eye.transform.position, visionRadius);
        Gizmos.DrawLine(eye.transform.position, focusPos);
    }

    IEnumerator KeepSearchingForInteractions(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            if (stateMachine.GetState() != NPCState.Dead)
            {
                LookForFriends();
                LookForThreats();
            }
        }
    }

    private void LookForFriends()
    {
        Collider[] friends = Physics.OverlapSphere(eye.transform.position,
        visionRadius, npcLayer);

        for (int i = 0; i < friends.Length; i++)
        {

            Vector3 friendPos = friends[i].transform.position;
            friendPos.y += 1.0f;
            Vector3 friendDirection = (friendPos - eye.transform.position).normalized;

            // is friend within fov?
            if (Vector3.Angle(eye.transform.forward, friendDirection) < fovAngle / 2)
            {
                // do we have line of sight?
                float distance = Vector3.Distance(eye.transform.position, friendPos);

                if (!Physics.Raycast(eye.transform.position, friendDirection,
                distance, wallLayer) && !Physics.Raycast(eye.transform.position, friendDirection,
                distance, threatLayer))
                {

                    if (friends[i].GetComponent<NPC>().isFeelingFriendly)
                    {
                        // change to chatting state
                        stateMachine.SetFriend(friends[i].transform);
                        stateMachine.SetState(NPCState.Chatting);
                    }

                    return;

                }
            }
        }
    }

    private void LookForThreats()
    {
        Collider[] threats = Physics.OverlapSphere(eye.transform.position,
        visionRadius, threatLayer);

        for (int i = 0; i < threats.Length; i++)
        {

            Vector3 threatPos = threats[i].transform.position;
            threatPos.y += 1.0f;
            Vector3 threatDirection = (threatPos - eye.transform.position).normalized;

            // is the threat within fov?
            if (Vector3.Angle(eye.transform.forward, threatDirection) < fovAngle / 2)
            {
                // do we have line of sight?
                float distance = Vector3.Distance(eye.transform.position, threatPos);

                if (!Physics.Raycast(eye.transform.position, threatDirection,
                distance, wallLayer) && !Physics.Raycast(eye.transform.position, threatDirection,
                distance, npcLayer))
                {
                    // change to In Danger state
                    stateMachine.SetThreat(threats[i].transform);
                    stateMachine.SetState(NPCState.InDanger);

                    return;

                }
            }
        }
    }
}
