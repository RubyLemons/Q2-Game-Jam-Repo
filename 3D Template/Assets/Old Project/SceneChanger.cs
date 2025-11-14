using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{

    [SerializeField] private string Level1 = "Level";
    public void Start()
    {
        
    }
    public void NewGameButton()
    {
        SceneManager.LoadScene(1);
    }

}