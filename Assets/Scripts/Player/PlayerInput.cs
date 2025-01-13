using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour, PlayerInputConfig.ICombatActions, PlayerInputConfig.IGeneralActions, PlayerInputConfig.IMenuActions
{
    [SerializeField] UnityEvent onAttack, onDash, onInventory, onMenu, onQuit;
    [SerializeField] UnityEvent<Vector2> onMovement;

    PlayerInputConfig config;

    void Awake()
    {
        config = new PlayerInputConfig();
        config.General.SetCallbacks(this);
        config.Menu.SetCallbacks(this);
        config.Combat.SetCallbacks(this);

        config.General.Enable();
        config.Combat.Enable();
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
        config.General.Disable();
        config.Combat.Disable();
        config.Menu.Enable();
        onInventory?.Invoke();
    }

    public void OnMenu(InputAction.CallbackContext context)
    {
        if (!IsClicked(context)) return;
        config.General.Disable();
        config.Combat.Disable();
        config.Menu.Enable();
        onMenu?.Invoke();
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
        config.General.Enable();
        config.Combat.Enable();
        config.Menu.Disable();
        onQuit?.Invoke();
    }
}
