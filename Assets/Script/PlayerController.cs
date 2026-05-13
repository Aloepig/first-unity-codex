using UnityEngine;
using UnityEngine.InputSystem;

namespace Script
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private InputActionReference moveAction;

        [Header("Coin Toss Settings")]
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private Transform tossPoint;
        [SerializeField] private Vector2 tossForce = new Vector2(3f, 5f);
        
        [Header("Inventory Coin")] [SerializeField]
        private int coinCount = 10;

        private InputAction moveInput;
        private float horizontal;
        private Rigidbody2D rb;

        private void Awake()
        {
            moveInput = moveAction != null ? moveAction.action : null;
            rb = GetComponent<Rigidbody2D>();
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
            horizontal = moveInput != null ? moveInput.ReadValue<Vector2>().x : 0f;
            
            if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame && coinCount > 0)
            {
                TossCoin();
            }
        }

        private void FixedUpdate()
        {
            if (rb == null)
            {
                return;
            }

            rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);
        }
        
        private void TossCoin()
        {
            if (coinPrefab == null)
            {
                Debug.LogWarning("PlayerController: coinPrefab is not assigned.");
                return;
            }

            coinCount--;

            Vector3 spawnPosition = tossPoint != null ? tossPoint.position : transform.position;
            GameObject newCoin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D coinRb = newCoin.GetComponent<Rigidbody2D>();
            if (coinRb == null)
            {
                Debug.LogWarning("PlayerController: Coin prefab has no Rigidbody2D.");
                return;
            }

            float lookDirection = transform.localScale.x;
            Vector2 finalForce = new Vector2(tossForce.x * lookDirection, tossForce.y);

            coinRb.AddForce(finalForce, ForceMode2D.Impulse);
        }

    }
}
