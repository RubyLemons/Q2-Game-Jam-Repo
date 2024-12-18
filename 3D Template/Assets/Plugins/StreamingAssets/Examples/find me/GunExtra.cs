using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunExtra : MonoBehaviour
{

    [Header("Revolver")]

    [SerializeField] Transform bulletsStatic;
    [SerializeField] GameObject[] bulletsAnimated = new GameObject[6];

    void Update()
    {
        //Revolver

        if (WeaponSelect.equipped.type == Tks.WeaponType.Revolver)
            for (int i = 0; i < WeaponSelect.equipped.ammoLimit; i++)
            {
                if (i < WeaponSelect.equipped.ammo)
                {
                    bulletsStatic.GetChild(i).gameObject.SetActive(true);
                    bulletsAnimated[i].SetActive(false);
                }
                else
                {
                    bulletsStatic.GetChild(i).gameObject.SetActive(false);
                    bulletsAnimated[i].gameObject.SetActive(true);
                }
            }
    }
}
