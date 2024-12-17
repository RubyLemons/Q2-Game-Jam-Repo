using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Menu : MonoBehaviour
{

    static bool paused;

    [Header("Gui")]

    [SerializeField] TextMeshProUGUI header;

    [SerializeField] string[] innerText = new string[2];
    static int headerIndex;

    void Awake()
    {
        innerText[0] = header.text;
    }

    void Update()
    {
        header.text = innerText[headerIndex];


        //Pause

        Time.timeScale = (!paused) ? 1.0f : 0.0f;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) {
            paused = !paused;
        }
    }
}
