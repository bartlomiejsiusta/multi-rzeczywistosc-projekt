using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private TMP_Text gameStateText;
    [SerializeField] private TMP_InputField inputTileToShot;
    [SerializeField] private TMP_Dropdown shipSizeDropdown;

    [SerializeField] private float stateQueryTime = 3.0f;

    #if UNITY_EDITOR
    [SerializeField] private bool overrideIds;
    [SerializeField] private string gameId;
    [SerializeField] private string playerId;
    #endif

    private GameState gameState;
    public enum GameState
    {
        PlacingShips = 0,
        HostTurn = 1,
        GuestTurn = 2,
    }

    private Dictionary<int, int> availableShips = new Dictionary<int, int>();
    private int currentCount;
    private int placedShips;

    private int[][][] mapState;

    private void Awake()
    {
        availableShips.Add(2, 2);
        availableShips.Add(3, 2);
        availableShips.Add(4, 2);

        StartCoroutine(GetCurrentGameState());
        StartCoroutine(GetMapState());
    }

    public void SendCoordinatesToServer_Event()
    {
        string coordinate = inputTileToShot.text;

        if (gameState != GameState.PlacingShips)
        {
            StartCoroutine(SendCoordinatesToServer(coordinate));
        }
        else
        {   
            if (placedShips <= 6)
            {
                int selectedShip = int.Parse(shipSizeDropdown.options[shipSizeDropdown.value].text);
                availableShips.TryGetValue(selectedShip, out currentCount);
                availableShips[selectedShip] = currentCount - 1;

                if (currentCount < 1)
                {
                    // update dropdown text?
                    availableShips.Remove(selectedShip);
                    shipSizeDropdown.options.RemoveAt(shipSizeDropdown.value);
                }

                StartCoroutine(PlaceShip(selectedShip, coordinate));
                placedShips++;
            }
        }
    }

    IEnumerator GetCurrentGameState()
    {
        string gameId = GameManager.Instance.gameName;
        #if UNITY_EDITOR
        if (overrideIds)
        {
            gameId = this.gameId;
        }
        #endif

        Debug.Log($"Getting game state {gameId}");

        UnityWebRequest uwr = UnityWebRequest.Get(CommunicationButtons.CURRENT_GAME_STATE + "?gameId=" + gameId);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            gameState = (GameState)int.Parse(uwr.downloadHandler.text);
            Debug.Log("Success getting game state: " + gameState.ToString());

            switch (gameState)
            {
            case GameState.PlacingShips:
                gameStateText.text = "Faza rozstawiania";
                break;
            case GameState.HostTurn:
                gameStateText.text = "Tura hosta";
                break;
            case GameState.GuestTurn:
                gameStateText.text = "Tura goœcia";
                break;
            }
        }
    }

    IEnumerator SendCoordinatesToServer(string coordinate)
    {
        string gameId = GameManager.Instance.gameName;
        string playerId = GameManager.Instance.playerID.ToString();
        #if UNITY_EDITOR
        if (overrideIds)
        {
            gameId = this.gameId;
            playerId = this.playerId;
        }
        #endif

        Debug.Log($"Sending coordinates, {coordinate}, {gameId}, {playerId}");

        WWWForm form = new WWWForm();
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", gameId);
        form.AddField("playerId", playerId);

        UnityWebRequest uwr = UnityWebRequest.Post(CommunicationButtons.COORDINATES_URL_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            Debug.Log("Success sending coordinates: " + uwr.downloadHandler.text);
        }
    }

    IEnumerator PlaceShip(int shipSize, string coordinate)
    {
        string gameId = GameManager.Instance.gameName;
        string playerId = GameManager.Instance.playerID.ToString();
        #if UNITY_EDITOR
        if (overrideIds)
        {
            gameId = this.gameId;
            playerId = this.playerId;
        }
        #endif

        Debug.Log($"Placing ship at {coordinate}, {gameId}, {playerId}");

        WWWForm form = new WWWForm();
        form.AddField("shipSize", shipSize);
        form.AddField("coordinate", coordinate);
        form.AddField("gameId", gameId);
        form.AddField("playerId", playerId);

        UnityWebRequest uwr = UnityWebRequest.Post(CommunicationButtons.PLACE_SHIP_ENDPOINT, form);
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
        }
        else
        {
            Debug.Log("Success placing ship: " + uwr.downloadHandler.text);
        }
    }

    IEnumerator GetMapState()
    {
        WaitForSeconds wait = new WaitForSeconds(stateQueryTime);
        while (true)
        {
            string gameId = GameManager.Instance.gameName;
            #if UNITY_EDITOR
            if (overrideIds)
            {
                gameId = this.gameId;
            }
            #endif

            Debug.Log($"Getting map state {gameId}");

            UnityWebRequest uwr = UnityWebRequest.Get(CommunicationButtons.GET_MAP_STATE_ENDPOINT + "?gameId=" + gameId);
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"{uwr.error}\n{uwr.downloadHandler.text}");
            }
            else
            {
                Debug.Log("Success downloading map state");
                mapState = Newtonsoft.Json.JsonConvert.DeserializeObject<int[][][]>(uwr.downloadHandler.text);
            }

            yield return wait;
        }
    }
}
