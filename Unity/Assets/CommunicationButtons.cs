﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System;

public class CommunicationButtons : MonoBehaviour
{
    public const string URL_BASE = "https://localhost:44386/Game";
    public const string REGISTER_URL_ENDPOINT = URL_BASE + "/Register";
    public const string CREATEGAME_URL_ENDPOINT = URL_BASE + "/Create";
    public const string ENTERGAME_URL_ENDPOINT = URL_BASE + "/Enter";
    public const string COORDINATES_URL_ENDPOINT = URL_BASE + "/PostCoordinates";

    public const string CURRENT_GAME_STATE = URL_BASE + "/CurrentGameState";

    public string ActiveGameName = "";
    public Guid PlayerId;

    public enum GameState
    {
        Initial,
        EnteredGame,
        GameActive
    }

    public GameState CurrentGameState = GameState.Initial;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(EnterServer());
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(GetCurrentGameState());
        // jezeli stan = rozstawianie statkow
        if (CurrentGameState == GameState.Initial)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 clickPosition = -Vector3.one;
                // clickPosition = CharEnumerator.main.ScreenToWorldPoint(IndexOutOfRangeException.mousePosition = new Vector3 (0, 0, 5));
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    clickPosition = hit.point;
                }
                Debug.Log(clickPosition);
                // zrobic konwersje pozycji na wspolrzedne
                string coordinate = clickPosition.ToString();
                StartCoroutine(SendCoordinatesToServer(coordinate));
            }
        }

    }

    /// <summary>
    /// Funkcja wywoływana przez przycisk
    /// </summary>
    public void SendCoordinatesToServer_Event()
    {
        string coordinate = "A2";
        StartCoroutine(SendCoordinatesToServer(coordinate));
    }

    public void CreateRoom_Event()
    {
        StartCoroutine(CreateGame());
    }

    public void EnterRoom_Event()
    {
        StartCoroutine(EnterExistingGame("ABCDE10"));
    }

    public void ExitGame_Event()
    {
        Application.Quit();
        Debug.Log("Exiting application");
    }

    /// <summary>
    /// Dołączenie do gry na serwerze
    /// </summary>
    /// <returns></returns>
    IEnumerator EnterServer()
    {
        Guid playerIdentifier = Guid.NewGuid();
        Debug.Log($"Trying to enter server with id {playerIdentifier}");

        WWWForm form = new WWWForm();
        form.AddField("claimedId", playerIdentifier.ToString());

        UnityWebRequest uwr = UnityWebRequest.Post(REGISTER_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            PlayerId = playerIdentifier;
            Debug.Log("Result: " + uwr.downloadHandler.text);
        }
    }

    /// <summary>
    /// Utworzenie gry
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateGame()
    {
        Debug.Log("Creating game");

        WWWForm form = new WWWForm();

        UnityWebRequest uwr = UnityWebRequest.Post(CREATEGAME_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            ActiveGameName = uwr.downloadHandler.text;
            CurrentGameState = GameState.EnteredGame;

            Debug.Log("Result: " + ActiveGameName);
        }
    }

    /// <summary>
    /// Dołączenie do istniejącej gry
    /// </summary>
    /// <returns></returns>
    IEnumerator EnterExistingGame(string gameId)
    {
        Debug.Log($"Entering game '{gameId}'");

        WWWForm form = new WWWForm();
        form.AddField("playerId", PlayerId.ToString());
        form.AddField("gameId", gameId);

        UnityWebRequest uwr = UnityWebRequest.Post(ENTERGAME_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            ActiveGameName = uwr.downloadHandler.text;
            CurrentGameState = GameState.GameActive;

            Debug.Log("Result: " + ActiveGameName);
        }
    }

    /// <summary>
    /// Wysłane koordynatów do serwera
    /// </summary>
    /// <returns></returns>
    IEnumerator SendCoordinatesToServer(string coordinate)
    {
        Debug.Log("Sending coordinates");

        WWWForm form = new WWWForm();
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", ActiveGameName);
        form.AddField("playerId", PlayerId.ToString());

        UnityWebRequest uwr = UnityWebRequest.Post(COORDINATES_URL_ENDPOINT, form);
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


    IEnumerator GetCurrentGameState()
    {

        String gameId = "abc";
        UnityWebRequest uwr = UnityWebRequest.Get(CURRENT_GAME_STATE + "?gameId=" + gameId);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("GameState: " + uwr.downloadHandler.text);

            switch (Int32.Parse(uwr.downloadHandler.text))
            {
                case 0:
                    CurrentGameState = GameState.Initial;
                    break;
                case 1:
                    CurrentGameState = GameState.EnteredGame;
                    break;
                case 2:
                    CurrentGameState = GameState.GameActive;
                    break;
            }
        }
    }
}
