using UnityEngine;

namespace Script
{
    public class Coin : MonoBehaviour
    {
        private Rigidbody2D rb;
        private Transform targetPlayer;
        private bool isFlyingToPlayer = false;
        private float canAttractAt;
    
        [SerializeField] private float magnetSpeed = 8f;
        [SerializeField] private float pickupDelayAfterThrow = 0.35f;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            canAttractAt = Time.time + pickupDelayAfterThrow;
        }

        public void InitializeAfterThrow(float delay)
        {
            canAttractAt = Time.time + Mathf.Max(0f, delay);
        }

        public bool CanBeCollected()
        {
            return Time.time >= canAttractAt;
        }

        // 플레이어가 코인을 감지했을 때 호출할 함수
        public void AttractTo(Transform playerTransform)
        {
            if (isFlyingToPlayer || !CanBeCollected()) return;
        
            targetPlayer = playerTransform;
            isFlyingToPlayer = true;
            rb.gravityScale = 0; // 자석 효과 중에는 중력 무시
            rb.linearVelocity = Vector2.zero;
        }

        void FixedUpdate()
        {
            if (isFlyingToPlayer && targetPlayer != null)
            {
                // 플레이어 위치를 향해 빠르게 이동
                Vector2 direction = (targetPlayer.position - transform.position).normalized;
                rb.linearVelocity = direction * magnetSpeed;
            }
        }
    }
}
