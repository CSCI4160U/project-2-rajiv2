using UnityEngine;
public class MeleeWeaponDamage : MonoBehaviour
{
    [SerializeField] private GameObject user = null;

    private Player player;
    private Enemy enemy;

     private void Update()
    {
        CheckUser();
    }

    private void CheckUser()
    {
        if(user != null)
        {
            if (user.GetComponent<Player>() != null)
            {
                user.GetComponent<Player>();
            }
            else if (user.GetComponent<Enemy>() != null)
            {
                enemy = user.GetComponent<Enemy>();
            }
        }
        
    }
    public void OnTriggerEnter(Collider other)
    {
        // if Enemy is dealing attack
        if (enemy != null)
        {
            if (other.CompareTag("Player"))
            {
                Debug.Log("Melee Damage on " + other.gameObject.name);
                Player playerTakingDamage = other.GetComponent<Player>();
                if (playerTakingDamage != null)
                {
                    playerTakingDamage.TakeMeleeDamage(enemy);
                }
            }
        }
        // if Player is dealing attack
        if (player != null)
        {
            Enemy enemyTakingDamage = other.GetComponent<Enemy>();
            if (enemyTakingDamage != null)
            {
                Debug.Log("Melee Damage on " + other.gameObject.name);
                enemyTakingDamage.TakeMeleeDamage(player);
            }
        }
        
    }
}
