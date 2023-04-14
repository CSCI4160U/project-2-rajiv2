using UnityEngine;
using System.Collections;

public class RotatingPlatform : MonoBehaviour
{
    // by default, rotate around x axis
    [SerializeField] private Vector3 axis = new Vector3(0.0f,0f,1f);
    [SerializeField] private float speed = 20f;
    [SerializeField] private float pauseTime = 5f;
    private bool movementIsPaused = false;

    void FixedUpdate()
    {
        if (!movementIsPaused)
        {
            Rotate();
        }
        
    }

    private void Rotate()
    {
        transform.Rotate(axis, speed * Time.deltaTime);
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

    //private void OnTriggerEnter(Collider other)
    //{
    //    // make whatever lands on platform stay on platform while it is moving
    //    if (other.CompareTag("Player"))
    //    {
    //        other.transform.SetParent(transform);
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    // make whatever leaves platform gets off
    //    if (other.CompareTag("Player"))
    //    {
    //        other.transform.SetParent(null);
    //    }
    //}
}