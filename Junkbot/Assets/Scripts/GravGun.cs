using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* "Gravity Gun" script I quickly threw together to help another user out on Reddit.
 * When clicking the mouse button, you will grab a rigidbody object in front of the
 * main camera's view.
 * Some initial information is recorded about where you grabbed the object, and
 * the difference between it's rotation and yours.
 *
 * The object will be moved around according to the offset point you initially
 * picked up.
 * Moving around, the object will rotate with the player so that the player will
 * always be viewing the object at the same angle.
 *
 *
 * Feel free to use or modify this script however you see fit.
 * I hope you guys can learn something from this script. Enjoy :)
 *
 * Original author: Jake Perry, reddit.com/user/nandos13
 */
public class GravGun : MonoBehaviour
{
    public FirstPersonAIO camScript;
    public Image reticle;

    /// <summary>The rigidbody we are currently holding</summary>
    public new Rigidbody rigidbody;

    #region Held Object Info
    /// <summary>The offset vector from the object's position to hit point, in local space</summary>
    private Vector3 hitOffsetLocal;

    /// <summary>The distance we are holding the object at</summary>
    public float currentGrabDistance;
    public bool maxGrabDistanceRange;

    /// <summary>The interpolation state when first grabbed</summary>
    private RigidbodyInterpolation initialInterpolationSetting;

    /// <summary>The difference between player & object rotation, updated when picked up or when rotated by the player</summary>
    private Vector3 rotationDifferenceEuler;
    #endregion

    /// <summary>Tracks player input to rotate current object. Used and reset every fixedupdate call</summary>
    private Vector2 rotationInput;

    /// <summary>The maximum distance at which a new object can be picked up</summary>
    public const float maxGrabDistance = 4;
    

    public Quaternion rotateBy;
    public float rotationSpeed = 20;
    public float initSensitivity;

    /// <returns>Ray from center of the main camera's viewport forward</returns>
    private Ray CenterRay()
    {
        return Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
    }

    private void Awake()
    {
        camScript = GetComponent<FirstPersonAIO>();
    }

    void Start()
    {
        initSensitivity = camScript.mouseSensitivity;
    }

    void Update()
    {
        if (!Input.GetMouseButton(0))
        {
            // We are not holding the mouse button. Release the object and return before checking for a new one

            if (rigidbody != null)
            {
                // Reset the rigidbody to how it was before we grabbed it
                rigidbody.interpolation = initialInterpolationSetting;
                rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rigidbody.gameObject.layer = 9;
                camScript.mouseSensitivity = initSensitivity;
                camScript.enableCameraMovement = true;
                rigidbody.gameObject.GetComponent<BoxCollider>().enabled = true;

                rigidbody = null;
                

                camScript.mouseSensitivity = initSensitivity;
            }

            return;
        }

        if (rigidbody == null)
        {
            // We are not holding an object, look for one to pick up

            Ray ray = CenterRay();
            RaycastHit hit;

            Debug.DrawRay(ray.origin, ray.direction * maxGrabDistance, Color.blue, 0.01f);

            if (Physics.Raycast(ray, out hit, maxGrabDistance))
            {
                // Don't pick up kinematic rigidbodies (they can't move)
                if (hit.rigidbody != null && hit.rigidbody.gameObject.tag == "moveable")//&& !hit.rigidbody.isKinematic)
                {
                    maxGrabDistanceRange = true;
                    Debug.Log("Object is picked up");


                    // Track rigidbody's initial information
                    rigidbody = hit.rigidbody;
                    initialInterpolationSetting = rigidbody.interpolation;
                    rigidbody.gameObject.layer = 11;
                    rotationDifferenceEuler = rigidbody.transform.rotation.eulerAngles - transform.rotation.eulerAngles;

                    hitOffsetLocal = hit.transform.InverseTransformVector(hit.point - hit.transform.position);

                    currentGrabDistance = Vector3.Distance(ray.origin, hit.point);

                    // Set rigidbody's interpolation for proper collision detection when being moved by the player
                    rigidbody.interpolation = RigidbodyInterpolation.Interpolate;

                    //Set rigidbody's collision detection to ContinuousDynamic to make it harder for it to clip through things
                    rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }
            }
        }
        else
        {
            //Object rotation with right mouse
            if (Input.GetMouseButton(1) && rigidbody.gameObject.tag == "moveable")
            {
                camScript.mouseSensitivity = 0;
                camScript.enableCameraMovement = false;
                rigidbody.gameObject.GetComponent<BoxCollider>().enabled = false;


                ObjRotate();
                rotationDifferenceEuler = rigidbody.transform.rotation.eulerAngles - transform.rotation.eulerAngles;
            }
            if (!Input.GetMouseButton(1))
            {
                camScript.mouseSensitivity = initSensitivity;
                camScript.enableCameraMovement = true;
                rigidbody.gameObject.GetComponent<BoxCollider>().enabled = true;
            }
        }



       
    }

    private void FixedUpdate()
    {
        if (rigidbody)
        {
            // We are holding an object, time to rotate & move it

            Ray ray = CenterRay();

            // Rotate the object to remain consistent with any changes in player's rotation
            rigidbody.MoveRotation(Quaternion.Euler(rotationDifferenceEuler + transform.rotation.eulerAngles));

            // Get the destination point for the point on the object we grabbed
            

            //Object moving towards or away from player with scrollwheel
            Vector3 holdPoint = ray.GetPoint(currentGrabDistance) 
                + ((transform.position - rigidbody.transform.position)) * Input.mouseScrollDelta.y * 0.1f;
            Debug.DrawLine(ray.origin, holdPoint, Color.blue, Time.fixedDeltaTime);

            // Apply any intentional rotation input made by the player & clear tracked input
            Vector3 currentEuler = rigidbody.rotation.eulerAngles;
            rigidbody.transform.RotateAround(rigidbody.position, transform.right, rotationInput.y);
            rigidbody.transform.RotateAround(rigidbody.position, transform.up, -rotationInput.x);

            // Remove all torque, reset rotation input & store the rotation difference for next FixedUpdate call
            rigidbody.angularVelocity = Vector3.zero;
            rotationInput = Vector2.zero;
            if (!Input.GetMouseButton(1))
                rotationDifferenceEuler = rigidbody.transform.rotation.eulerAngles - transform.rotation.eulerAngles;

            // Calculate object's center position based on the offset we stored
            // NOTE: We need to convert the local-space point back to world coordinates
            Vector3 centerDestination = holdPoint - rigidbody.transform.TransformVector(hitOffsetLocal);

            // Find vector from current position to destination
            Vector3 toDestination = centerDestination - rigidbody.transform.position;

            // Calculate force
            Vector3 force = toDestination / Time.fixedDeltaTime;

            // Remove any existing velocity and add force to move to final position
            rigidbody.velocity = Vector3.zero;
            rigidbody.AddForce(force, ForceMode.VelocityChange);

        }




    }

    public void ObjRotate()
    {
        ///My personal code for rotating with a nonmoving camera
        ///
        /*if (Input.GetMouseButtonDown(1))
           // selected.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;

        yaw += Input.GetAxis("Mouse X") * rotationSpeed * -1;
        pitch += Input.GetAxis("Mouse Y") * rotationSpeed;

        selected.transform.eulerAngles = new Vector3(pitch, yaw, 0);*/

        ///Code adapted from Raigex on the Unity forums
        ///https://answers.unity.com/questions/299126/how-to-rotate-relative-to-camera-angleposition.html
        ///
        //Gets the world vector space for cameras up vector 
        Vector3 relativeUp = this.transform.TransformDirection(Vector3.up);
        //Gets world vector for space cameras right vector
        Vector3 relativeRight = this.transform.TransformDirection(Vector3.right);

        //Turns relativeUp vector from world to objects local space
        Vector3 objectRelativeUp = rigidbody.transform.InverseTransformDirection(relativeUp);
        //Turns relativeRight vector from world to object local space
        Vector3 objectRelativeRight = rigidbody.transform.InverseTransformDirection(relativeRight);

        rotateBy = Quaternion.AngleAxis(Input.GetAxis("Mouse X") / gameObject.transform.localScale.x * -rotationSpeed, objectRelativeUp)
            * Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") / gameObject.transform.localScale.x * -rotationSpeed, objectRelativeRight);

        rigidbody.transform.localRotation *= rotateBy;
    }

}
