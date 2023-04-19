using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadlyCollisions : MonoBehaviour
{
    [SerializeField] private Player player = null;
    [SerializeField] private Enemy enemy = null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            if(player != null)
            {
                player.Die();
            }

            if (enemy != null)
            {
                enemy.Die();
            }

        }
        if (other.CompareTag("Hazard"))
        {
            Hazard hazard = other.GetComponent<Hazard>();

            if (player != null)
            {
                player.TakeHazardDamage(hazard);
            }

            if (enemy != null)
            {
                enemy.TakeHazardDamage(hazard);
            }
        }
    }
}
