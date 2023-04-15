using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

[RequireComponent(typeof(EnemyAIStateMachine))]
public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int attack;
    public int shootingDamage;
    public int defense;
    public int maxHealth = 50;
    public int health;
    public bool isDead = false;
    public float reviveTime;
    public int numberOfLives;
    public bool isShielded;
    public int value;
    public bool isShooter;
    public bool isBrawler;
    
    [SerializeField] private Animator animator;
    private EnemyAIStateMachine stateMachine;
    

    private void Awake()
    {
        stateMachine = GetComponent<EnemyAIStateMachine>();
        ResetEnemy();
    }

    /*
     * Function takes away health of enemy based on its
     * defense and the given player's attack
     */
    public void TakeMeleeDamage(Player player)
    {
        if (!isDead && !isShielded)
        {

            int damage = (player.GetAttackPower() - this.defense);
            if (damage > 0)
            {
                this.health -= damage;
                Debug.Log(player.userName + " has dealt " + damage + " damage to " + enemyName);

                // show message in console for 3 seconds
                HUDConsole._instance.Log(player.userName + " has dealt " + damage + " damage to " + enemyName, 3f);

                // take damage animation
                animator.SetTrigger("tookDamage");
            }


            if (health <= 0)
            {
                Die();

                numberOfLives--;

                if (RanOutOfLives())
                {
                    // add to bosses defeated, with its corresponding scene
                    player.bossesDefeatedNames.Add(gameObject.name);
                    player.bossesDefeatedScenes.Add(SceneManager.GetActiveScene().name);
                }
                else
                {
                    StartCoroutine(ReviveCoolDown());
                }

                // increase player score
                player.playerScore += value;
            }

            stateMachine.SetTarget(player.transform);
            stateMachine.SetState(EnemyState.TargetVisible);
        }  
    }

    public void TakeGunDamage(Player player)
    {
        if (!isDead)
        {
            // if shield is enabled
            if(isShielded)
            {
                // display force field
            }
            // if not shielded
            else
            {
                // if enemy hasn't seen player, now they see them once they are hit
                stateMachine.SetTarget(player.transform);
                stateMachine.SetState(EnemyState.TargetVisible);

                int damage = (player.gun.power - this.defense);
                if (damage > 0)
                {
                    this.health -= damage;
                    Debug.Log(player.userName + " has dealt " + damage + " damage to " + enemyName);

                    // show message in console for 3 seconds
                    HUDConsole._instance.Log(player.userName + " has dealt " + damage + " damage to " + enemyName, 3f);

                    // take damage animation
                    //animator.SetTrigger("tookDamage");
                }


                if (health <= 0)
                {
                    Die();

                    numberOfLives--;

                    if (RanOutOfLives())
                    {
                        // add to bosses defeated, with its corresponding scene
                        player.bossesDefeatedNames.Add(gameObject.name);
                        player.bossesDefeatedScenes.Add(SceneManager.GetActiveScene().name);
                    }
                    else
                    {
                        StartCoroutine(ReviveCoolDown());
                    }

                    // increase player score
                    player.playerScore += value;
                }
            }
            
        }
    }

    IEnumerator ReviveCoolDown()
    {

        // wait until enemy revive time expires, then revive
        yield return new WaitForSeconds(reviveTime);

        Revive();
    }

    /*
     * Function disables the game object of an enemy is it
     * ran out of lives. It also returns whether or not the
     * enemy has ran out of lives
     */
    public bool RanOutOfLives()
    {
        if(numberOfLives == 0)
        {
            // hide game object
            // do Coroutine to despawn
            //this.gameObject.SetActive(false);

            return true;
        }
        return false;
    }

    /*
     * Function does death animation disables colliders
     * and disables this component when the enemy is dead
     */
    private void Die()
    {
        stateMachine.SetState(EnemyState.Dead);
        isDead = true;

        Debug.Log(enemyName + " has been defeated!");

        // show message in console for 3 seconds
        HUDConsole._instance.Log(enemyName + " has been defeated!", 3f);

        // disable Player from being able to hit enemy
        this.GetComponent<Collider>().enabled = false;

        // disable element
        this.enabled = false;
        
    }

    /*
     * Function re enables colliders and component of enemy
     * and resets health and brings enemy back to life
     */
    public void Revive()
    {
        //animator.ResetTrigger("tookDamage");
        stateMachine.SetState(EnemyState.Reviving);

        ResetEnemy();
    }

    private void ResetEnemy()
    {
        // enable Player from being able to hit enemy
        this.GetComponent<Collider>().enabled = true;

        // enable element
        this.enabled = true;

        // reset health
        health = maxHealth;

        // now damage can be dealt again
        isDead = false;
    }

    private void Update()
    {
        if(stateMachine.GetState() == EnemyState.Reviving)
        {
            isShielded = true;
        }
        else 
        { 
            isShielded = false; 
        }

        RanOutOfLives();
    }
}
