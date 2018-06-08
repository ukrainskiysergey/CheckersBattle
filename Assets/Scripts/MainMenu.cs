using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    Animator animator;

    public Slider DifficultySlider;
    public Dropdown Player1DropDown;
    public Dropdown Player2DropDown;

    void Start()
    {
        animator = GetComponent<Animator>();

        var gameSettings = GameSettings.Instance;
        DifficultySlider.value = gameSettings.Difficulty;
        Player1DropDown.value = gameSettings.isPlayerHuman[0] ? 0 : 1;
        Player2DropDown.value = gameSettings.isPlayerHuman[1] ? 0 : 1;
    }

    void Update()
    {

    }

    public void GameStart()
    {
        var gameSettings = GameSettings.Instance;
        gameSettings.Difficulty = DifficultySlider.value;

        gameSettings.isPlayerHuman = new bool[2] {
            Player1DropDown.value == 0,
            Player2DropDown.value == 0
        };

        SceneManager.LoadScene("GameScene");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
