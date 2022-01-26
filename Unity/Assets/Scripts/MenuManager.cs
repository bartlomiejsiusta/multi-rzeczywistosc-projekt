using System.Collections;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField gameIdInputField;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button quitGameButton;

    private IEnumerator Start()
    {
        string playerId = Guid.NewGuid().ToString();
        yield return Communication.Register(playerId);
        GameManager.playerId = playerId;

        newGameButton.onClick.AddListener(() => StartCoroutine(NewGame()));
        joinGameButton.onClick.AddListener(() => StartCoroutine(JoinGame()));
        quitGameButton.onClick.AddListener(() => Application.Quit());
    }

    private IEnumerator NewGame()
    {
        yield return Communication.Create((gameId) => GameManager.gameId = gameId);
        yield return Communication.Enter(GameManager.playerId, GameManager.gameId);
        GameManager.playerIndex = 0;
        SceneManager.LoadScene("Game");
    }

    private IEnumerator JoinGame()
    {
        string gameId = gameIdInputField.text;
        GameManager.gameId = gameId;
        yield return Communication.Enter(GameManager.playerId, gameId);
        GameManager.playerIndex = 1;
        SceneManager.LoadScene("Game");
    }
}
