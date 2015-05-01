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
        var transform = RayCastToScreenPosition(screenPosition);
        if (transform)
        {
            trackedTransforms.Add(transform);
        }
    }

    void MouseDownDidChange(int mouseButton, Vector3 screenPosition)
    {
        var transform = RayCastToScreenPosition(screenPosition);
        if (transform)
        {
            trackedTransforms.Add(transform);
        }
    }

    void MouseDownDidEnd(int mouseButton, Vector3 screenPosition)
    {
        var transform = RayCastToScreenPosition(screenPosition);
        if (transform)
        {
            trackedTransforms.Add(transform);
        }

        Debug.Log("TRACKED TRANSFORMS: " + trackedTransforms.Count() + "\n");
        // Call Attack

        //Empty Set
        trackedTransforms.Clear();
    }
}
