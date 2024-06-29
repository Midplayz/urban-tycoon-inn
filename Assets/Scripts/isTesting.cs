using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class isTesting : MonoBehaviour
{
    public bool isInTesting = false;
    // Start is called before the first frame update
    void Start()
    {
        if (isInTesting)
        {
            Time.timeScale = 2.0f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
