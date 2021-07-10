using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAround1 : MonoBehaviour
{
    float time;

    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;    
    }

    // Update is called once per frame
    void Update()
    {
        float elapsed = Time.time - time;

        if( elapsed > 10.0f )
        {
            gameObject.SetActive(false);
        }
    }
}
