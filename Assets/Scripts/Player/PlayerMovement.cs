using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;

namespace CompleteProject
{
    public class PlayerMovement : MonoBehaviour
    {
        public Camera mainCamera;
        private Transform mainCameraTransform;
        private Vector3 cameraVelocity = Vector3.zero;
        private Vector3 cameraOffset= Vector3.zero;
        private Vector3 initOffsetToPlayer;
        public float cursorPlaneHeight = 0;
        public float speed = 6f;            // The speed that the player will move at.
        Vector3 movement;                   // The vector to store the direction of the player's movement.
        Animator anim;                      // Reference to the animator component.
        Rigidbody playerRigidbody;          // Reference to the player's rigidbody.
        Plane playerMovementPlane;
        private Quaternion screenMovementSpace ;
        private Vector3 screenMovementForward ;
        private Vector3 screenMovementRight ;
        public float cameraSmoothing = 0.01f;
        private Transform cursorObject ;
        private PlayerMelee playerMelee;

#if !MOBILE_INPUT
       public int floorMask;                      // A layer mask so that a ray can be cast just at gameobjects on the floor layer.
        float camRayLength = 75f;          // The length of the ray from the camera into the scene.
#endif

        void Awake ()
        {

  
#if !MOBILE_INPUT
            // Create a layer mask for the floor layer.
            Debug.Log (floorMask = LayerMask.GetMask ("Floor"));
#endif

            // Set up references.
            anim = GetComponent <Animator> ();
            playerRigidbody = GetComponent <Rigidbody> ();
            mainCamera = Camera.main;
            mainCameraTransform = mainCamera.transform;
            initOffsetToPlayer = mainCameraTransform.position - playerRigidbody.position;
            cameraOffset = mainCameraTransform.position - playerRigidbody.position;
            playerMovementPlane = new Plane(this.transform.up, this.transform.position + this.transform.up * cursorPlaneHeight);
            screenMovementSpace = Quaternion.Euler(0, mainCameraTransform.eulerAngles.y, 0);
            screenMovementForward = screenMovementSpace * Vector3.forward;
            screenMovementRight = screenMovementSpace * Vector3.right;

            playerMelee = GetComponent<PlayerMelee>();

	
        }


        void FixedUpdate ()
        {

            // Store the input axes.
            float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            float v = CrossPlatformInputManager.GetAxisRaw("Vertical");

            playerMovementPlane.normal = this.transform.up;
            playerMovementPlane.distance = -playerRigidbody.position.y + cursorPlaneHeight;
            // Move the player around the scene.
            Move (h, v);

            // Turn the player to face the mouse cursor.
            Turning ();

            // Animate the player.
            Animating (h, v);

          
            	// Set the target position of the camera to point at the focus point
         
        }


        void Move (float h, float v)
        {       
             Vector3 cameraAdjustmentVector = Vector3.zero;
            // Set the movement vector based on the axis input.
            movement.Set (h, 0f, v);
            
            // Normalise the movement vector and make it proportional to the speed per second.
            movement = movement.normalized * speed * Time.deltaTime;
            Vector3 cursorScreenPosition = new Vector3(h, v, v);
            // Move the player to it's current position plus the movement.
            playerRigidbody.MovePosition (transform.position + movement);
            Vector3 cursorWorldPosition = ScreenPointToWorldPointOnPlane(cursorScreenPosition, playerMovementPlane, mainCamera);
           //var cursorWorldPosition : Vector3 = ScreenPointToWorldPointOnPlane (cursorScreenPosition, playerMovementPlane, mainCamera);
			
			float halfWidth  = Screen.width / 2.0f;
			float halfHeight  = Screen.height / 2.0f;
			float maxHalf = Mathf.Max (halfWidth, halfHeight);
			
			// Acquire the relative screen position			
			Vector3 posRel  = cursorScreenPosition - new Vector3 (halfWidth, halfHeight, cursorScreenPosition.z);		
			posRel.x /= maxHalf; 
			posRel.y /= maxHalf;
						
//			cameraAdjustmentVector = posRel.x * screenMovementRight + posRel.y * screenMovementForward;
//			cameraAdjustmentVector.y = 0.0f;
//
//            Vector3 cameraTargetPosition = this.transform.position + initOffsetToPlayer + cameraAdjustmentVector * 2.0f;
//
//            // Apply some smoothing to the camera movement
//            mainCameraTransform.position = Vector3.SmoothDamp(mainCameraTransform.position, cameraTargetPosition, ref cameraVelocity, cameraSmoothing);
//
//            // Save camera offset so we can use it in the next frame
//            cameraOffset = mainCameraTransform.position - this.transform.position;
        }

        public static Vector3 PlaneRayIntersection (Plane plane ,Ray ray ){
	        float dist;
	        plane.Raycast (ray, out dist);
	        return ray.GetPoint (dist);
        }
           public static Vector3 ScreenPointToWorldPointOnPlane (Vector3 screenPoint , Plane plane, Camera camera) {
	        // Set up a ray corresponding to the screen position
               Ray ray = camera.ScreenPointToRay(screenPoint);
	
	            // Find out where the ray intersects with the plane
	            return PlaneRayIntersection (plane, ray);
        }

        void Turning ()
        {
#if !MOBILE_INPUT

            if (playerMelee.isAttacking) return;
            // Create a ray from the mouse cursor on screen in the direction of the camera.
            Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

            // Create a RaycastHit variable to store information about what was hit by the ray.
            RaycastHit floorHit;

            // Perform the raycast and if it hits something on the floor layer...
            if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = floorHit.point - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation (playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation (newRotatation);
            }
#else

            Vector3 turnDir = new Vector3(CrossPlatformInputManager.GetAxisRaw("Mouse X") , 0f , CrossPlatformInputManager.GetAxisRaw("Mouse Y"));

            if (turnDir != Vector3.zero)
            {
                // Create a vector from the player to the point on the floor the raycast from the mouse hit.
                Vector3 playerToMouse = (transform.position + turnDir) - transform.position;

                // Ensure the vector is entirely along the floor plane.
                playerToMouse.y = 0f;

                // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
                Quaternion newRotatation = Quaternion.LookRotation(playerToMouse);

                // Set the player's rotation to this new rotation.
                playerRigidbody.MoveRotation(newRotatation);
            }
#endif
        }


        void Animating (float h, float v)
        {
            // Create a boolean that is true if either of the input axes is non-zero.
            bool walking = h != 0f || v != 0f;

            // Tell the animator whether or not the player is walking.
            anim.SetBool ("IsWalking", walking);
        }

  


    }

}