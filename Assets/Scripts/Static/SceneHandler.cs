using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneHandler
{
    public static void SetScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public static int SceneIndex { get => SceneManager.GetActiveScene().buildIndex; }
}
