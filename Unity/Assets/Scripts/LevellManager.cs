using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LevellManager : MonoBehaviour
{
    public GameObject shotMenu;
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        // shotManu powinno sie pokazywac tylko jesli jest nasza tura
       // shotMenu.SetActive(false);
    }
}
