using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class PlayerMelee : MonoBehaviour 
{
    private HashSet<Transform> trackedTransforms;

	// Use this for initialization
	void Start ()
    {
        trackedTransforms = new HashSet<Transform>();
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
}
