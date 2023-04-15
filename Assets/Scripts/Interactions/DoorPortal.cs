using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorPortal : MonoBehaviour
{
    [SerializeField] private string destination;

    // stores position of player when exiting doorway
    [SerializeField] private PlayerTransform playerTransformInDestination;

    // stores initial position of player when entering doorway
    [SerializeField] private PlayerTransform playerTransformInCurrentScene;

    [SerializeField] private int scoreRequired = 0;
    [SerializeField] private float requiredDistance = 5f;
    [SerializeField] private bool isPortal = false;
    [SerializeField] private bool isOpen = false;
    [SerializeField] private GameObject hint = null;
    [SerializeField] private Animator doorAnimator = null;

    public static bool enteredDoorPortal = false;
    public bool isOneWay = false;
    
    private bool canStillOpen = true;

    private Player player = null;

    private void Update()
    {
        player = FirstPersonController._instance.GetComponent<Player>();
        ShowHint();
    }

    public void SetDoorOpen(bool state)
    {
        if (doorAnimator != null)
        {
            isOpen = state;
            Vector3 doorTransformDirection = transform.TransformDirection(Vector3.forward);
            Vector3 playerTransformDirection = player.transform.position - transform.position;
            float dot = Vector3.Dot(doorTransformDirection, playerTransformDirection);

            // direction doot swings
            doorAnimator.SetFloat("dot", dot);

            doorAnimator.SetBool("isOpen", isOpen);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canStillOpen && !isOpen)
            {
                SetDoorOpen(true);
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // if door is open
            if (isOpen)
            {
                // if door is a one way, can't open it again
                if (isOneWay && canStillOpen)
                {
                    canStillOpen = false;
                    SetDoorOpen(false);
                }
                else
                {
                    // if is a normal door, close after player exits collider
                    SetDoorOpen(false);
                }

                // after going through door, go through portal
                if (isPortal && PlayerHasReachedGoal())
                {
                    GoThroughDoorPortal();
                }
            }

            
            
        }
    }

    /*
     * Function loads destination Scene of the DoorWay
     * and Saves the current state of the game after
     * entering the DoorWay
     */
    private void GoThroughDoorPortal()
    {
        // You met the requirement
        Debug.Log("Player has met the requirement to go through portal.");

        playerTransformInCurrentScene.initialPosition = playerTransformInDestination.initialPosition;
        playerTransformInCurrentScene.initialRotation = playerTransformInDestination.initialRotation;
        playerTransformInCurrentScene.initialSize = playerTransformInDestination.initialSize;

        SceneManager.LoadScene(destination);

        enteredDoorPortal = true;
        SaveManager._instance.SaveGame();
    }

    private void ShowHint()
    {
        if (player != null)
        {

            // get distance between player and goal (has to b within 5)
            float distanceBetween = Vector3.Distance(this.transform.position, player.transform.position);

            // if player is within the required distance and has pressed attack button
            if (distanceBetween <= requiredDistance)
            {
                // show hint on how to open door
                if (hint != null)
                {
                    hint.SetActive(true);
                }
            }
            else
            {
                // hide hint on how to open door
                if (hint != null)
                {
                    hint.SetActive(false);
                }
            }

        }
    }

    /*
     * Function checks the requirements for a player to enter a DoorWay
     */
    private bool PlayerHasReachedGoal()
    {

        if (player != null)
        {

            // if player has reached score required
            if (player.playerScore >= scoreRequired)
            {
                return true;
            }
            else
            {
                Debug.Log("You need a score of " + scoreRequired + " to progress. CURRENT SCORE: " +
                    player.playerScore + "/" + scoreRequired + ".");

                // show message in console for 10 seconds
                HUDConsole._instance.Log("You need a score of " + scoreRequired + " to progress.\n CURRENT SCORE: " +
                    player.playerScore + "/" + scoreRequired + ".", 10f);
            }

        }
        return false;
    }
}
