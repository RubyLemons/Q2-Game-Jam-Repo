using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ImgMover : MonoBehaviour
{

    [SerializeField] Image img;

    public void ChangeImagePosition()
    {
        img = GetComponent<Image>();
        img.enabled = false;
    }
}
