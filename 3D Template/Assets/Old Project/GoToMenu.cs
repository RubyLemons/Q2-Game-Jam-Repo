using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToMenu : MonoBehaviour
{

    [SerializeField] private string Level1 = "CREDITS";
    public void Back2Home()
    {
        SceneManager.LoadScene(0);
    }

}
