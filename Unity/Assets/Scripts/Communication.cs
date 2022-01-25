using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public static class Communication
{
    private const string URL_BASE = "http://shipsgameserver.azurewebsites.net/Game";
    private const string REGISTER_URL_ENDPOINT = URL_BASE + "/Register";
    private const string CREATEGAME_URL_ENDPOINT = URL_BASE + "/Create";
    private const string ENTERGAME_URL_ENDPOINT = URL_BASE + "/Enter";
    private const string COORDINATES_URL_ENDPOINT = URL_BASE + "/PostCoordinates";
    private const string PLACE_SHIP_ENDPOINT = URL_BASE + "/PlaceShip";
    private const string GET_MAP_STATE_ENDPOINT = URL_BASE + "/MapState";
    private const string CURRENT_GAME_STATE = URL_BASE + "/CurrentGameState";

    public static IEnumerator Register(string claimedId)
    {
        Debug.Log($"Register with GUID {claimedId}");

        WWWForm form = new WWWForm();
        form.AddField("claimedId", claimedId);

        UnityWebRequest uwr = UnityWebRequest.Post(REGISTER_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            Debug.Log("Success");
        }
    }

    public static IEnumerator Create(Action<string> onGameId)
    {
        Debug.Log("Create game");

        UnityWebRequest uwr = UnityWebRequest.Post(CREATEGAME_URL_ENDPOINT, new WWWForm());
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            string gameId = uwr.downloadHandler.text;
            onGameId(gameId);
            Debug.Log($"Success, created game {gameId}");
        }
    }

    public static IEnumerator Enter(string playerId, string gameId)
    {
        Debug.Log($"Entering game {gameId} as player {playerId}");

        WWWForm form = new WWWForm();
        form.AddField("playerId", playerId);
        form.AddField("gameId", gameId);

        UnityWebRequest uwr = UnityWebRequest.Post(ENTERGAME_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            Debug.Log("Success");
        }
    }

    public static IEnumerator PlaceShip(string coordinate, string gameId, string playerId, int shipSize)
    {
        Debug.Log($"Placing ship at {coordinate} of size {shipSize} for game {gameId} as player {playerId}");

        WWWForm form = new WWWForm();
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", gameId);
        form.AddField("playerId", playerId);
        form.AddField("shipSize", shipSize);

        UnityWebRequest uwr = UnityWebRequest.Post(PLACE_SHIP_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            Debug.Log("Success");
        }
    }

    public static IEnumerator PostCoordinates(string coordinate, string gameId, string playerId)
    {
        Debug.Log($"Posting coordinates {coordinate} for game {gameId} as player {playerId}");

        WWWForm form = new WWWForm();
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", gameId);
        form.AddField("playerId", playerId);

        UnityWebRequest uwr = UnityWebRequest.Post(COORDINATES_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            Debug.Log("Success");
        }
    }

    public static IEnumerator GetMapState(string gameId, Action<int[][][]> onMapState)
    {
        Debug.Log($"Getting map state for game {gameId}");

        UnityWebRequest uwr = UnityWebRequest.Get($"{GET_MAP_STATE_ENDPOINT}?gameId={gameId}");
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            int[][][] mapState = Newtonsoft.Json.JsonConvert.DeserializeObject<int[][][]>(uwr.downloadHandler.text);
            onMapState(mapState);
            Debug.Log("Success getting map state");
        }
    }

    public static IEnumerator GetCurrentGameState(string gameId, Action<GameState> onCurrentGameState)
    {
        Debug.Log($"Getting current game state for game {gameId}");

        UnityWebRequest uwr = UnityWebRequest.Get($"{CURRENT_GAME_STATE}?gameId={gameId}");
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            GameState currentGameState = (GameState)int.Parse(uwr.downloadHandler.text);
            onCurrentGameState(currentGameState);
            Debug.Log($"Success getting current game state, {currentGameState}");
        }
    }
}
