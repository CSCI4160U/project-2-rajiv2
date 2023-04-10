using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAIStateMachine))]
public class FieldOfView : MonoBehaviour
{
    [Header("Vision")]
    [SerializeField] private GameObject eye;
    [Range(0, 360)] [SerializeField] private float fovAngle = 60f;
    [SerializeField] private float visionRadius = 10f;

    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private float targetScanDelay = 0.25f;
    [Header("State Management")]
    [SerializeField] private bool isAlerted = false;
    private EnemyAIStateMachine stateMachine;

    private void Start()
    {
        stateMachine = GetComponent<EnemyAIStateMachine>();
        StartCoroutine(KeepSearchingForTargets(targetScanDelay));
    }
    public void OnDrawGizmos()
    {
        Gizmos.matrix = Matrix4x4.identity;
        // draw colour by state
        Vector3 focusPos = eye.transform.position + eye.transform.forward * visionRadius;
        focusPos.y = eye.transform.position.y;
        if (stateMachine != null)
        {
            if (stateMachine.GetState() == EnemyState.TargetVisible)
            {
                Gizmos.color = Color.red;
            }
            else if (stateMachine.GetState() == EnemyState.Alerted)
            {
                Gizmos.color = Color.yellow;
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
    
    IEnumerator KeepSearchingForTargets(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            LookForTargets();
        }
    }

    private void LookForTargets()
    {
        // 1. any objects within our vision radius
        bool canSeeTarget = false;
        Collider[] targets = Physics.OverlapSphere(eye.transform.position,
        visionRadius, targetLayer);
        
        bool isOverlappingWall = false;

        for (int i = 0; i < targets.Length; i++)
        {
            
            Vector3 targetPos = targets[i].transform.position;
            targetPos.y += 1.0f;
            Vector3 targetDirection = (targetPos - eye.transform.position).normalized;

            // 2. is the object within fov?
            if (Vector3.Angle(eye.transform.forward, targetDirection) < fovAngle / 2)
            {
                // 3. do we have line of sight?
                float distance = Vector3.Distance(eye.transform.position,targetPos);
                
                if (!Physics.Raycast(eye.transform.position, targetDirection,
                distance, wallLayer) && !isOverlappingWall)
                {
                    canSeeTarget = true;
                    isAlerted = true;
                    // change to target visible state
                    stateMachine.SetTarget(targets[i].transform);
                    stateMachine.SetState(EnemyState.TargetVisible);

                    return;
                }
            }
        }
        if (!canSeeTarget && isAlerted)
        {
            stateMachine.SetState(EnemyState.Alerted);
            isAlerted = false;
        }
    }
}
