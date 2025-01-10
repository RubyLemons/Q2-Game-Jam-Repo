using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitmarker : MonoBehaviour
{
    // Start is called before the first frame update
    public bool IsOn = false;
    public void Start()
    {
        IsOn = false;
    }

    // Update is called once per frame
    public void Update()
    {
        if (IsOn == true)
        {
           
            IsOn = false;

        }
    }
}
