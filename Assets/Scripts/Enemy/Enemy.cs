using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int attack;
    public int defense;
    public int maxHealth = 50;
    public int health;
    public bool isDead = false;
    public float reviveTime;
    public int numberOfLives;
    public int value;

    private void Awake()
    {
        Revive();
    }
    
    /*
     * Function takes away health of enemy based on its
     * defense and the given player's attack
     */
    public void TakeHit(Player player)
    {
        if (!isDead)
        {
            int damage = (player.GetAttackPower() - this.defense);
            if (damage > 0)
            {
                this.health -= damage;
                Debug.Log(player.userName + " has dealt " + damage + " damage to " + enemyName);

                // show message in console for 3 seconds
                HUDConsole._instance.Log(player.userName + " has dealt " + damage + " damage to " + enemyName, 3f);

                // take damage animation
                this.GetComponent<Animator>().SetTrigger("tookDamage");
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
            this.gameObject.SetActive(false);

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
        isDead = true;

        Debug.Log(enemyName + " has been defeated!");

        // show message in console for 3 seconds
        HUDConsole._instance.Log(enemyName + " has been defeated!", 3f);

        // do death animation
        this.GetComponent<Animator>().SetTrigger("isDead");

        // disable Player from being able to hit enemy
        this.GetComponent<Collider2D>().enabled = false;

        // disable element
        this.enabled = false;
        
    }

    public void DoWalkAnimation()
    {
        this.GetComponent<Animator>().SetTrigger("isWalking");
    }

    public void StopWalkAnimation()
    {
        this.GetComponent<Animator>().ResetTrigger("isWalking");
    }

    public void DoAttackAnimation()
    {
        // do death animation
        this.GetComponent<Animator>().SetTrigger("attacked");
    }

    /*
     * Function re enables colliders and component of enemy
     * and resets health and brings enemy back to life
     */
    public void Revive()
    {

        // make enemy alive
        this.GetComponent<Animator>().ResetTrigger("isDead");

        // enable Player from being able to hit enemy
        this.GetComponent<Collider2D>().enabled = true;

        // enable element
        this.enabled = true;

        // reset health
        health = maxHealth;

        // now damage can be dealt again
        isDead = false;
    }

    private void Update()
    {
        RanOutOfLives();
    }
}
