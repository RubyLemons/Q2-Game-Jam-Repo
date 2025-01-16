using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImgMover : MonoBehaviour
{

    [SerializeField] Image img;

    public void ToggleVisibility()
    {
        if (img.enabled == true)
        {
            GetComponent<Image>();
            img.enabled = false;
        }
        else
        {
            GetComponent<Image>();
            img.enabled = true;
        }
    }
}
