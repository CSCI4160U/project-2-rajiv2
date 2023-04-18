using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCAIStateMachine))]
public class NPC : MonoBehaviour
{
    public string npcName;
    public int attack;
    public int defense;
    public int maxHealth = 50;
    public int health;
    public bool isDead = false;
    public bool isShielded;
    public int value;
    public bool isChatter;
    public bool isWalker;

    [SerializeField] private Animator animator;
    private NPCAIStateMachine stateMachine;

    private void Awake()
    {
        stateMachine = GetComponent<NPCAIStateMachine>();
        ResetNPC();
    }

    /*
     * Function takes away health of enemy based on its
     * defense and the given player's attack
     */
    public void TakeMeleeDamage(Player player)
    {
        if (!isDead)
        {

            int damage = (player.GetAttackPower() - this.defense);
            if (damage > 0 && !isShielded)
            {
                this.health -= damage;
                Debug.Log(player.userName + " has dealt " + damage + " damage to " + npcName);

                // take damage animation
                animator.SetTrigger("tookDamage");
            }


            if (health <= 0)
            {
                Die();

                // decrease player score
                player.playerScore -= value;
            }

            stateMachine.SetThreat(player.transform);
            stateMachine.SetState(NPCState.InDanger);
        }
    }

    public void TakeGunDamage(Player player)
    {
        if (!isDead)
        {
            // if shield is enabled
            if (isShielded)
            {
                // display force field
            }
            // if not shielded
            else
            {
                // if enemy hasn't seen player, now they see them once they are hit
                stateMachine.SetThreat(player.transform);
                stateMachine.SetState(NPCState.InDanger);

                int damage = (player.gun.power - this.defense);
                if (damage > 0)
                {
                    this.health -= damage;
                    Debug.Log(player.userName + " has dealt " + damage + " damage to " + npcName);

                    // take damage animation
                    //animator.SetTrigger("tookDamage");
                }


                if (health <= 0)
                {
                    Die();

                    // decrease player score
                    player.playerScore -= value;
                }
            }

        }
    }

    /*
     * Function does death animation disables colliders
     * and disables this component when the enemy is dead
     */
    private void Die()
    {
        //stateMachine.SetState(NPCState.Dead);
        isDead = true;

        Debug.Log(npcName + " has died!");

        // disable Player from being able to hit enemy
        this.GetComponent<Collider>().enabled = false;

        // disable element
        this.enabled = false;

    }

    private void ResetNPC()
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
}
