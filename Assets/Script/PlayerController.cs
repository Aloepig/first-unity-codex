using UnityEngine;
using UnityEngine.InputSystem;

namespace Script
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private InputActionReference moveAction;

        private InputAction moveInput;

        private void Awake()
        {
            moveInput = moveAction != null ? moveAction.action : null;
        }

        private void OnEnable()
        {
            moveInput?.Enable();
        }

        private void OnDisable()
        {
            moveInput?.Disable();
        }

        private void Update()
        {
            float horizontal = moveInput != null ? moveInput.ReadValue<Vector2>().x : 0f;
            transform.Translate(Vector2.right * (horizontal * moveSpeed * Time.deltaTime));
        }
    }
}