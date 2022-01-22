using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance 
    { 
        get { 
            if(instance == null)
            {
                //instance = FindObjectOfType<GameManager>();
                var newGameObject = new GameObject();
                instance = newGameObject.AddComponent<GameManager>();
            }
            return instance;
        }
    }
    private static GameManager instance;

    public string gameName;
    public Guid playerID;

    //[SerializeField] private string mainScene;

    private void Awake()
    {
  
        DontDestroyOnLoad(this);
        //SceneManager.LoadScene(mainScene);
    }


}
