using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MoverImage : MonoBehaviour
{
    public void ChangeImagePosition()
    {
    GetComponent<Image>().enabled = false;
    }
}
