using UnityEngine;
using System.Collections;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] private Transform startPoint = null;
    [SerializeField] private Transform endPoint = null;
    [SerializeField] private float speed = 3f;
    [SerializeField] private bool isMovingUpAndDown = false;
    [SerializeField] private float pauseTime = 5f;
    private bool movingUp = false;
    private bool movingForward = false;
    private Vector3 velocity;
    private bool movementIsPaused = false;

    void FixedUpdate()
    {
        if (!movementIsPaused)
        {
            if (isMovingUpAndDown)
            {
                MoveUpAndDown();
            }
            else
            {
                MoveForwardAndBackward();
            }
        }
    }

    private void MoveUpAndDown()
    {
        

        if (transform.position.y >= startPoint.position.y)
        {
            movingUp = false;
        }
        if(transform.position.y <= endPoint.position.y)
        { 
            movingUp = true;
        }

        if (movingUp)
        {
            // go up
            velocity = Vector3.up * speed;
            transform.Translate(velocity * Time.deltaTime);
        }
        else
        {
            // go down
            velocity = -1 * Vector3.up * speed;
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    private void MoveForwardAndBackward()
    {

        if (transform.localPosition.z >= startPoint.localPosition.z)
        {
            movingForward = false;
        }
        if (transform.localPosition.z <= endPoint.localPosition.z)
        {
            movingForward = true;
        }

        if (movingForward)
        {
            // go forward
            velocity = (Vector3.forward * speed);
            transform.Translate(velocity * Time.deltaTime);
        }
        else
        {
            // go backward
            velocity = -1 * (Vector3.forward * speed);
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // make whatever lands on platform stay on platform while it is moving
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered");
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // make whatever lands on platform stay on platform while it is moving
        if (other.CompareTag("Player"))
        {
            Debug.Log("Staying");
            other.transform.SetParent(transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // make whatever leaves platform gets off
        if (other.CompareTag("Player"))
        {
            Debug.Log("Exited");
            other.transform.SetParent(null);
        }
    }

    public IEnumerator PauseMovement()
    {
        movementIsPaused = true;
        yield return new WaitForSeconds(pauseTime);
        movementIsPaused = false;
    }

    public void SetMovementPaused(bool isPaused)
    {
        movementIsPaused = isPaused;
    }

    public bool GetMovementPaused()
    {
        return movementIsPaused;
    }
}
