using UnityEngine;
using UnityEngine.InputSystem;

public class player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    private float horizontalInput;
    private bool isFacingRight = true;
    private Vector2 vec;

    [Header("Coin Toss Settings")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform tossPoint;
    [SerializeField] private Vector2 tossForce = new Vector2(3f, 5f);

    [Header("Inventory")]
    [SerializeField] private int coinCount = 10;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalInput = 0f;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            {
                horizontalInput -= 1f;
            }

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            {
                horizontalInput += 1f;
            }

            if (Keyboard.current.gKey.wasPressedThisFrame && coinCount > 0)
            {
                TossCoin();
            }
        }

        vec.x = horizontalInput;
        vec.y = 0f;

        FlipCharacter();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
    }

    private void FlipCharacter()
    {
        if ((isFacingRight && horizontalInput < 0f) || (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void TossCoin()
    {
        coinCount--;

        GameObject newCoin = Instantiate(coinPrefab, tossPoint.position, Quaternion.identity);
        Rigidbody2D coinRb = newCoin.GetComponent<Rigidbody2D>();

        float lookDirection = transform.localScale.x;
        Vector2 finalForce = new Vector2(tossForce.x * lookDirection, tossForce.y);

        coinRb.AddForce(finalForce, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            coinCount++;
            Debug.Log($"Coin picked up. Current coins: {coinCount}");
        }
    }
}
