using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;


public class Player : MonoBehaviour
{
    // Values are increased In-Game if weapons or defense boosts are equipped
    private int currentAttack;

    // Gun
    public Gun gun;

    private int currentDefense;
    private int health;
    public string userName;
    public int defaultAttack = 25;
    public int defaultDefense = 10;
    public int maxHealth = 100;
    public int playerScore;
    public bool isDead;
    

    public PlayerTransform startingTransform;

    // stores Enemy names
    public List<string> bossesDefeatedNames;

    // store Scene names where Enemy was defeated
    public List<string> bossesDefeatedScenes;

    [SerializeField] private AudioSource getHurtSoundEffect;
    [SerializeField] private AudioSource takeMeleeDamageSoundEffect;
    [SerializeField] private AudioSource getShotSoundEffect;

    public void Awake()
    {
        // initialize player variables
        currentAttack = defaultAttack;
        currentDefense = defaultDefense;
        health = maxHealth;
        isDead = false;
        bossesDefeatedNames = new List<string>();
        bossesDefeatedScenes = new List<string>();
    }

    public void TakeMeleeDamage(Enemy enemy)
    {
        int damage = (enemy.attack - this.currentDefense);

        if(damage > 0)
        {
            health -= damage;

            // enemy attack animation
            //enemy.GetComponent<Animator>().SetTrigger("attacked");

            Debug.Log(enemy.enemyName + " has dealt " + damage + " damage to " + userName);

            // show message in console for 3 seconds
            HUDConsole._instance.Log(enemy.enemyName + " has dealt " + damage + " damage to " + userName, 3f);

            //getHurtSoundEffect.Play();
            takeMeleeDamageSoundEffect.Play();
        }

        CheckHealth();
    }

    public void TakeGunDamage(Enemy enemy)
    {
        int damage = (enemy.shootingDamage - this.currentDefense);

        if (damage > 0)
        {
            health -= damage;

            // enemy attack animation
            enemy.GetComponent<Animator>().SetTrigger("attacked");

            Debug.Log(enemy.enemyName + " has shot " + userName);

            // show message in console for 3 seconds
            HUDConsole._instance.Log(enemy.enemyName + " has shot " + userName, 3f);

            //getHurtSoundEffect.Play();
            //getShotSoundEffect.Play();
        }

        CheckHealth();
    }

    private void CheckHealth()
    {
        if (health <= 0)
        {
            isDead = true;
        }
    }

    public void UpdateHealthSlider()
    {
        GameObject healthSlider = GameObject.Find("HealthSlider");

        if (healthSlider != null)
        {
            healthSlider.GetComponent<Slider>().maxValue = maxHealth;
            healthSlider.GetComponent<Slider>().value = health;
        }
    }

    public void UpdatePlayerScore()
    {
        GameObject scoreNumber = GameObject.Find("ScoreNumber");

        if (scoreNumber != null)
        {
            scoreNumber.GetComponent<TMP_Text>().text = playerScore.ToString();
        }
        
    }

    private void UpdateGunInfo()
    {
        GameObject gunHold = this.GetComponentInChildren<Camera>().GetComponentInChildren<Transform>().gameObject;
        if (gunHold != null)
        {
            if (gunHold.transform.childCount > 0)
            {
                Gun gunEquipped = gunHold.GetComponentInChildren<Gun>();

                if (gunEquipped != null)
                {
                    gun = gunEquipped;
                    return;
                }
            }
        }

        gun = null;
    }

    private void Update()
    {
        UpdateHealthSlider();
        UpdatePlayerScore();
        UpdateGunInfo();
    }

    // Getters and Setters

    public int GetAttackPower()
    {
        return currentAttack;
    }

    public bool HasGun()
    {
        return gun != null;
    }
    public void SetAttackPower(int newAttack)
    {
        currentAttack = newAttack;
    }

    public int GetDefense()
    {
        return currentDefense;
    }

    public void SetDefense(int newDefense)
    {
        currentDefense = newDefense;
    }

    public int GetCurrentHealth()
    {
        return health;
    }

    public void Heal(int healthBoost)
    {
        health += healthBoost;

        if(health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
    }
}
