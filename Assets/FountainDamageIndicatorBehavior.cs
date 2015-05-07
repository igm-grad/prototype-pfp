using UnityEngine;
using System.Collections;

public class FountainDamageIndicatorBehavior : MonoBehaviour {

    public float VanishingDistance;
    private GameObject Fountain;
    private GameObject Player;
    // Use this for initialization
	void Start () {
        Fountain = GameObject.FindGameObjectWithTag("target");
        Player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (Vector3.Distance(Player.transform.position, Fountain.transform.position) < VanishingDistance)
        {
           // renderer.material.color.a = 0.0f;
        }
        else
        {
           // renderer.material.color.a = 1.0f;
        }

        transform.position = (Fountain.transform.position + Player.transform.position) * 4 / 5;
        transform.LookAt(Fountain.transform);
	}
}
