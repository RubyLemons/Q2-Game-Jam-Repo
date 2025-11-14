using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Health : BarController
{
    [Header("Gui")]

    [SerializeField] CanvasGroup vignette;

    [Header("Animate")]

    [SerializeField] Transform camAnimated;

    [Header("Aniamted visuals")]

    [SerializeField] Volume vol;

    [Header("Sound")]

    [SerializeField] AudioSource audioSrc;
    [Tooltip("Clip that plays upon taking damage")] [SerializeField] AudioClip clip;

    [Space(10)]

    [SerializeField] AudioSource track;


    protected override void Awake()
    {
        base.Awake();

        audioSrc.clip = clip;
    }

    protected override void Update()
    {
        base.Update();

        hide = (value < 1);

        Tks.OnValueChanged((p) =>
        {
            if (!p)
            {
                //animate
                vignette.alpha = 1;

                //camera
                camAnimated.localRotation *= Quaternion.Euler(Vector3.left * (pop / 2) + Vector3.forward * (pop));

                //volume
                vol.weight = 1;

                //sound

                audioSrc.Play(); //play stun sound
            }
        }, value, "Health");

        AnimateGui();
    }

    protected override void AnimateGui()
    {
        base.AnimateGui();

        //vignette
        vignette.alpha = Mathf.Lerp(vignette.alpha, 0.0f, 0.05f);

        //camera
        camAnimated.localRotation = Quaternion.Lerp(camAnimated.localRotation, Quaternion.identity, 0.175f);

        //backdrop
        if (value < 0.375f)
            StartCoroutine(Tks.FlickerImg(backdrop.gameObject, 100));

        //volume
        vol.weight = Mathf.Lerp(vol.weight, 0, 0.005f);

        //sound

        if (track == null) {
            Debug.LogWarning("`Track` is null! Health can not affect soundtrack pitch.");
            return;
        }

        track.pitch = (1 - (vol.weight / 2)) * (Menu.paused ? 0 : 1);  //health effect soundtrack speed and stop if time paused
    }
}
