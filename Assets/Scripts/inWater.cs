using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inWater : MonoBehaviour
{
    public bool touchesWater;
    // Start is called before the first frame update
    void Start()
    {
        touchesWater = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            touchesWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Water")
        {
            touchesWater = false;
        }
    }
}
