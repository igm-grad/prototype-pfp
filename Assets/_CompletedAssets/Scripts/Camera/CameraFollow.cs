using UnityEngine;
using System.Collections;

namespace CompleteProject
{
	public class CameraFollow : MonoBehaviour {
		public GameObject target;
		private GameObject lookAt;
		public float rotateSpeed = 5;
		Vector3 offset;
		Vector3 lookAtOffset;
		public float sensitivity = 2f;
		
		void Start() {
			offset = target.transform.position - transform.position;
			
			
			RaycastHit hit;
			Physics.Raycast(Camera.main.ViewportPointToRay(Vector3.one * .5f), out hit);
			var centerpos = hit.point;
			
			Physics.Raycast(transform.position, offset.normalized, out hit);
			var playerpos = hit.point;
			
			
			lookAtOffset = playerpos - centerpos;
			lookAt = new GameObject("CamTarget");
	
		}
		
		void LateUpdate() {
			var offSetPos2d = (Camera.main.ScreenToViewportPoint(Input.mousePosition) * 2) - Vector3.one;
			var offsetPos = new Vector3(offSetPos2d.x, 0, offSetPos2d.y);
			lookAt.transform.position = target.transform.position + offsetPos*sensitivity;
			transform.position = lookAt.transform.position - offset;
			transform.LookAt(lookAt.transform.position - lookAtOffset);
		}
	}
}