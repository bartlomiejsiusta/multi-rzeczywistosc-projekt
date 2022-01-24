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
    [SerializeField] private TMP_Dropdown shipSizeDropdown;

    public GameState gameState;
    public enum GameState
    {
        Initial,
        EnteredGame,
        GameActive
    }


    public Dictionary<int, int> availableShips = new Dictionary<int, int>();
    int currentCount;
    int placedShips = 0;


    public int[][][] mapState;
    void Start()
    {
        availableShips.Add(2, 2);
        availableShips.Add(3, 2);
        availableShips.Add(4, 2);

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

        InvokeRepeating("updateGameStateAndMapState", 2.0f, 2.0f);
    }
        
    

    // Update is called once per frame
    void Update()
    {
        //if (gameState == GameState.Initial)
        //{
        //   // shipSizeDropdown.gameObject.SetActive(true);
        //}
    }


    void updateGameStateAndMapState()
    {
        var gameId = GameManager.Instance.gameName;
        StartCoroutine(GetMapState(gameId));
    }

    public void SendCoordinatesToServer_Event()
    {
        string coordinate = inputTileToShot.text;
        int selectedShip;
        
        Debug.Log(coordinate);
        if(gameState == GameState.GameActive)
        {
            StartCoroutine(SendCoordinatesToServer(coordinate));
        }
        else if(gameState == GameState.Initial)
        {   
            if(placedShips <= 6)
            {
                Debug.Log(string.Join(", ", availableShips.Values));
                //selectedShip = shipSizeDropdown.value;
                selectedShip = Int32.Parse(shipSizeDropdown.options[shipSizeDropdown.value].text);
                Debug.Log(selectedShip);
                availableShips.TryGetValue(selectedShip, out currentCount);
                Debug.Log(currentCount);
                availableShips[selectedShip] = currentCount - 1;

                if (currentCount < 1)
                {
                    // update dropdown text?
                    availableShips.Remove(selectedShip);
                    shipSizeDropdown.options.RemoveAt(shipSizeDropdown.value);

                }
                Debug.Log(string.Join(", ", availableShips.Values));

                StartCoroutine(PlaceShip(selectedShip, coordinate));
                placedShips++;
            }
            //else
            //{
                // jezeli rozstawiono wszystkie statki zmien stan gry?
            //}
            
        }

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

    IEnumerator PlaceShip(int shipSize, string coordinate)
    {
        Debug.Log("Placing ship at " + coordinate);
       
        Debug.Log(GameManager.Instance.gameName);
        Debug.Log(GameManager.Instance.playerID.ToString());
        WWWForm form = new WWWForm();
        form.AddField("shipSize", shipSize);
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", GameManager.Instance.gameName);
        form.AddField("playerId", GameManager.Instance.playerID.ToString());

        UnityWebRequest uwr = UnityWebRequest.Post(CommunicationButtons.PLACE_SHIP_ENDPOINT, form);
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

    IEnumerator GetMapState(string gameId)
    {

        UnityWebRequest uwr = UnityWebRequest.Get(CommunicationButtons.GET_MAP_STATE_ENDPOINT + "?gameId=" + gameId);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Result: " + uwr.downloadHandler.text);
            mapState = Newtonsoft.Json.JsonConvert.DeserializeObject<int[][][]>(uwr.downloadHandler.text);
        }
    }
}
