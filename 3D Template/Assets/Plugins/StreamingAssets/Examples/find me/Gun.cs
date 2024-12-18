using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    
    [SerializeField] Freelook freelook;
    [SerializeField] Transform viewmodel;

    [SerializeField] Transform camAnimated;
    [SerializeField] Animator camShake;

    [Space(10)]

    [SerializeField] Transform ammoRapper;
    [SerializeField] GameObject ammoImg;

    [SerializeField] GameObject bulletScar;
    [SerializeField] LayerMask layers;

    float elaspedTime;

    [Space(10)]

    bool ammoFull;
    bool ammoEmpty;


    bool fireDeb;
    bool reloadDeb;

    bool fireAction;
    bool reloadAction;

    void Start()
    {
        AmmoGui();
    }

    void Update()
    {
        if (WeaponSelect.wheelEnabled || Time.timeScale < 1) return;

        //ammo clamp
        WeaponSelect.equipped.ammo = Mathf.Clamp(WeaponSelect.equipped.ammo, 0, WeaponSelect.equipped.ammoLimit);


        //Animate Gui

        Tks.OnValueChanged((p) => AmmoGui(), WeaponSelect.equipped.ammo, "Ammo");


        //Fire

        reloadAction = WeaponSelect.equipped.animator.GetCurrentAnimatorStateInfo(0).IsName("reload");
        fireAction = WeaponSelect.equipped.animator.GetCurrentAnimatorStateInfo(0).IsName("fire");

        elaspedTime += Time.deltaTime;
        fireDeb = (elaspedTime < WeaponSelect.equipped.fireRate);

        ammoFull = WeaponSelect.equipped.ammo == WeaponSelect.equipped.ammoLimit;
        ammoEmpty = WeaponSelect.equipped.ammo <= 0;

        if (Input.GetMouseButton(0) && !ammoEmpty && (!fireDeb && !reloadAction)) {

            elaspedTime = 0;

            for (int i = 0; i < WeaponSelect.equipped.bullets; i++)
                Fire();
        }

        if (Input.GetMouseButtonDown(0) && ammoEmpty)
            Reload();


        //Reload

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        //cam animated recoil
        camAnimated.localRotation = Quaternion.Lerp(camAnimated.localRotation, Quaternion.identity, WeaponSelect.equipped.recoilSmooth);
        viewmodel.localPosition = Vector3.Lerp(viewmodel.localPosition, Vector3.zero, WeaponSelect.equipped.pullBackSmooth);
    }

    public void Fire()
    {
        //aniamte

        WeaponSelect.equipped.animator.Play("fire", 0, 0.0f);
        camShake.Play(WeaponSelect.equipped.shake, 0, 0.0f);

        WeaponSelect.equipped.ammo -= 1;

        camAnimated.transform.localRotation *= Quaternion.Euler(Vector3.left * WeaponSelect.equipped.recoil / WeaponSelect.equipped.bullets);
        viewmodel.localPosition += (Vector3.forward * -Mathf.Abs(WeaponSelect.equipped.pullBack));


        //Raycast

        Vector3 spread = (freelook.cam.transform.right * Random.Range((float)-WeaponSelect.equipped.spread, WeaponSelect.equipped.spread)) + (freelook.cam.transform.up * Random.Range((float)-WeaponSelect.equipped.spread, WeaponSelect.equipped.spread));
        bool ray = Physics.Raycast(freelook.cam.transform.position, freelook.cam.transform.forward + spread, out RaycastHit hit, 100, layers);


        //Attack

        if (ray)
        {
            Enemy enemy = hit.collider.GetComponent<Enemy>();

            bool head = (enemy == null && hit.collider.tag == "Enemy");
            enemy = (head) ? hit.collider.transform.parent.GetComponent<Enemy>() : enemy;

            if (enemy)
            {
                enemy.health -= WeaponSelect.equipped.damage * ((head) ? 2 : 1);
            }
            else
            {
                BulletScar(hit);
            }
        }
    }

    void BulletScar(RaycastHit _hit)
    {
        GameObject newBulletScar = Instantiate(bulletScar, null);
        newBulletScar.SetActive(true);
        newBulletScar.transform.localScale = Vector3.one * 0.00375f;

        newBulletScar.transform.position = _hit.point - (freelook.cam.transform.forward * 0.001f);
        newBulletScar.transform.rotation = Quaternion.LookRotation(_hit.normal) * Quaternion.Euler(Vector3.left * -90) * Quaternion.Euler(Vector3.up * Random.Range(0, 90));
    }


    void Reload()
    {
        if (reloadDeb || reloadAction || fireAction || ammoFull) return;
        reloadDeb = true;

        WeaponSelect.equipped.animator.Play(!ammoEmpty ? WeaponSelect.equipped.reload[0] : WeaponSelect.equipped.reload[1]);

        float sleep = WeaponSelect.equipped.reloadTime * 1000;

        StartCoroutine(Tks.SetTimeout(() => {
            WeaponSelect.equipped.ammo = WeaponSelect.equipped.ammoLimit;
            reloadDeb = false;
        }, sleep));
    }

    void AmmoGui()
    {
        //add new
        for (int i = 0; i < WeaponSelect.equipped.ammo; i++)
            Instantiate(ammoImg, ammoRapper);

        //clear all
        for (int i = 0; i < ammoRapper.childCount; i++)
            if (i >= WeaponSelect.equipped.ammo)
            {
                Destroy(ammoRapper.GetChild(i).gameObject);
            }
    }
}
