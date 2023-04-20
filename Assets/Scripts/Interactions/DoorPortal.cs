using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    [SerializeField] private Canvas hintCanvas = null;
    private TextMeshProUGUI hintText;
    [SerializeField] private Animator doorAnimator = null;
    [SerializeField] private PatternGrid patternGrid = null;
    [SerializeField] private bool requiresPatternGridToUnlock;

    public static bool enteredDoorPortal = false;
    public bool isOneWay = false;
    
    private bool canStillOpen = true;

    private Player player = null;
    private Enemy enemy = null;
    private bool isPlayer = false;

    private void Awake()
    {
        if (hintCanvas != null)
        {
            hintText = hintCanvas.GetComponentInChildren<TextMeshProUGUI>();
            hintCanvas.gameObject.SetActive(false);
        }
        if (patternGrid != null)
        {
            requiresPatternGridToUnlock = true;
            hintText.text = "Follow the patterns to enter...";
        }
        else
        {
            requiresPatternGridToUnlock = false;
            hintText.text = "More is required to enter...";
        }

        
    }

    private void Update()
    {
        player = FirstPersonController._instance.GetComponent<Player>();
        ShowHint();
    }

    public void SetDoorOpen(bool state, bool isPlayer)
    {
        if (doorAnimator != null)
        {
            isOpen = state;
            Vector3 doorTransformDirection = transform.TransformDirection(Vector3.forward);
            
            
            float dot = 0f;
            if (isPlayer)
            {
                Vector3 playerTransformDirection = player.transform.position - transform.position;
                dot = Vector3.Dot(doorTransformDirection, playerTransformDirection);
            }
            else
            {
                Vector3 enemyTransformDirection = enemy.transform.position - transform.position;
                dot = Vector3.Dot(doorTransformDirection, enemyTransformDirection);
            }
            

            // direction doot swings
            doorAnimator.SetFloat("dot", dot);

            doorAnimator.SetBool("isOpen", isOpen);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayer = true;
            if (canStillOpen && !isOpen && PlayerHasReachedGoal())
            {
                SetDoorOpen(true,isPlayer);
            }
            
        }
        else if (other.CompareTag("Enemy"))
        {
            isPlayer = false;
            enemy = other.GetComponent<Enemy>();
            if (canStillOpen && !isPortal)
            {  
                SetDoorOpen(true, isPlayer);
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
                isPlayer = true;
                // if door is a one way, can't open it again
                if (isOneWay && canStillOpen)
                {
                    
                    canStillOpen = false;
                    SetDoorOpen(false, isPlayer);
                }
                else
                {
                    // if is a normal door, close after player exits collider
                    SetDoorOpen(false, isPlayer);
                }

                // after going through door, go through portal
                if (isPortal && PlayerHasReachedGoal())
                {
                    GoThroughDoorPortal();
                }
            }
        }
        else if (other.CompareTag("Enemy"))
        {
            isPlayer = false;
            enemy = other.GetComponent<Enemy>();
            if (!isOneWay)
            {
                SetDoorOpen(false, isPlayer);
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
                if (hintCanvas != null && !PlayerHasReachedGoal())
                {
                    hintCanvas.gameObject.SetActive(true);
                }
            }
            else
            {
                // hide hint on how to open door
                if (hintCanvas != null)
                {
                    hintCanvas.gameObject.SetActive(false);
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
            if (requiresPatternGridToUnlock)
            {
                if (!patternGrid.isComplete)
                {
                    hintText.text = "Follow the patterns to enter...";
                    
                    return false;
                }
                else
                {
                    requiresPatternGridToUnlock = false;
                }
            }

            // if player has reached score required
            if (player.playerScore >= scoreRequired)
            {
                return true;
            }
            else
            {
                hintText.text = "You need a score of " + scoreRequired + " to progress. CURRENT SCORE: " +
                    player.playerScore + "/" + scoreRequired + ".";
                Debug.Log("You need a score of " + scoreRequired + " to progress. CURRENT SCORE: " +
                    player.playerScore + "/" + scoreRequired + ".");
            }

        }
        return false;
    }
}
