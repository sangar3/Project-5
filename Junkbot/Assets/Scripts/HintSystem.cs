using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintSystem : MonoBehaviour
{
    public AudioClip hintsystemsfx;
    public GameObject hintplank1;
    public GameObject hintplank2;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) )
        {
            Debug.Log("the Q button pressed");
            AudioManager.Instance.PlaySFX(hintsystemsfx, 1.0f);
            hintplank1.SetActive(true);
            hintplank2.SetActive(true);

        }

       
    }
}
