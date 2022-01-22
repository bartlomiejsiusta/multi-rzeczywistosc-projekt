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
    [SerializeField] private TMP_InputField inputTileToShot;


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

    public void SendCoordinatesToServer_Event()
    {
        string coordinate = inputTileToShot.text;
        Debug.Log(coordinate);
        StartCoroutine(SendCoordinatesToServer(coordinate));
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

    IEnumerator SendCoordinatesToServer(string coordinate)
    {
        Debug.Log("Sending coordinates");
        Debug.Log(coordinate);
        Debug.Log(GameManager.Instance.gameName);
        Debug.Log(GameManager.Instance.playerID.ToString());
        WWWForm form = new WWWForm();
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", GameManager.Instance.gameName);
        form.AddField("playerId", GameManager.Instance.playerID.ToString());

        UnityWebRequest uwr = UnityWebRequest.Post(CommunicationButtons.COORDINATES_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Result: " + uwr.downloadHandler.text);
        }
    }
}
