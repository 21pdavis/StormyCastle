using UnityEngine;

using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public abstract class NPCController : MonoBehaviour
{
    public abstract void OnInteract();
}
