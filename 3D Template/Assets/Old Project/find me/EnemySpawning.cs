using System.Collections;
using System.Collections.Generic;
using static Tks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[System.Serializable]
public class Chance
{
    [Range(0, 1)] public float first = 0.5f;
    [Range(0, 1)] public float second = 0.5f;
    [Range(0, 1)] public float third = 0.5f;
}

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] Freelook freelook;

    [Header("Spawning")]

    [SerializeField] Transform points;

    [SerializeField] GameObject[] enemy;
    int index;
    [SerializeField] Chance chance;

    public Transform enemyContainer;

    [Space(10)]

    [Range(0, 30)] [SerializeField] float amount = 2;
    [Range(0, 4)][SerializeField] float multiplier = 1.25f;

    [Space(10)]

    public int wave;
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
                    .setOnUpdate((v) => { if (headerInfoWrapper != null) headerInfoWrapper.padding = new Vector4(0, 0, v, 0); })
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
                StartCoroutine(FlickerImg(header.gameObject, 150));
                StartCoroutine(FlickerImg(label.gameObject, 150));
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

        #region --Choose enemy type by `Chance`
        if (TestChance(chance.first)) {
            //SPAWN "FISRT"
            index = 0;
        }
        else if (TestChance(chance.second)) {
            //SPAWN "SPRINTER"
            index = 1;
        }
        else if (TestChance(chance.third)) {
            //SPAWN "THIRD"
            index = 2;
        }
        else {
            //SPAWN DEFAULT
            index = 1;
        }
        #endregion

        GameObject newEnemy = Instantiate(enemy[index], enemyContainer);
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
