using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public InputAction LoadInputAction(string action)
    {
        InputAction inputAction;
        var inputActionAsset = Resources.Load<InputActionAsset>("InputActions/InputSystem_Actions");
        if (inputActionAsset != null)
        {
            inputAction = inputActionAsset.FindAction(action);


            if (inputAction != null)
            {
                return inputAction;
            }
            else
            {
                UnityEngine.Debug.LogError("Hotbar/SelectPosition action not found in the Input Action Asset.");
            }
        }
        else
        {
            UnityEngine.Debug.LogError("Input Action Asset not found.");
        }
        return null; 
    }
}