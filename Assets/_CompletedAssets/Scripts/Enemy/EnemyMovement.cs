using UnityEngine;
using System.Collections;

namespace CompleteProject
{
    public class EnemyMovement : MonoBehaviour
    {
        Transform target;
        Transform fountain;
        Transform player;          // Reference to the player's position.
        PlayerHealth playerHealth;      // Reference to the player's health.
        EnemyHealth enemyHealth;        // Reference to this enemy's health.
        NavMeshAgent nav;               // Reference to the nav mesh agent.


        void Awake ()
        {
            // Set up the references.
			fountain = GameObject.FindGameObjectWithTag("target").transform;
            player = GameObject.FindGameObjectWithTag ("Player").transform;
            target = fountain;

            playerHealth = player.GetComponent <PlayerHealth> ();
            enemyHealth = GetComponent <EnemyHealth> ();
            nav = GetComponent <NavMeshAgent> ();
        }


        public void SetAgro(Transform target)
        {
            var health = GetComponent<EnemyHealth>();
            if (health.currentHealth <= 0)
            {
                return;
            }

            if (target == null)
            {
                this.target = fountain;
                nav.SetLayerCost(4, 10);
            }
            else
            {
                this.target = target;
                nav.SetLayerCost(4, 1);
            }
        }

        void Update ()
        {
            if (target == null)
            {
                SetAgro(null);
            }

            if (enemyHealth.currentHealth <= 0)
            {
                return;
            }

            nav.SetDestination(target.position);


            //// If the enemy and the player have health left...
            //if(enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0)
            //{
            //    // ... set the destination of the nav mesh agent to the player.
            //    nav.SetDestination (target.position);
            //}
            //// Otherwise...
            //else
            //{
            //    // ... disable the nav mesh agent.
            //    nav.enabled = false;
            //}
        }
    }
}