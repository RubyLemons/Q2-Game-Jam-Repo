using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lazyfix : MonoBehaviour
{
    [SerializeField] GameObject imgWrapper;

    public void ShowMeThis()
    {
        for (int i = 0; i < imgWrapper.transform.childCount; i++) {
            imgWrapper.transform.GetChild(i).gameObject.SetActive(false);
        }

        imgWrapper.transform.Find(gameObject.name).gameObject.SetActive(true);
    }
}
