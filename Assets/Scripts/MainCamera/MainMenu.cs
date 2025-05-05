using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneHandler.SetScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
