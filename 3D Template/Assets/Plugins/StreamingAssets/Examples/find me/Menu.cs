using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{

    public static bool paused;

    [SerializeField] Health health;

    [Header("Gui")]

    [SerializeField] TextMeshProUGUI header;

    [SerializeField] string[] innerText = new string[2];
    int headerIndex;

    int quitQuestion;

    [Space(10)]

    [SerializeField] CanvasGroup group;

    [SerializeField] Color[] backdropColor = new Color[2];

    [Space(10)]

    [SerializeField] CanvasGroup btnRapper;

    [Header("Animate")]

    [SerializeField] float pop;
    [SerializeField] float dur = 0.5f;

    [Header("Cheat")]

    Dictionary<string, string> userCombo = new Dictionary<string, string>();
    Dictionary<string, float> inputTimer = new Dictionary<string, float>();


    void Awake()
    {
        innerText[0] = header.text;

        quitQuestion = 0;

        Clean();
    }

    void Update()
    {
        paused = (health.value == 0 && Time.timeScale != 0) ? true : paused; //force pause

        if (health.value == 0 && headerIndex == 0)
            Loser();
        else if (health.value > 0 && headerIndex == 1)
            Clean();

        //Gui

        header.text = innerText[headerIndex];

        group.alpha = (paused) ? 1.0f : 0.0f;
        group.blocksRaycasts = paused;

        //Pause

        Time.timeScale = (!paused) ? 1.0f : 0.0f;

        if ((Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) && !WeaponSelect.wheelEnabled) {
            paused = !paused;
            UpdateValues();
        }


        //Cheat

        ListenForCheat(() => { WeaponSelect.equipped.ammo = WeaponSelect.equipped.ammoLimit; }, "fullclip");
    }



    void Loser()
    {
        headerIndex = 1;
        UpdateValues();

        group.GetComponent<Image>().color = backdropColor[1];

        LeanTween.value(0.0f, 1.0f, 0.5f).setOnUpdate((a) => header.color = new Color(1, 1, 1, a))
            .setIgnoreTimeScale(true);

        LeanTween.cancel(header.gameObject);
        header.transform.localScale = Vector3.one * (3.5f + pop);

        LeanTween.scale(header.gameObject, Vector3.one * 3.5f, dur)
            .setEaseInCubic()
            .setIgnoreTimeScale(true);

        header.transform.localRotation = Quaternion.Euler(Vector3.forward * (7.5f * Tks.GetRandomSign()));

        btnRapper.blocksRaycasts = false;
        btnRapper.alpha = 0.0f;
        StartCoroutine(Tks.SetTimeout(() => LeanTween.value(0.0f, 1.0f, 0.5f).setOnUpdate((a) => btnRapper.alpha = a).setOnComplete(() => btnRapper.blocksRaycasts = true).setIgnoreTimeScale(true), 750, true));
    }

    void Clean()
    {
        headerIndex = 0;
        UpdateValues();

        group.GetComponent<Image>().color = backdropColor[0];

        header.color = Color.white;
        header.transform.localScale = Vector3.one * 3.5f;

        header.transform.localRotation = Quaternion.identity;

        btnRapper.alpha = 1;
        btnRapper.blocksRaycasts = true;
    }


    void UpdateValues()
    {
        Tks.cursorState = (paused) ? CursorLockMode.None : CursorLockMode.Locked;
        Freelook.freelook = !paused;
        Freeroam.freeroam = !paused;

        quitQuestion = 0;
    }

    //Buttons

    public void Continue()
    {
        if (health.value > 0) {
            paused = false;
            UpdateValues();
        }


        if (health.value > 0) return;

        TextMeshProUGUI label = btnRapper.transform.GetChild(0).Find("Label").GetComponent<TextMeshProUGUI>();
        label.text = "You can not revive!";
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);

        paused = false;
        UpdateValues();
    }

    public void Exit()
    {
        quitQuestion++;

        TextMeshProUGUI label = btnRapper.transform.GetChild(2).Find("Label").GetComponent<TextMeshProUGUI>();

        if (quitQuestion > 0) {
            label.text += " (Click again)";
        }

        if (quitQuestion < 2) return;

        print("Close application");
        Application.Quit();
    }


    //Cheat

    void ListenForCheat(System.Action action, string targetCombo)
    {
        if (!inputTimer.ContainsKey(targetCombo))
            inputTimer.Add(targetCombo, 0);

        inputTimer[targetCombo] += Time.unscaledDeltaTime;

        if (inputTimer[targetCombo] > 0.75f)
            userCombo[targetCombo] = "";

        if (Input.inputString.Length > 0)
        {
            if (!userCombo.ContainsKey(targetCombo))
                userCombo.Add(targetCombo, "");

            userCombo[targetCombo] += Input.inputString;

            bool correctInput = userCombo[targetCombo][userCombo[targetCombo].Length - 1] == targetCombo[userCombo[targetCombo].Length - 1];
            bool validCombo = (userCombo[targetCombo] == targetCombo);

            inputTimer[targetCombo] = (correctInput) ? 0 : inputTimer[targetCombo];

            if (!correctInput && !validCombo)
                userCombo[targetCombo] = "";

            //Gift
            if (validCombo)
            {
                userCombo[targetCombo] = "";

                print("Cheat activated"); //inform
                action.Invoke();
            }
        }
    }
}