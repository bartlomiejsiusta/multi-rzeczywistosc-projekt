using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tales;

    [SerializeField] private TMP_Text gameStateText;
    [SerializeField] private TMP_Text gameIdText;
    [SerializeField] private TMP_InputField coordinateInput;
    [SerializeField] private TMP_Dropdown shipSizeDropdown;
    [SerializeField] private Button fireButton;

    [SerializeField] private float stateQueryTime = 3.0f;

    #if UNITY_EDITOR
    [Header("For debugging"), SerializeField] private bool overrideIds;
    [SerializeField] private string gameId;
    [SerializeField] private string playerId;
    #endif

    private WaitForSeconds wait;

    private GameState gameState;
    private int[][][] mapState;

    private void Awake()
    {
        wait = new WaitForSeconds(stateQueryTime);

        gameIdText.text = GameManager.gameId;

        StartCoroutine(GetCurrentGameStateCoroutine());
        StartCoroutine(GetMapStateCoroutine());

        fireButton.onClick.AddListener(() => StartCoroutine(Fire()));
    }

    IEnumerator GetCurrentGameStateCoroutine()
    {
        while (true)
        {
            string gameId = GameManager.gameId;
            #if UNITY_EDITOR
            if (overrideIds)
            {
                gameId = this.gameId;
            }
            #endif

            yield return Communication.GetCurrentGameState(gameId, (gameState) =>
            {
                this.gameState = gameState;
                switch (gameState)
                {
                case GameState.PlacingShips:
                    gameStateText.text = "Faza rozstawiania";
                    break;
                case GameState.HostTurn:
                    gameStateText.text = "Tura hosta";
                    break;
                case GameState.GuestTurn:
                    gameStateText.text = "Tura go�cia";
                    break;
                }
            });

            yield return wait;
        }
    }

    IEnumerator GetMapStateCoroutine()
    {
        while (true)
        {
            string gameId = GameManager.gameId;
            #if UNITY_EDITOR
            if (overrideIds)
            {
                gameId = this.gameId;
            }
            #endif

            yield return Communication.GetMapState(gameId, (mapState) =>
            {
                this.mapState = mapState;

                // TODO Update board tiles
            });

            yield return wait;
        }
    }

    private IEnumerator Fire()
    {
        string coordinate = coordinateInput.text;
        string gameId = GameManager.gameId;
        string playerId = GameManager.playerId;
        #if UNITY_EDITOR
        if (overrideIds)
        {
            gameId = this.gameId;
            playerId = this.playerId;
        }
        #endif

        if (gameState == GameState.PlacingShips)
        {
            int shipSize = int.Parse(shipSizeDropdown.options[shipSizeDropdown.value].text);
            yield return Communication.PlaceShip(coordinate, gameId, playerId, shipSize);

            // TODO Place ship on board
        }
        else
        {
            yield return Communication.PostCoordinates(coordinate, gameId, playerId);
        }
    }
}
