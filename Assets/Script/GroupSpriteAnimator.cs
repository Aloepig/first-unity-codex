using UnityEngine;

namespace Script
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class GroupSpriteAnimator : MonoBehaviour
    {
        [Header("Sprites")]
        [SerializeField] private Sprite idleSprite;
        [SerializeField] private Sprite walkSprite1;
        [SerializeField] private Sprite walkSprite2;

        [Header("Animation")]
        [SerializeField] private float walkFps = 6f;
        [SerializeField] private float moveThreshold = 0.05f;
        [SerializeField] private Rigidbody2D targetRigidbody;

        private SpriteRenderer spriteRenderer;
        private float walkTimer;
        private int walkFrame;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (targetRigidbody == null)
            {
                targetRigidbody = GetComponentInParent<Rigidbody2D>();
            }
        }

        private void Update()
        {
            bool isMoving = false;
            if (targetRigidbody != null)
            {
                isMoving = Mathf.Abs(targetRigidbody.linearVelocity.x) > moveThreshold;
            }

            if (!isMoving)
            {
                spriteRenderer.sprite = idleSprite;
                walkTimer = 0f;
                walkFrame = 0;
                return;
            }

            walkTimer += Time.deltaTime;
            if (walkTimer >= 1f / Mathf.Max(1f, walkFps))
            {
                walkTimer = 0f;
                walkFrame = 1 - walkFrame;
            }

            spriteRenderer.sprite = walkFrame == 0 ? walkSprite1 : walkSprite2;
        }
    }
}
