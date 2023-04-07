using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyHealthBar : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = this.GetComponent<Enemy>();
    }

    private void Update()
    {
        if (enemy.isDead && enemy.RanOutOfLives())
        {
            EnemyHealthBarManager._instance.HideEnemyHeathBar();
        }
    }

    private void OnMouseEnter()
    {
        EnemyHealthBarManager._instance.SetAndShowEnemyHealthBar(enemy);
    }

    private void OnMouseExit()
    {
        EnemyHealthBarManager._instance.HideEnemyHeathBar();
    }
}
