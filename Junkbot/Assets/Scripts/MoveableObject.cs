using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    private Vector3 resetPos;
    private Quaternion resetRot;

    private void Start()
    {
        this.tag = "moveable";
        resetPos = this.transform.position;
        resetRot = this.transform.rotation;
    }

    private void Update()
    {   //this could be repaced by the inclusion of trigger boundaries above and below the level.
        if (gameObject.transform.position.y < -20 || gameObject.transform.position.y > -2)
            ObjectRespawn();
    }

    public void ObjectRespawn()
    {
        this.gameObject.layer = 9;                              //changes object back to "object" in case it's still "selected".
        this.GetComponent<Rigidbody>().velocity = Vector3.zero; //removes velocity/force from the object
        this.transform.position = resetPos;                   //returns object to its initial position
        this.transform.rotation = resetRot;                   //returns object ro its initial
    }

    public void SetRespawn(Vector3 pos, Quaternion rot)
    {
        resetPos = pos;
        resetRot = rot;
    }

    public Vector3 GetRespawnPos()
    { return resetPos; }

    public Quaternion GetRespawnRot()
    { return resetRot; }

}
