using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] Button quitButton;
    [SerializeField] GameObject defaultMenu;
    [SerializeField] UnityEvent onMenuOpened, onMenuClosed;

    GameObject menu;

    void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!defaultMenu) return;
        OpenMenu(defaultMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenMenu(GameObject menu)
    {
        if (this.menu) this.menu.SetActive(false);
        this.menu = menu;

        menu.SetActive(true);
        onMenuOpened?.Invoke();
    }

    public void CloseMenu()
    {
        if (quitButton) quitButton.onClick.RemoveAllListeners();
        menu.SetActive(false);

        onMenuClosed?.Invoke();
    }
}
