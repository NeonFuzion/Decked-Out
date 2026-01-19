using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour, PlayerInputConfig.ICombatActions, PlayerInputConfig.IMenuActions, PlayerInputConfig.IDialogueActions, PlayerInputConfig.IPlayerActions
{
    [SerializeField] UnityEvent onAttack, onDash, onInventory, onMenu, onQuit, onMap, onDialogue, onContinue, onMouseDown, onMouseUp;
    [SerializeField] UnityEvent<int> onHotbar;
    [SerializeField] UnityEvent<Vector2> onMovement, onMouse;

    PlayerInputConfig config;

    void Awake()
    {
        config = new PlayerInputConfig();
        config.Menu.SetCallbacks(this);
        config.Combat.SetCallbacks(this);
        config.Player.SetCallbacks(this);

        config.Combat.Enable();
        config.Player.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    bool IsClicked(InputAction.CallbackContext context)
    {
        return context.phase == InputActionPhase.Started;
    }

    void OnHotbar(InputAction.CallbackContext context, int index)
    {
        if (!IsClicked(context)) return;
        onHotbar?.Invoke(index);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!IsClicked(context)) return;
        onAttack?.Invoke();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!IsClicked(context)) return;
        onDash?.Invoke();
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!IsClicked(context)) return;
        InventorySetup();
    }

    public void InventorySetup()
    {
        MenuSetup();
        onInventory?.Invoke();
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (!IsClicked(context)) return;
        MenuSetup();
        onMenu?.Invoke();
    }

    public void MenuSetup()
    {
        config.Player.Disable();
        config.Combat.Disable();
        config.Menu.Enable();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        onMovement?.Invoke(context.ReadValue<Vector2>());
    }

    public void OnQuit(InputAction.CallbackContext context)
    {
        if (!IsClicked(context)) return;
        QuitSetup();
    }

    public void QuitSetup()
    {
        config.Player.Enable();
        config.Combat.Enable();
        config.Menu.Disable();
        onQuit?.Invoke();
    }

    public void OnMap(InputAction.CallbackContext context)
    {
        MenuSetup();
        onMap?.Invoke();
    }

    public void OnContinue(InputAction.CallbackContext context)
    {
        if (!IsClicked(context)) return;
        onContinue?.Invoke();
    }

    public void StartDialogue()
    {
        config.Player.Disable();
        config.Combat.Disable();
        config.Menu.Disable();
        config.Dialogue.Enable();
    }

    public void OnMouse(InputAction.CallbackContext context)
    {
        Vector2 position = context.ReadValue<Vector2>();
        onMouse?.Invoke(position);
    }

    public void OnHotbar1(InputAction.CallbackContext context)
    {
        OnHotbar(context, 1);
    }

    public void OnHotbar2(InputAction.CallbackContext context)
    {
        OnHotbar(context, 2);
    }

    public void OnHotbar3(InputAction.CallbackContext context)
    {
        OnHotbar(context, 3);
    }

    public void OnHotbar4(InputAction.CallbackContext context)
    {
        OnHotbar(context, 4);
    }

    public void OnLeftMouse(InputAction.CallbackContext context)
    {
        if (IsClicked(context)) onMouseDown?.Invoke();
        else if (context.canceled) onMouseUp?.Invoke();
    }
}
