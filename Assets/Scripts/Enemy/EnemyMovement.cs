using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{

    [SerializeField] private GameObject player; 
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float hostileRadius = 5f;
    private Enemy enemy;

    private float distanceFromPlayer;

    private void Start()
    {
        enemy = this.GetComponent<Enemy>();

        if(player == null)
        {
            Debug.Log("Please set up 'Player' value in EnemyMovement script! Game Object: " + this.gameObject.name);
        }
    }

    void Update()
    {
        if (!enemy.isDead)
        {
            Move();
        }        
    }

    /*
     * Function enables enemy to follow player around based on its
     * hostile radius
     */
    public void FollowPlayer()
    {

        // direction the enemy must move in ordeer to get to the player
        Vector2 direction = player.transform.position - transform.position;

        // as long as player is alive
        if (!player.GetComponent<Player>().isDead)
        {
            if (direction.x > 0)
            {
                transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }
            else
            {
                transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            }

            // move the enemy towards the player
            transform.position = Vector3.MoveTowards(this.transform.position, player.transform.position, movementSpeed * Time.deltaTime);

            // walk animation
            enemy.DoWalkAnimation();
        }
        else
        {
            enemy.StopWalkAnimation();
        }
    }

    /*
     * Function does all functionality relating to enemy movement
     * including following the player around, and doing attack and
     * walking animations.
     */
    public void Move()
    {
        if (player != null)
        {
            // get distance from player 
            distanceFromPlayer = Vector2.Distance(transform.position, player.transform.position);

            if(distanceFromPlayer < hostileRadius)
            {
                FollowPlayer();
            }
            else
            {
                // reset walking animation
                enemy.StopWalkAnimation();
            }

            if(distanceFromPlayer < 2)
            {
                enemy.DoAttackAnimation();
            }
        }
    }
}
