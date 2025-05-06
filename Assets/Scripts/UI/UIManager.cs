using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Button quitButton;
    [SerializeField] UnityEvent onMenuOpened, onMenuClosed;

    GameObject menu;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
