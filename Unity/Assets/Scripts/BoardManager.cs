using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BoardManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private TMP_Text gameStateText;

    public GameState gameState;
    public enum GameState
    {
        Initial,
        EnteredGame,
        GameActive
    }

    void Start()
    {
       
        StartCoroutine(GetCurrentGameState());
            switch (gameState)
            {
                case GameState.Initial:
                    gameStateText.text = "Faza rozstawiania";
                    break;
                case GameState.EnteredGame:
                    gameStateText.text = "Faza hosta";
                    break;
                case GameState.GameActive:
                    gameStateText.text = "Faza goœcia";
                    break;
            }
    }
        
    

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GetCurrentGameState()
    {

        String gameId = GameManager.Instance.gameName;
        UnityWebRequest uwr = UnityWebRequest.Get(CommunicationButtons.CURRENT_GAME_STATE + "?gameId=" + gameId);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("GameState: " + uwr.downloadHandler.text);
            gameState = (GameState)Int32.Parse(uwr.downloadHandler.text);
        }
    }
}
