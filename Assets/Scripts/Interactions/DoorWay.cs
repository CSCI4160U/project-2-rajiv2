using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorWay : MonoBehaviour
{
    [SerializeField] private string destination;
    [SerializeField] private Player player = null;

    // stores position of player when exiting doorway
    [SerializeField] private PlayerTransform playerTransformInDestination;

    // stores initial position of player when entering doorway
    [SerializeField] private PlayerTransform playerTransformInCurrentScene;

    [SerializeField] private int scoreRequired = 0;
    [SerializeField] private float requiredDistance = 5f;

    public static bool enteredDoorWay = false;

    private void Update()
    {
        CheckIfPlayerHasReachedGoal();
    }

    /*
     * Function loads destination Scene of the DoorWay
     * and Saves the current state of the game after
     * entering the DoorWay
     */
    private void GoThroughDoorWay()
    {
        // You met the requirement
        Debug.Log("You have met the requirement to pass.");

        playerTransformInCurrentScene.initialPosition = playerTransformInDestination.initialPosition;
        playerTransformInCurrentScene.initialRotation = playerTransformInDestination.initialRotation;
        playerTransformInCurrentScene.initialSize = playerTransformInDestination.initialSize;

        SceneManager.LoadScene(destination);

        enteredDoorWay = true;
        player.GetComponent<SaveManager>().SaveGame();
    }

    /*
     * Function checks the requirements for a player to enter a DoorWay
     */
    private void CheckIfPlayerHasReachedGoal()
    {
        if (player != null)
        {

            // get distance between player and goal (has to b within 5)
            float distanceBetween = Vector3.Distance(this.transform.position, player.transform.position);

            // if player is within the required distance and has pressed attack button
            if (distanceBetween <= requiredDistance)
            {

                // show hint on how to open door
                transform.GetChild(0).gameObject.SetActive(true);

                // if player has reached score required
                if (player.playerScore >= scoreRequired)
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        Debug.Log("Went through a door way.");
                        GoThroughDoorWay();
                    }
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
            else
            {
                // hide hint on how to open door
                transform.GetChild(0).gameObject.SetActive(false);
            }

        }
    }
}
