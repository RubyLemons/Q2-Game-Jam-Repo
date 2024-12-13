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
            }
        }, value, "Health");

        AnimateGui();

        //DEBUG
        if (Input.GetKeyDown(KeyCode.H))
            value -= 0.1f;
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
            StartCoroutine(Tks.FlickerImg(backdrop, 100));

        //volume
        vol.weight = Mathf.Lerp(vol.weight, 0, 0.005f);
    }
}
