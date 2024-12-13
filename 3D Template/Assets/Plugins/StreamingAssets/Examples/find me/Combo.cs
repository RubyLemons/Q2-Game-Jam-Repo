using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Combo : BarController
{
    [Header("Animated tile")]

    [SerializeField] Transform[] aniamted = new Transform[3];

    [SerializeField] float dur = 0.35f;

    [Header("Aniamted visuals")]

    [SerializeField] Volume vol;

    float volLerp;
    float flickerVol;



    protected override void Awake()
    {
        base.Awake();

        Aniamted();

        StartCoroutine(AnimatedVolume());
    }

    protected override void Update()
    {
        hide = (value > 0);

        base.Update();

        Tks.OnValueChanged((p) => 
        {
            if (p)
            {
                //aniamte
                label.transform.localScale += Vector3.one * pop;
                label.transform.localRotation = Quaternion.Euler(label.transform.localRotation.eulerAngles + Vector3.forward * (pop * 10 * Tks.GetRandomSign()));
            }
        }, value, "Combo");

        //debug
        if (Input.GetKeyDown(KeyCode.F))
            value += 0.01f;
    }

    protected override void AnimateGui()
    {
        base.AnimateGui();

        flickerVol = Mathf.Lerp(flickerVol, 0, 0.325f);

        volLerp = Mathf.Lerp(volLerp, Mathf.Clamp01(value + (value > 0 ? 0.65f : 0)), 0.025f);
        vol.weight = Mathf.Clamp01(volLerp + ((volLerp > (1 - flickerVol)) ? -flickerVol : flickerVol));
    }


    IEnumerator AnimatedVolume()
    {
        while (true)
        {
            if (value > 0)
            {
                flickerVol = 0.25f;
            }

            yield return new WaitForSeconds(0.15f - (value / 25));
        }
    }

    //run once
    private void Aniamted()
    {
        LeanTween.cancel(aniamted[0].gameObject);
        LeanTween.move(aniamted[0].gameObject, aniamted[2].position, dur)
            .setOnComplete(() => {
                aniamted[0].position = aniamted[1].position;
                Aniamted();
            });
    }
}
