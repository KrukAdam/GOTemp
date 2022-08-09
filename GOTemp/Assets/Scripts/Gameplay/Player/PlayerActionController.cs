using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActionController : MonoBehaviour
{
    [SerializeField]
    private PlayerMoveController playerMoveController = null;

    public void Setup()
    {
        CanUse(true);
    }

    public void CanUse(bool canUse)
    {
        if (canUse)
        {
            GameManager.Instance.InputManager.InputActions.Player.Interaction.performed += UseFieldObject;
        }
        else
        {
            GameManager.Instance.InputManager.InputActions.Player.Interaction.performed -= UseFieldObject;
        }
    }

    private void UseFieldObject(InputAction.CallbackContext ctx)
    {
        if(playerMoveController.CurrentStandField.ActionObject != null)
        {
            playerMoveController.CurrentStandField.ActionObject.ExecuteInteraction();
        }
    }
}
