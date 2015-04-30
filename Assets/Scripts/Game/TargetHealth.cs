using UnityEngine;
using System.Collections;

public class TargetHealth : MonoBehaviour {

    public GameObject explosionPrefab;

    void OnTriggerEnter(Collider other)
    {
        var gm = GameManager.Instance;
        var enemyHealth = other.GetComponent<CompleteProject.EnemyHealth>();
        if (enemyHealth == null || enemyHealth.isDead)
        {
            return;
        }

        Instantiate(explosionPrefab, transform.position, transform.rotation);
        enemyHealth.Kill();
        gm.DamageFountain();
    }
}
