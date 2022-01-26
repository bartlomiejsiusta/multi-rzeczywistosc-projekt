using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject[] tiles;
    [SerializeField] private GameObject[] ships;
    [SerializeField] private ShotMarkersManager shotMarkersManager;

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

    private int shipIndex = 99;
    private string shipCoordinate = "";
    int size2 = 2, size3 = 2, size4 = 2;

    private void Awake()
    {
        wait = new WaitForSeconds(stateQueryTime);

        gameIdText.text = GameManager.gameId;

        StartCoroutine(GetCurrentGameStateCoroutine());
        StartCoroutine(GetMapStateCoroutine());

        fireButton.onClick.AddListener(() => StartCoroutine(Fire()));
    }

    private void Update()
    {
        if((shipIndex!=99) & (!shipCoordinate.Equals(""))){
            setShipsPosition(shipIndex, shipCoordinate);
            shipIndex = 99;
            shipCoordinate = "";
        }
        
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
                    gameStateText.text = "Tura goscia";
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

            
            if(shipSize == 2 & size2 == 2)
            {
                shipIndex = 0;
                size2--;
            } else if(shipSize == 2 & size2 == 1)
            {
                shipIndex = 1;
            } else if(shipSize == 3 & size3 == 2){
                shipIndex = 2;
                size3--;
            } else if(shipSize == 3 & size3 == 1){
                shipIndex = 3;
            } else if(shipSize == 4 & size4 == 2){
                shipIndex = 4;
                size4--;
            } else {
                shipIndex = 5;
            }

            shipCoordinate = coordinate;
        }
        else
        {
            shotMarkersManager.coordinates = coordinate;
            yield return Communication.PostCoordinates(coordinate, gameId, playerId);
        }
    }

    private void setShipsPosition(int shipIndex, string shipCoordinate)
    {
        for(int i = 0; i<tiles.Length; i++)
        {
            if(tiles[i].name.Equals(shipCoordinate.ToLower())){
                Vector3 tilePosition = tiles[i].transform.localPosition;
                tilePosition.x = tilePosition.x + 0.93f;
                tilePosition.y = tilePosition.y + 0.18f;
                if(shipIndex == 0 | shipIndex == 1){
                    tilePosition.z = tilePosition.z - 0.1f;
                } else if(shipIndex == 2 | shipIndex == 3){
                    tilePosition.z = tilePosition.z - 0.28f;
                } else if(shipIndex == 4 | shipIndex == 5){
                    tilePosition.z = tilePosition.z - 0.47f;
                }
                
                Debug.Log(tilePosition);
                ships[shipIndex].transform.localPosition = tilePosition;
            }
        }
    }
}


