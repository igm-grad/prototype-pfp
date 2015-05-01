using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using CompleteProject;

public class PlayerMelee : MonoBehaviour 
{
    private HashSet<Transform> trackedTransforms;
    public ParticleSystem ninjaParticles;
    private TrailRenderer trail;
        
    public bool isAttacking { get; private set;}
    
    // Use this for initialization
	void Start ()
    {
        trackedTransforms = new HashSet<Transform>();
        trail = GetComponent<TrailRenderer>();
        trail.enabled = false;
    }
	
	// Update is called once per frame
	void Update () 
    {
        if (Input.GetMouseButtonDown(0))
        {
            MouseDownBegan(0, Input.mousePosition);
        }
        else if (Input.GetMouseButton(0))
        {
            MouseDownDidChange(0, Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            MouseDownDidEnd(0, Input.mousePosition);
        }

        if (Input.GetMouseButtonDown(1))
        {
            MouseDownBegan(1, Input.mousePosition);
        }
        else if (Input.GetMouseButton(1))
        {
            MouseDownDidChange(1, Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            MouseDownDidEnd(1, Input.mousePosition);
        }
	}

    Transform RayCastToScreenPosition(Vector3 screenPosition)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Shootable")))
        {
            return hit.collider.gameObject.GetComponent<Transform>();
        }
        else { return null; }
    }

    void MouseDownBegan(int mouseButton, Vector3 screenPosition)
    {
        if (isAttacking) return; 

        switch (mouseButton)
        {
            case 0:
                {
                    var transform = RayCastToScreenPosition(screenPosition);
                    if (transform)
                    {
                        trackedTransforms.Add(transform);
                    }
                    break;
                }
            case 1:
                {
                    break;
                }
            default:
                
                    break;
                
        }
        
    }

    void MouseDownDidChange(int mouseButton, Vector3 screenPosition)
    {
        if (isAttacking) return; 

        switch (mouseButton)
        {
            case 0:
                {
                    var transform = RayCastToScreenPosition(screenPosition);
                    if (transform)
                    {
                        trackedTransforms.Add(transform);
                    }
                    break;
                }
            case 1:
                {
                    break;
                }
            default:
                
                    break;
                
        }
        
    }

    void MouseDownDidEnd(int mouseButton, Vector3 screenPosition)
    {
        if (isAttacking) return; 

        switch (mouseButton)
        {
            case 0:
                {
                    var transform = RayCastToScreenPosition(screenPosition);
                    if (transform)
                    {
                        trackedTransforms.Add(transform);
                    }

                    // Call Attack
                    StartCoroutine(Ninja());
                    break;
                }
            case 1:
                {
                    break;
                }
            default:
                
                    break;
                
        }
        

    }


    public float AnimationDelay = 0.1f;

    IEnumerator Ninja()
    {
        isAttacking = true;
        trail.enabled = true;
        foreach (Transform t in trackedTransforms)
        {
            // Check if target still alive
            if (!t) continue;
            
            //Move to target and animate
            Vector3 dir = Vector3.Normalize(t.transform.position - transform.position);
            if (ninjaParticles)
            {
                ninjaParticles.transform.localPosition = Vector3.zero;
                ninjaParticles.transform.LookAt(transform.position);
                ninjaParticles.Play();
            }

            StartCoroutine(MoveObject(this.transform, this.transform.position, t.transform.position + dir * 2.0f, AnimationDelay));
            transform.LookAt(t);
            
            // Animate Hammer

            // Try and find an EnemyHealth script on the gameobject hit.
            EnemyHealth enemyHealth = t.collider.GetComponent<EnemyHealth>();
            EnemyMovement enemyMovement = t.collider.GetComponent<EnemyMovement>();

            // If the EnemyHealth component exist...
            if (enemyHealth != null)
            {
                // ... the enemy should take damage.
                enemyHealth.TakeDamage(100000, t.position);
            }

            yield return new WaitForSeconds(AnimationDelay*1.2f);
        }

        trackedTransforms.Clear();

        isAttacking = false;

        yield return new WaitForSeconds(AnimationDelay*2);

        trail.enabled = false;
    }

    IEnumerator MoveObject(Transform thisTransform, Vector3 startPos, Vector3 endPos, float time)
    {
        float i = 0.0f;
        float rate = 1.0f / time;
        while (i < 1.0)
        {
            i += Time.deltaTime * rate;
            thisTransform.position = Vector3.Lerp(startPos, endPos, i);
            yield return 0;
        }
    }
}
