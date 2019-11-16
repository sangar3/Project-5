using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Stephen Gruver
 * FirstPersonCamera.cs
 * Project 1
 * Allows the player to control the camera in first person. Meant for building/moving mode.
 */
public class FirstPersonCamera : MonoBehaviour
{
    public float XSensitivity;
    public float YSensitivity;
    public float MinimumX = -90F;
    public float MaximumX = 90F;

    public bool smooth;
    public float smoothTime = 5f;
    public bool clampVerticalRotation = true;
    public bool lockCursor = true;
    private bool cursorLocked = true;

    Quaternion gameobjRotation;
    Quaternion camRotation;

    Transform player;
    Transform camTrans;

    // Start is called before the first frame update
    void Start()
    {
        player = this.transform;
        camTrans = GetComponentInChildren<Camera>().transform;

        gameobjRotation = player.localRotation;
        camRotation = camTrans.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        fpRotation();
    }

    void fpMovement()
    {
        
    }

    void fpRotation()
    {
        float yRot = Input.GetAxis("Mouse X") * XSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * YSensitivity;

        gameobjRotation *= Quaternion.Euler(0f, yRot, 0f);
        camRotation *= Quaternion.Euler(-xRot, 0f, 0f);

        if (clampVerticalRotation)
            camRotation = ClampRotationAroundXAxis(camRotation);

        if (smooth)
        {
            this.transform.localRotation = Quaternion.Slerp(player.localRotation, gameobjRotation,
                smoothTime * Time.deltaTime);
            camTrans.localRotation = Quaternion.Slerp(camTrans.localRotation, camRotation,
                smoothTime * Time.deltaTime);
        }
        else
        {
            player.localRotation = gameobjRotation;
            camTrans.localRotation = camRotation;
        }

        UpdateCursorLock();
    }

    public void SetCursorLock(bool value)
    {
        lockCursor = value;
        if (!lockCursor)
        {//we force unlock the cursor if the user disable the cursor locking helper
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void UpdateCursorLock()
    {
        //if the user set "lockCursor" we check & properly lock the cursos
        if (lockCursor)
            InternalLockUpdate();
    }

    private void InternalLockUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            cursorLocked = false;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            cursorLocked = true;
        }

        if (cursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (!cursorLocked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);

        angleX = Mathf.Clamp(angleX, MinimumX, MaximumX);

        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }
}
