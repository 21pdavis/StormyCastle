using UnityEngine;
using UnityEngine.InputSystem;
using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

using static Helpers;

public class PlayerCombat : MonoBehaviour
{
    private PlayerAnimationStateController animationController;

    private void Start()
    {
        animationController = GetComponent<PlayerAnimationStateController>();
    }

    private void Update()
    {

    }

    public void Attack(CallbackContext context)
    {
        if (context.performed)
        {
            animationController.Attack();

            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
            Vector2 projectedMousePos = new Vector2(mousePos.x, mousePos.y);

            Vector2 directionFromCharacter = (projectedMousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
            FlipSprite(directionFromCharacter, transform);
        }
    }
}
