using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] Freelook freelook;

    [Header("Spawning")]

    [SerializeField] Transform points;
    [SerializeField] GameObject enemy;

    [SerializeField] Transform enemyContainer;

    [Space(10)]

    [Range(0, 30)] [SerializeField] float amount = 2;
    [Range(0, 4)][SerializeField] float multiplier = 1.25f;

    [Space(10)]

    [SerializeField] int wave;
    [SerializeField] int maxWave = 5;

    [SerializeField] float waveTime = 60 + 30;
    float waveSeconds;

    bool deb;

    [Header("Gui")]

    [SerializeField] TextMeshProUGUI label;

    [Space(10)]

    [SerializeField] TextMeshProUGUI header;
    [SerializeField] string[] waveMessage = new string[2] { "Next wave starting", "Final wave starting" };
    [SerializeField] int range = 675;

    [SerializeField] RectMask2D headerInfoWrapper;

    void Awake()
    {
        waveSeconds = waveTime;

        headerInfoWrapper.padding = new Vector4(0, 0, range, 0);
    }


    void Update()
    {
        wave = Mathf.Clamp(wave, 0, maxWave);

        waveSeconds = (waveSeconds > 0) ? waveSeconds - Time.deltaTime : 0; //timer

        headerInfoWrapper.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = waveMessage[(wave < maxWave - 1) ? 0 : 1];

        AnimateGui();

        if (waveSeconds <= 0 || enemyContainer.childCount == 0)
        {
            if (deb) return;

            AnimateNewRound(() => {

                wave++;
                waveSeconds = waveTime;

                for (int i = 0; i < Mathf.CeilToInt(amount); i++)
                {
                    ProduceEnemy();
                }
            });

            amount *= multiplier;
        }

    }

    void AnimateNewRound(System.Action action)
    {
        deb = true;

        //fade out main headser

        LeanTween.value(1.0f, 0.0f, 0.5f)
            .setOnUpdate((a) => header.color = new Color(1, 1, 1, a))
            .setOnComplete(() =>
            {
                //animate secondaray header

                LeanTween.value(range, 0, 1.25f)
                    .setOnUpdate((v) => headerInfoWrapper.padding = new Vector4(0, 0, v, 0))
                    .setEaseOutCubic()
                    .setOnComplete(() =>
                    {
                        //flicker main header
                        StartCoroutine(FlickerAnimationFrame());
                    });
            });

        IEnumerator FlickerAnimationFrame(int delay = 2750)
        {
            yield return new WaitForSeconds(delay / 1000);

            headerInfoWrapper.padding = new Vector4(0, 0, range, 0);
            header.color = Color.white;

            //new round

            action.Invoke();

            float elaspedTime = 0;

            while (elaspedTime < 2.5f) {
                elaspedTime += Time.deltaTime;
                yield return null;
                StartCoroutine(Tks.FlickerImg(header.gameObject, 150));
                StartCoroutine(Tks.FlickerImg(label.gameObject, 150));
            }

            deb = false;
        }
    }

    void AnimateGui()
    {
        //lazy timer visuals
        string seconds = Mathf.FloorToInt(((waveSeconds < 60) ? waveSeconds : waveSeconds - 60)).ToString();
        string minutes = Mathf.FloorToInt(((waveSeconds > 60) ? 1 : 0)).ToString();

        label.text = minutes + ":" + ((seconds.Length > 1) ? seconds : "0" + seconds);

        //text

        header.text = "Wave "+ wave;
    }



    void ProduceEnemy()
    {
        //produce
        Transform spawnPoint = points.GetChild(Random.Range(0, points.childCount - 1));

        GameObject newEnemy = Instantiate(enemy, enemyContainer);
        newEnemy.SetActive(false);

        //position
        newEnemy.transform.position = spawnPoint.position;

        //release
        StartCoroutine(WaitFor(() => newEnemy.SetActive(true), spawnPoint));
    }

    bool NotFacing(Transform target)
    {
        Vector3 normalizedDist = (freelook.cam.transform.position - target.position).normalized;
        float dotProduct = Vector3.Dot(normalizedDist, freelook.cam.transform.forward);

        return (dotProduct > 0);
    }

    IEnumerator WaitFor(System.Action action, Transform point)
    {
        while (!NotFacing(point)) {
            yield return null;
        }

        action.Invoke();
    }
}
