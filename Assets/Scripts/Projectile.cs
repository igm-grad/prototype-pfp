using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {


    public void OnCollisionEnter(Collision collision)
    {
        GameObject.Destroy(this.rigidbody);
    }
}
