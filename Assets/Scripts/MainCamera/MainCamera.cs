using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] GameObject prefabDeathObj;
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
        GameObject deathObj = Instantiate(prefabDeathObj);
        Transform canvas = deathObj.transform.GetChild(0);
        canvas.GetChild(1).GetComponent<Button>().onClick.AddListener(LoadDungeon);
        canvas.GetChild(2).GetComponent<Button>().onClick.AddListener(LoadMainMenu);
    }

    void LoadDungeon() => SceneHandler.SetScene(1);
    void LoadMainMenu() => SceneHandler.SetScene(0);
}
