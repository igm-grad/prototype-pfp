using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using CompleteProject;

public class PlayerMelee : MonoBehaviour 
{
    private HashSet<Transform> trackedTransforms;
    public float maxProjectileDistance;
    public float projectileLaunchSpeed;
       public float AnimationDelay = 0.1f;

	// Use this for initialization
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

    Transform RayCastToScreenPosition(Vector3 screenPosition, LayerMask mask)
    {
        var ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
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
                    var transform = RayCastToScreenPosition(screenPosition, LayerMask.GetMask("Shootable"));
                    if (transform)
                    {
                        trackedTransforms.Add(transform);
                    }
                    break;
                }
            case 1:
                // Use to update visual if we want to provide feedback to where the grenade will land.
                break;
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
                    var transform = RayCastToScreenPosition(screenPosition, LayerMask.GetMask("Shootable"));
                    if (transform)
                    {
                        trackedTransforms.Add(transform);
                    }
                    break;
                }
            case 1:
                    // Use to update visual if we want to provide feedback to where the grenade will land.
                    break;
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
                    var transform = RayCastToScreenPosition(screenPosition, LayerMask.GetMask("Shootable"));
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
                    var ray = Camera.main.ScreenPointToRay(screenPosition);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
                    {
                        Vector3 projectileDirection = (hit.point - transform.position);
                        projectileDirection.y = transform.position.y;

                        if (projectileDirection.magnitude > maxProjectileDistance)
                        {
                            projectileDirection = projectileDirection.normalized * maxProjectileDistance;
                        }

                        // Call Attack
                        LaunchProjectile(projectileDirection);
                    }
                    break;
                }
            default:
                
                    break;
                
        }
        

    }

    void LaunchProjectile(Vector3 vector)
    {
        var height = 5.0f;
        var distance = vector.magnitude;
        var launchAngle = Mathf.Atan((height * 4) / distance);
        var v0 = Mathf.Sqrt((distance * 9.78f) / Mathf.Sin(2 * launchAngle));
        Debug.Log("MAG1: " + distance + "\n");
        Debug.Log("DENOM: " + Mathf.Sin(2 * launchAngle) + "\n");
        Debug.Log(" V: " + v0 + "\n");


        launchAngle = -launchAngle * Mathf.Rad2Deg;
        Debug.Log("LAUNCH ANGLE: " + launchAngle + "\n");

        vector = (Quaternion.Euler(launchAngle, 0.0f, 0.0f) * vector);
        Debug.Log("DIR: " + vector + "\n");

        var projectile = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        projectile.transform.position = transform.position + transform.forward * 2 + Vector3.up;
        projectile.AddComponent<Rigidbody>();
       // projectile.AddComponent<Projectile>();
       // projectile.GetComponent<Rigidbody>().useGravity = false;
        projectile.GetComponent<Rigidbody>().velocity = vector.normalized * v0;
    }


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
