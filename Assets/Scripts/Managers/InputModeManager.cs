using UnityEngine;
using UnityEngine.InputSystem;

public class InputModeManager : MonoBehaviour
{
    public static InputModeManager Instance;

    private PlayerInput playerInput;

    private void Awake()
    {
        Instance = this;
        playerInput = FindFirstObjectByType<PlayerInput>();
    }

    public void SwitchToUI()
    {
        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("UI");
    }

    public void SwitchToGameplay()
    {
        if (playerInput == null)
            playerInput = FindFirstObjectByType<PlayerInput>();

        if (playerInput != null)
            playerInput.SwitchCurrentActionMap("MainGame");
    }
}