using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown graphicsDropdown;

    //MainMenu
    public void StartGame()
    {
        Debug.Log("Start Game !!");
        SceneManager.LoadScene("rpg");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game !!");
        Application.Quit();
    }

    //SettingsMenu
    public void ChangeGraphicsQuality()
    {
        Debug.Log("Changed Quality !!");
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Debug.Log("FullScreen!");
        Screen.fullScreen = isFullScreen;
    }

    public void SetVolume(float volume)
    {

    }

}
