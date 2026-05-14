using UnityEngine;
using UnityEngine.InputSystem;

namespace Script
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] private float moveSpeed = 6f;
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference throwAction;

        [Header("Coin Toss Settings")] [SerializeField]
        private GameObject coinPrefab;
        [SerializeField] private Transform tossPoint;
        [SerializeField] private Vector2 tossForce = new Vector2(3f, 5f);
        [SerializeField] private float coinPickupDelayAfterThrow = 0.35f;

        [Header("Inventory Coin")] [SerializeField]
        private int coinCount = 10;

        private InputAction moveInput;
        private InputAction throwInput;
        private float horizontal;
        private Rigidbody2D rb;

        private void Awake()
        {
            moveInput = moveAction != null ? moveAction.action : null;
            throwInput = throwAction != null ? throwAction.action : null;
            rb = GetComponent<Rigidbody2D>();
        }

        private void OnEnable()
        {
            moveInput?.Enable();
            throwInput?.Enable();
        }

        private void OnDisable()
        {
            moveInput?.Disable();
            throwInput?.Disable();
        }

        private void Update()
        {
            horizontal = moveInput != null ? moveInput.ReadValue<Vector2>().x : 0f;

            if (throwInput != null && throwInput.WasPressedThisFrame() && coinCount > 0)
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
            float lookDirection;
            if (tossPoint == null || tossPoint == transform)
            {
                lookDirection = transform.localScale.x >= 0f ? 1f : -1f;
                spawnPosition += new Vector3(lookDirection * 0.8f, 0.2f, 0f);
            }
            GameObject newCoin = Instantiate(coinPrefab, spawnPosition, Quaternion.identity);
            Rigidbody2D coinRb = newCoin.GetComponent<Rigidbody2D>();
            if (coinRb == null)
            {
                Debug.LogWarning("PlayerController: Coin prefab has no Rigidbody2D.");
                return;
            }

            Coin coin = newCoin.GetComponent<Coin>();
            if (coin != null)
            {
                coin.InitializeAfterThrow(coinPickupDelayAfterThrow);
            }

            lookDirection = transform.localScale.x >= 0f ? 1f : -1f;
            Vector2 finalForce = new Vector2(tossForce.x * lookDirection, tossForce.y);

            coinRb.AddForce(finalForce, ForceMode2D.Impulse);
        }


        public void HandleCoinTrigger(Collider2D collision)
        {
            if (collision.CompareTag("Coin"))
            {
                Coin coin = collision.GetComponent<Coin>();
                if (coin != null)
                {
                    coin.AttractTo(transform);
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Keep support when trigger collider exists on the same object as PlayerController.
            HandleCoinTrigger(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Coin"))
            {
                Coin coin = collision.gameObject.GetComponent<Coin>();
                if (coin != null && !coin.CanBeCollected())
                {
                    return;
                }

                Destroy(collision.gameObject);
                coinCount++;
                Debug.Log($"Coin picked up. Current coins: {coinCount}");
            }
        }
    }
}
