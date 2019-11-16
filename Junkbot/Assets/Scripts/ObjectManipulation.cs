using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManipulation : MonoBehaviour
{
    public Camera playerCam;
    private Vector3 holdPosition;

    public FirstPersonAIO camScript;
    private float initSensitivity;

    public Rigidbody selected;
    public float maxGrabRange;
    private float holdDistance;
    private RigidbodyInterpolation initialInterpolationSetting;

    private Rigidbody rotatedObject;
    private Vector3 rotationDifferenceEuler;
    public float rotationSpeed = 20;
    private Quaternion rotateBy;
    private bool rotateDown;
    private Vector2 rotationInput;

    public Image crosshair;

    private Ray CenterRay()
    {
        return Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
    }

    private void Awake()
    {
        camScript = GetComponent<FirstPersonAIO>();
        //holdPosition = CenterRay().GetPoint(1f);
    }

    // Start is called before the first frame update
    void Start()
    {
        initSensitivity = camScript.mouseSensitivity;
    }

    void FixedUpdate()
    {
        InputHandler(CenterRay());
    }

    private void InputHandler(Ray ray)
    {
        DetectObjects(ray);

        if (Input.GetMouseButtonDown(0) && selected == null)
        {
            PickUp(ray);
        }
        else if (Input.GetMouseButton(0) && selected)
        {
            ManipulateSelected(ray);
        }
        else if (!Input.GetMouseButton(0) && selected)
        {
            DropSelected();
        }
    }

    private void PickUp(Ray ray)
    {
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * maxGrabRange, Color.blue, 0.01f);

        if (Physics.Raycast(ray, out hit, maxGrabRange))
        {
            // Don't pick up kinematic rigidbodies (they can't move)
            if (hit.rigidbody != null && hit.rigidbody.gameObject.tag == "moveable")//&& !hit.rigidbody.isKinematic)
            {
                initialInterpolationSetting = hit.rigidbody.interpolation;
                hit.rigidbody.gameObject.layer = 11;
                hit.rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                hit.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                hit.rigidbody.useGravity = false;
                hit.rigidbody.freezeRotation = true;
                hit.rigidbody.isKinematic = true;
                rotationDifferenceEuler = hit.rigidbody.transform.rotation.eulerAngles - transform.rotation.eulerAngles;

                holdDistance = hit.distance;
                //hit.rigidbody.position = Vector3.Lerp(hit.transform.position, holdPosition, Time.deltaTime * 15);

                selected = hit.rigidbody;
            }
        }
    }

    private void ManipulateSelected(Ray ray)
    {
        holdPosition = ray.GetPoint(holdDistance);
        selected.velocity = Vector3.zero;

        selected.transform.position = Vector3.Lerp(selected.transform.position, holdPosition, Time.deltaTime * 15);

        if (!Input.GetMouseButton(1))
        {
            if (rotateDown)
            {
                camScript.mouseSensitivity = initSensitivity;
                camScript.enableCameraMovement = true;
                rotatedObject.gameObject.GetComponent<BoxCollider>().enabled = false;
                rotatedObject = null;
                rotateDown = false;
            }

            selected.transform.rotation = Quaternion.Euler(rotationDifferenceEuler + transform.rotation.eulerAngles);
        }
        if (Input.GetMouseButton(1))
        {
            RotateSelected();
            rotationDifferenceEuler = selected.transform.rotation.eulerAngles - transform.rotation.eulerAngles;
        }

        if (Input.mouseScrollDelta.magnitude != 0)
        {
            if (Input.mouseScrollDelta.y > 0 && holdDistance <= 3.5f)
            {
                holdDistance += Input.mouseScrollDelta.y * 0.1f;
            }
            else if (Input.mouseScrollDelta.y < 0 && holdDistance >= 2f)
            {
                holdDistance += Input.mouseScrollDelta.y * 0.1f;
            }
            
        }
    }

    private void RotateSelected()
    {
        rotateDown = true;
        rotatedObject = selected;
        camScript.mouseSensitivity = 0;
        camScript.enableCameraMovement = false;
        selected.gameObject.GetComponent<BoxCollider>().enabled = false;

        ///Code adapted from Raigex on the Unity forums
        ///https://answers.unity.com/questions/299126/how-to-rotate-relative-to-camera-angleposition.html
        ///
        //Gets the world vector space for cameras up vector 
        Vector3 relativeUp = this.transform.TransformDirection(Vector3.up);
        //Gets world vector for space cameras right vector
        Vector3 relativeRight = this.transform.TransformDirection(Vector3.right);

        //Turns relativeUp vector from world to objects local space
        Vector3 objectRelativeUp = selected.transform.InverseTransformDirection(relativeUp);
        //Turns relativeRight vector from world to object local space
        Vector3 objectRelativeRight = selected.transform.InverseTransformDirection(relativeRight);

        rotateBy = Quaternion.AngleAxis(Input.GetAxis("Mouse X") / gameObject.transform.localScale.x * -rotationSpeed, objectRelativeUp)
            * Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") / gameObject.transform.localScale.x * -rotationSpeed, objectRelativeRight);

        selected.transform.localRotation *= rotateBy;

        
    }

    public void DropSelected()
    {
        // Reset everything to how it was before we picked up the object
        selected.interpolation = initialInterpolationSetting;
        selected.collisionDetectionMode = CollisionDetectionMode.Discrete;
        selected.gameObject.layer = 9;
        camScript.mouseSensitivity = initSensitivity;
        camScript.enableCameraMovement = true;
        selected.gameObject.GetComponent<BoxCollider>().enabled = true;
        selected.useGravity = true;
        selected.freezeRotation = false;
        selected.isKinematic = false;
        holdDistance = 3f;

        selected = null;
    }

    private void DetectObjects(Ray ray)
    {
        RaycastHit hit;
        Physics.Raycast(ray, out hit, maxGrabRange);

        if (hit.rigidbody)
        {
            if (hit.rigidbody.gameObject.tag == "moveable")
            {
                //Debug.Log("Pickup-able object found");
                crosshair.color = Color.cyan;
            }
            else
            {
                crosshair.color = Color.white;
            }
        }
        else
            crosshair.color = Color.white;
    }
}




