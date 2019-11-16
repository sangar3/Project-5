using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectBuilder : MonoBehaviour
{
    public GameObject prebuiltobject;
    public GameObject builtobject;
    public AudioClip ObjectBuiltsfx;



    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.name == "MainObjectHolder")
        {

            Debug.Log("testbox3 collided with MainObjectHolder");
            Destroy(prebuiltobject, .2f);
            builtobject.SetActive(true);
            AudioManager.Instance.PlaySFX(ObjectBuiltsfx, 1.0f);    //add sound fx 



        }
    }


}
