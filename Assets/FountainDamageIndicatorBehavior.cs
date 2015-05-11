using UnityEngine;
using System.Collections;

public class FountainDamageIndicatorBehavior : MonoBehaviour {

    public float AlphaDelay;
    private GameObject Fountain;
    private GameObject Player;
    private float timeElapsed;

    // Use this for initialization
	void Start () {
        Fountain = GameObject.FindGameObjectWithTag("target");
        Player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
        if (timeElapsed <= AlphaDelay)
        {
            var renderer = gameObject.GetComponentInChildren<Renderer>();
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1.0f - (timeElapsed / AlphaDelay));
            timeElapsed += Time.deltaTime;
        }

        transform.position = (Fountain.transform.position + Player.transform.position) * 4 / 5;
        transform.LookAt(Fountain.transform);
	}

    public void ResetAlpha()
    {
        var renderer = gameObject.GetComponentInChildren<Renderer>();
        timeElapsed = 0.0f;
        renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 1.0f);
    }
}
