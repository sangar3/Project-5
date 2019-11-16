using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CollisionGameObject : MonoBehaviour
{
    public GameObject morphedobject;

    public GameObject box1;
    public GameObject box2;
    public AudioClip morphedobjectsfx;

    //Detect collisions between the GameObjects with Colliders attached
    void OnCollisionEnter(Collision collision)
    {
        //Check for a match with the specified name on any GameObject that collides with your GameObject
        if (collision.gameObject.name == "testbox2")
        {
            
            Debug.Log("testbox1 collided with testbox2");
            Destroy(box1, .2f);
            Destroy(box2, .2f);
            Instantiate(morphedobject);
            morphedobject.SetActive(true);
            AudioManager.Instance.PlaySFX(morphedobjectsfx, 1.0f);    //add sound fx 
        }
        /*
        //Check for a match with the specific tag on any GameObject that collides with your GameObject
        if (collision.gameObject.tag == "movable")
        {
            //If the GameObject has the same tag as specified, output this message in the console
            Debug.Log("testbox1 collided with testbox2");
        }
        */
    }
}
