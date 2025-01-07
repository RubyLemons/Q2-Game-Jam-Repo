using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
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
        WeaponSelect.equipped.ammoTopic.ammo = Mathf.Clamp(WeaponSelect.equipped.ammoTopic.ammo, 0, WeaponSelect.equipped.ammoTopic.ammoLimit);


        //Animate Gui

        Tks.OnValueChanged((p) => AmmoGui(), WeaponSelect.equipped.ammoTopic.ammo, "Ammo");


        //Fire

        reloadAction = WeaponSelect.equipped.animateTopic.animator.GetCurrentAnimatorStateInfo(0).IsName("reload");
        fireAction = WeaponSelect.equipped.animateTopic.animator.GetCurrentAnimatorStateInfo(0).IsName("fire");

        elaspedTime += Time.deltaTime;
        fireDeb = (elaspedTime < WeaponSelect.equipped.fireTopic.fireRate);

        ammoFull = WeaponSelect.equipped.ammoTopic.ammo == WeaponSelect.equipped.ammoTopic.ammoLimit;
        ammoEmpty = WeaponSelect.equipped.ammoTopic.ammo <= 0;

        if (Input.GetMouseButton(0) && !ammoEmpty && (!fireDeb && !reloadAction)) {

            elaspedTime = 0;

            for (int i = 0; i < WeaponSelect.equipped.fireTopic.bullets; i++)
                Fire();
        }

        if (Input.GetMouseButtonDown(0) && ammoEmpty)
            Reload();


        //Reload

        if (Input.GetKeyDown(KeyCode.R))
            Reload();

        //cam animated recoil
        camAnimated.localRotation = Quaternion.Lerp(camAnimated.localRotation, Quaternion.identity, WeaponSelect.equipped.fireTopic.recoilSmoothing);
        viewmodel.localPosition = Vector3.Lerp(viewmodel.localPosition, Vector3.zero, WeaponSelect.equipped.animateTopic.pullBackSmoothing);
    }

    public void Fire()
    {
        //aniamte

        WeaponSelect.equipped.animateTopic.animator.Play("fire", 0, 0.0f);
        camShake.Play(WeaponSelect.equipped.animateTopic.shake, 0, 0.0f);

        WeaponSelect.equipped.ammoTopic.ammo -= 1;

        camAnimated.transform.localRotation *= Quaternion.Euler(Vector3.left * WeaponSelect.equipped.fireTopic.recoil / WeaponSelect.equipped.fireTopic.bullets);
        viewmodel.localPosition += (Vector3.forward * -Mathf.Abs(WeaponSelect.equipped.animateTopic.pullBack));


        //Raycast

        Vector3 spread = (freelook.cam.transform.right * Random.Range((float)-WeaponSelect.equipped.fireTopic.spread, WeaponSelect.equipped.fireTopic.spread)) + (freelook.cam.transform.up * Random.Range((float)-WeaponSelect.equipped.fireTopic.spread, WeaponSelect.equipped.fireTopic.spread));

        bool ray = false;
        RaycastHit[] hit = new RaycastHit[] { };

        if (!WeaponSelect.equipped.fireTopic.piercing) {
            ray = Physics.Raycast(freelook.cam.transform.position, freelook.cam.transform.forward + spread, out hit[0], 100, layers);
        }
        else {
            hit = Physics.RaycastAll(freelook.cam.transform.position, freelook.cam.transform.forward + spread, 100, layers);

            //reorder with for ipairs
            System.Array.Sort(hit, (a, b) => a.distance.CompareTo(b.distance));
        }

        //Attack

        if (ray) return;

        int reductionMultiplier = 0;

        for (int i = 0; i < hit.Length; i++)
        {
            Enemy enemy = hit[i].collider.GetComponent<Enemy>();

            bool head = (enemy == null && hit[i].collider.tag == "Enemy");
            enemy = (head) ? hit[i].collider.transform.parent.GetComponent<Enemy>() : enemy;

            if (enemy)
            {
                enemy.health -= WeaponSelect.equipped.fireTopic.damage * (head ? 2 : 1) * (1 - (i * WeaponSelect.equipped.fireTopic.breakDamageReduction)); //decrease by 25% each break
                print(enemy +" "+ WeaponSelect.equipped.fireTopic.damage * (head ? 2 : 1) * (1 - (i * WeaponSelect.equipped.fireTopic.breakDamageReduction)));

                reductionMultiplier++;

                //blood particle effect can happen here
            }
            else
            {
                BulletScar(hit[i]);
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

        WeaponSelect.equipped.animateTopic.animator.Play(!ammoEmpty ? WeaponSelect.equipped.animateTopic.reload[0] : WeaponSelect.equipped.animateTopic.reload[1]);

        float sleep = WeaponSelect.equipped.fireTopic.reloadTime * 1000;

        StartCoroutine(Tks.SetTimeout(() => {
            WeaponSelect.equipped.ammoTopic.ammo = WeaponSelect.equipped.ammoTopic.ammoLimit;
            reloadDeb = false;
        }, sleep));
    }

    void AmmoGui()
    {
        //add new
        for (int i = 0; i < WeaponSelect.equipped.ammoTopic.ammo; i++)
            Instantiate(ammoImg, ammoRapper);

        //clear all
        for (int i = 0; i < ammoRapper.childCount; i++)
            if (i >= WeaponSelect.equipped.ammoTopic.ammo)
            {
                Destroy(ammoRapper.GetChild(i).gameObject);
            }
    }
}
