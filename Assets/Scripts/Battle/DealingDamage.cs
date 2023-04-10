using UnityEngine;
using System.Collections;

public class DealingDamage : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float attackRange = 1.0f;
    [SerializeField] private Transform upAttackPoint;
    [SerializeField] private Transform downAttackPoint;
    [SerializeField] private Transform leftAttackPoint;
    [SerializeField] private Transform rightAttackPoint;
    [SerializeField] private LayerMask enemyLayers;

    // boolean that enables damage cool down if true
    private bool justTookDamage = false;

    private void Start()
    {
        // begin damage cool down
        StartCoroutine(ResetDamageCoolDown());
    }

    /*
     * Function makes it so player is unable to be attacked 
     * for a certain amount of time.
     */
    IEnumerator ResetDamageCoolDown()
    {

        // damage cool down for 1 seconds
        yield return new WaitForSeconds(1);

        // now damage can be dealt again
        justTookDamage = false;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // if an enemy collider with player who is alive
        if (collision.CompareTag("Enemy") && !player.isDead)
        {

            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            // if attack can do damage, if enemy is within range, if player damage cool down done, and if enemy is alive
            if (player.GetDefense() < enemy.attack && !justTookDamage && !enemy.isDead)
            {
                //Debug.Log("Test");
                player.TakeMeleeDamage(enemy);
                justTookDamage = true;
            }
            else
            {
                return;
            }

            if (justTookDamage)
            {
                // do damage cool down
                StartCoroutine(ResetDamageCoolDown());
            }

            if (player.GetCurrentHealth() <= 0)
            {
                player.isDead = true;
                Debug.Log("Game Over: Player is Dead");
            }
        }

    }

    //private void Attack()
    //{
    //    Collider2D[] hitEnemies = null;

    //    if (player.isFacingUp)
    //    {
    //        hitEnemies = Physics2D.OverlapCircleAll(upAttackPoint.position, attackRange, enemyLayers);
    //    }

    //    if (player.isFacingDown)
    //    {
    //        hitEnemies = Physics2D.OverlapCircleAll(downAttackPoint.position, attackRange, enemyLayers);
    //    }

    //    if (player.isFacingLeft)
    //    {
    //        hitEnemies = Physics2D.OverlapCircleAll(leftAttackPoint.position, attackRange, enemyLayers);
    //    }

    //    if (player.isFacingRight)
    //    {
    //        hitEnemies = Physics2D.OverlapCircleAll(rightAttackPoint.position, attackRange, enemyLayers);
    //    }

        
    //    if(hitEnemies == null)
    //    {
    //        return;
    //    }

    //    foreach(Collider2D enemy in hitEnemies)
    //    {
    //        if(enemy.GetComponent<Enemy>() != null)
    //        {
    //            // enemy takes damage from player
    //            enemy.GetComponent<Enemy>().TakeHit(player);
    //        }
    //    }

    //    // reset cool down since player has attacked
    //    justTookDamage = false;
    //}

    //private void Update()
    //{
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        Attack();
    //    }
    //}

    // show attacking points in all directions
    private void OnDrawGizmosSelected()
    {
        if(upAttackPoint == null || downAttackPoint == null || 
            leftAttackPoint == null || rightAttackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(upAttackPoint.position, attackRange);
        Gizmos.DrawWireSphere(downAttackPoint.position, attackRange);
        Gizmos.DrawWireSphere(leftAttackPoint.position, attackRange);
        Gizmos.DrawWireSphere(rightAttackPoint.position, attackRange);
    }
}
