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

    [Header("Combo")]

    [Range(0, 1)] [SerializeField] float onShot = 0.01f;
    [Range(0, 1)] [SerializeField] float onHeadShot = 0.05f;
    [Range(0, 1)] [SerializeField] float onSlideMultiplier = 2.5f;
    [Range(0, 1)] [SerializeField] float onSlideShot = 0.07f;

    void Start()
    {
        AddNewAmmoGui();
    }

    void Update()
    {
        if (WeaponSelect.wheelEnabled || Time.timeScale < 1) return;

        //ammo clamp
        WeaponSelect.equipped.ammoTopic.ammo = Mathf.Clamp(WeaponSelect.equipped.ammoTopic.ammo, 0, WeaponSelect.equipped.ammoTopic.ammoLimit);


        //Animate Gui

        Tks.OnValueChanged((p) => AddNewAmmoGui(), WeaponSelect.equipped.ammoTopic.ammo, "Ammo"); //outdated


        //Fire

        reloadAction = WeaponSelect.equipped.animateTopic.animator.GetCurrentAnimatorStateInfo(0).IsName("reload");
        fireAction = WeaponSelect.equipped.animateTopic.animator.GetCurrentAnimatorStateInfo(0).IsName("fire");

        elaspedTime += Time.deltaTime;
        fireDeb = (elaspedTime < WeaponSelect.equipped.fireTopic.fireRate);

        ammoFull = WeaponSelect.equipped.ammoTopic.ammo == WeaponSelect.equipped.ammoTopic.ammoLimit;
        ammoEmpty = WeaponSelect.equipped.ammoTopic.ammo <= 0;

        if (Input.GetMouseButton(0) && !ammoEmpty && (!fireDeb && !reloadAction))
        {

            elaspedTime = 0;

            WeaponSelect.equipped.ammoTopic.ammo -= 1;

            RemoveAmmoGui();

            for (int i = 0; i < WeaponSelect.equipped.fireTopic.bullets; i++)
                Fire();
        }

        if (Input.GetMouseButtonDown(0) && ammoEmpty)
            Reload();


        //Reload

        if (Input.GetKeyDown(KeyCode.R))
            Reload();
        
        if (Input.GetKeyDown(KeyCode.F)) {
            AddNewAmmoGui();
        }

        //cam animated recoil
        camAnimated.localRotation = Quaternion.Lerp(camAnimated.localRotation, Quaternion.identity, WeaponSelect.equipped.fireTopic.recoilSmoothing);
        viewmodel.localPosition = Vector3.Lerp(viewmodel.localPosition, Vector3.zero, WeaponSelect.equipped.animateTopic.pullBackSmoothing);
    }

    public void Fire()
    {
        //aniamte

        WeaponSelect.equipped.animateTopic.animator.Play("fire", 0, 0.0f);
        camShake.Play(WeaponSelect.equipped.animateTopic.shake, 0, 0.0f);

        camAnimated.transform.localRotation *= Quaternion.Euler(Vector3.left * WeaponSelect.equipped.fireTopic.recoil / WeaponSelect.equipped.fireTopic.bullets);
        viewmodel.localPosition += (Vector3.forward * -Mathf.Abs(WeaponSelect.equipped.animateTopic.pullBack));


        //Raycast

        #region --audio src
        WeaponSelect.equipped.audioTopic.audioSrc.pitch = 1 + Random.Range((float)-WeaponSelect.equipped.audioTopic.pitchDifferRange, WeaponSelect.equipped.audioTopic.pitchDifferRange); //randomize pitch
        WeaponSelect.equipped.audioTopic.audioSrc.PlayOneShot(WeaponSelect.equipped.audioTopic.clip, WeaponSelect.equipped.audioTopic.volume); //play new and volume
        #endregion

        Vector3 spread = (freelook.cam.transform.right * Random.Range((float)-WeaponSelect.equipped.fireTopic.spread, WeaponSelect.equipped.fireTopic.spread)) + (freelook.cam.transform.up * Random.Range((float)-WeaponSelect.equipped.fireTopic.spread, WeaponSelect.equipped.fireTopic.spread));

        bool ray = false;
        RaycastHit[] hit = new RaycastHit[] { };

        if (!WeaponSelect.equipped.fireTopic.piercing)
        {
            hit = new RaycastHit[1];
            ray = Physics.Raycast(freelook.cam.transform.position, freelook.cam.transform.forward + spread, out hit[0], 100, layers);
        }
        else
        {
            hit = Physics.RaycastAll(freelook.cam.transform.position, freelook.cam.transform.forward + spread, 100, layers);

            //reorder with for ipairs
            System.Array.Sort(hit, (a, b) => a.distance.CompareTo(b.distance));
        }

        //Attack

        int reductionMultiplier = 0;

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].collider == null) continue; //skip if no hit fool!

            Enemy enemy = hit[i].collider.GetComponent<Enemy>();

            bool head = (enemy == null && hit[i].collider.tag == "Enemy");
            enemy = (head) ? hit[i].collider.transform.parent.GetComponent<Enemy>() : enemy;

            if (enemy)
            {
                enemy.health -= WeaponSelect.equipped.fireTopic.damage * (head ? 2 : 1) * (1 - (i * WeaponSelect.equipped.fireTopic.breakDamageReduction)); //decrease by 25% each break

                reductionMultiplier++;

                //blood particle effect can happen here
            }
            else
            {
                BulletScar(hit[i]);
            }



            #region --COMBO LAZY

            if (head)
            {
                GetComponent<Combo>().value += onHeadShot * (Freeroam.slideDeb ? onSlideMultiplier : 1);
            }
            else if (enemy && !head)
            {
                if (Freeroam.slideDeb)
                {
                    GetComponent<Combo>().value += onSlideShot;
                }
                else
                {
                    if (enemy.health <= 0)
                    {
                        GetComponent<Combo>().value += onShot;
                    }
                }
            }
            #endregion
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

        StartCoroutine(Tks.SetTimeout(() =>
        {
            WeaponSelect.equipped.ammoTopic.ammo = WeaponSelect.equipped.ammoTopic.ammoLimit;
            reloadDeb = false;

            AddNewAmmoGui();
        }, sleep));
    }

    void AddNewAmmoGui()
    {
        //add new ammo
        for (int i = 0; i < WeaponSelect.equipped.ammoTopic.ammoLimit; i++) {
            Instantiate(ammoImg, ammoRapper);
        }
            

        //clear all extras
        for (int i = 0; i < ammoRapper.childCount; i++)
        {
            if (i >= WeaponSelect.equipped.ammoTopic.ammo)
            {
                Destroy(ammoRapper.GetChild(i).gameObject);
            }
        }
    }
    
    void RemoveAmmoGui()
    {
        //remove one

        Animator thisImg = ammoRapper.GetChild(ammoRapper.childCount - 1).GetComponent<Animator>();

        thisImg.transform.SetParent(ammoRapper.parent); //unparent

        thisImg.enabled = true; //play
    }
}
