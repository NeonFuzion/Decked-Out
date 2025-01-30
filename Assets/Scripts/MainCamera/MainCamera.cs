using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] GameObject deathScreen;
    [SerializeField] Vector3 offset;
    [SerializeField] float dampening;
    [SerializeField] UnityEvent onLoadScene;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        onLoadScene.Invoke();

        velocity = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        Vector3 movePos = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, movePos, ref velocity, dampening);
    }

    public void InitializeDeathScreen()
    {
        deathScreen.SetActive(true);
    }

    public void MoveCameraNonlinear()
    {
        transform.position = target.position + offset;
    }

    public void LoadMainMenu() => SceneHandler.SetScene(0);
    public void LoadVillage() => SceneHandler.SetScene(1);
    public void LoadDungeon() => SceneHandler.SetScene(2);

    public static Vector2 MousePosition { get => Camera.main.ScreenToWorldPoint(Input.mousePosition); }
}
