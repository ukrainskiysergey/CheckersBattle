using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public GameObject GameOverCanvas;
    public GameObject NextPlayerCanvas;
    public GameObject NextPlayerText;
    public GameObject WinnerText;
    public Player[] players;
    public float MinimalPlayerChangingTime = 1.0f;
    public float DestroyHeight = -2.0f;
    public float RestSceneVelocity = 1e-5f;

    void Start()
    {
        GameOverCanvas.SetActive(false);

        playerControllers = new PlayerController[players.Length];
        for (var i = 0; i < players.Length; i++)
        {
            if (GameSettings.Instance.isPlayerHuman[i])
                playerControllers[i] = new HumanController(players[i]);
            else
                playerControllers[i] = new AIController(players[i]);
        }

        ShowNextPlayerCanvas();
    }

    void Update()
    {
        if (!IsPause)
        {
            var move = currentPlayer.Update();
            if (move.HasValue)
            {
                var moveVal = move.Value;
                moveVal.piece.GetComponent<Rigidbody>().AddForce(moveVal.direction * moveVal.velocity, ForceMode.VelocityChange);
                PrepareForPlayerSwitching();
            }
        }
    }

    void LateUpdate()
    {
        if (IsGameOver)
        {
            if (GameOverCanvas && !GameOverCanvas.activeSelf && SceneIsResting)
            {
                GameOverCanvas.SetActive(true);

                bool draw = true;
                foreach (var player in players)
                {
                    if (player.transform.childCount != 0)
                    {
                        WinnerText.GetComponent<Text>().text = player.Name + "の勝ち";
                        draw = false;
                        break;
                    }
                }

                if (draw)
                    WinnerText.GetComponent<Text>().text = "引き分け";
            }
            return;
        }

        if (playerSwitching && (Time.time - changingStart) >= MinimalPlayerChangingTime)
        {
            if (SceneIsResting)
                NextPlayer();
        }
    }

    private void PrepareForPlayerSwitching()
    {
        changingStart = Time.time;
        playerSwitching = true;
    }

    private void NextPlayer()
    {
        currentPlayerIdx = (currentPlayerIdx + 1) % playerControllers.Length;
        playerSwitching = false;
        ShowNextPlayerCanvas();
    }

    private void ShowNextPlayerCanvas()
    {
        NextPlayerText.GetComponent<Text>().text = players[currentPlayerIdx].Name + "の番";
        NextPlayerCanvas.GetComponent<Animator>().SetTrigger("NextPlayer");
    }

    private bool SceneIsResting
    {
        get
        {
            float totalVelocity = 0.0f;
            foreach (var player in players)
            {
                foreach (Transform piece in player.transform)
                {
                    if (piece.position.y < DestroyHeight)
                        Destroy(piece.gameObject);

                    totalVelocity += piece.GetComponent<Rigidbody>().velocity.magnitude;
                }
            }
            return totalVelocity <= RestSceneVelocity;
        }
    }

    private bool IsGameOver
    {
        get
        {
            if (gameOver)
                return true;

            foreach (var player in players)
                if (player.transform.childCount == 0)
                {
                    gameOver = true;
                    return true;
                }

            return false;
        }
    }

    private PlayerController currentPlayer
    {
        get { return playerControllers[currentPlayerIdx]; }
    }

    public bool IsPause
    {
        get { return playerSwitching; }
    }

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private PlayerController[] playerControllers;

    private int currentPlayerIdx = 0;
    private bool playerSwitching = false;
    private float changingStart;
    private bool gameOver = false;
}
