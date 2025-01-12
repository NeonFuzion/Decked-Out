using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScreenHandle
{
    public static Vector2 MousePos { get => Camera.main.ScreenToWorldPoint(Input.mousePosition); }

    public static Vector2 ScreenRes
    {
        get
        {
            Vector2 pixRes = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            Vector2 cameraPos = Camera.main.transform.position;
            return pixRes - cameraPos;
        }
    }
}
