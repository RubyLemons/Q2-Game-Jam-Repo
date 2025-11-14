using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CREDITS : MonoBehaviour
{

    [SerializeField] private string Level1 = "CREDITS";
    public void Start()
    {

    }
    public void NewGameButton2()
    {
        SceneManager.LoadScene(2);
    }

}