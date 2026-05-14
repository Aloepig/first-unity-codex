using UnityEngine;

namespace Script
{
    [RequireComponent(typeof(Collider2D))]
    public class PickupRangeTrigger : MonoBehaviour
    {
        private PlayerController playerController;
        private Collider2D triggerCollider;

        private void Awake()
        {
            playerController = GetComponentInParent<PlayerController>();
            triggerCollider = GetComponent<Collider2D>();

            if (playerController == null)
            {
                Debug.LogWarning("PickupRangeTrigger: Parent PlayerController not found. Attach this only under Player.");
                return;
            }

            if (transform == playerController.transform)
            {
                Debug.LogWarning("PickupRangeTrigger: Do not attach on Player root/body. Create a child object such as 'PickupRange'.");
            }

            if (gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.LogWarning("PickupRangeTrigger: Attached to Ground layer. Move this component to Player child trigger object.");
            }

            triggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (playerController == null || triggerCollider == null || !triggerCollider.isTrigger)
            {
                return;
            }

            playerController.HandleCoinTrigger(collision);
        }
    }
}
