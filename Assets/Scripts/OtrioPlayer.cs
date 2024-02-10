using UnityEngine;
using UnityEngine.InputSystem;

public class OtrioPlayer : MonoBehaviour
{
    public Vector2 inputVector;
    public OtrioController otrio;
    public int playerID;
    public void OnMovement(InputAction.CallbackContext context)
    {
        if(context.performed == true)
        {
            inputVector = context.ReadValue<Vector2>();

            if (otrio.playerActive == playerID)
            {
                otrio.IncrementVector(inputVector, playerID);
            }
        }
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed == true)
        {
            otrio.IncrementSelection(context.ReadValue<Vector2>().x, playerID);
        }
    }

    public void OnPlace(InputAction.CallbackContext context)
    {
        if (context.performed == true)
        {
            otrio.PlaceSelection(playerID);
        }
    }

}
