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

    #if UNITY_EDITOR
    [SerializeField] private bool overrideIDs;
    [SerializeField] private string gameID;
    [SerializeField] private string playerGuid;
    #endif

    private GameState gameState;
    public enum GameState
    {
        PlacingShips = 0,
        HostTurn = 1,
        GuestTurn = 2
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
            case GameState.PlacingShips:
                gameStateText.text = "Faza rozstawiania";
                break;
            case GameState.HostTurn:
                gameStateText.text = "Faza hosta";
                break;
            case GameState.GuestTurn:
                gameStateText.text = "Faza goœcia";
                break;
        }

        StartCoroutine(GetMapState());
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
        StartCoroutine(GetMapState());
    }

    public void SendCoordinatesToServer_Event()
    {
        string coordinate = inputTileToShot.text;
        int selectedShip;
        
        Debug.Log(coordinate);
        if(gameState != GameState.PlacingShips)
        {
            StartCoroutine(SendCoordinatesToServer(coordinate));
        }
        else
        {   
            if (placedShips <= 6)
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
        string gameID = GameManager.Instance.gameName;
        #if UNITY_EDITOR
        if (overrideIDs)
        {
            gameID = this.gameID;
        }
        #endif
        UnityWebRequest uwr = UnityWebRequest.Get(CommunicationButtons.CURRENT_GAME_STATE + "?gameId=" + gameID);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("GameState: " + uwr.downloadHandler.text);
            gameState = (GameState)int.Parse(uwr.downloadHandler.text);
        }
    }

    IEnumerator SendCoordinatesToServer(string coordinate)
    {
        string gameID = GameManager.Instance.gameName;
        string playerGuid = GameManager.Instance.playerID.ToString();
        #if UNITY_EDITOR
        if (overrideIDs)
        {
            gameID = this.gameID;
            playerGuid = this.playerGuid;
        }
        #endif

        Debug.Log($"Sending coordinates, {coordinate}, {gameID}, {playerGuid}");
        WWWForm form = new WWWForm();
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", gameID);
        form.AddField("playerId", playerGuid);

        UnityWebRequest uwr = UnityWebRequest.Post(CommunicationButtons.COORDINATES_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Success: " + uwr.downloadHandler.text);
        }
    }

    IEnumerator PlaceShip(int shipSize, string coordinate)
    {
        string gameID = GameManager.Instance.gameName;
        string playerGuid = GameManager.Instance.playerID.ToString();
        #if UNITY_EDITOR
        if (overrideIDs)
        {
            gameID = this.gameID;
            playerGuid = this.playerGuid;
        }
        #endif
        Debug.Log($"Placing ship at {coordinate}, {gameID}, {playerGuid}");
        WWWForm form = new WWWForm();
        form.AddField("shipSize", shipSize);
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", gameID);
        form.AddField("playerId", playerGuid);

        UnityWebRequest uwr = UnityWebRequest.Post(CommunicationButtons.PLACE_SHIP_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Error: " + uwr.error);
        }
        else
        {
            Debug.Log("Success: " + uwr.downloadHandler.text);
        }
    }

    IEnumerator GetMapState()
    {
        var wait = new WaitForSeconds(2.0f);
        while (true)
        {
            string gameID = GameManager.Instance.gameName;
            #if UNITY_EDITOR
            if (overrideIDs)
            {
                gameID = this.gameID;
            }
            #endif

            UnityWebRequest uwr = UnityWebRequest.Get(CommunicationButtons.GET_MAP_STATE_ENDPOINT + "?gameId=" + gameID);
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error: " + uwr.error);
            }
            else
            {
                //Debug.Log("Result: " + uwr.downloadHandler.text);
                mapState = Newtonsoft.Json.JsonConvert.DeserializeObject<int[][][]>(uwr.downloadHandler.text);
            }

            yield return wait;
        }
    }
}
