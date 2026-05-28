using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class CollectibleItem : MonoBehaviour
{
    [SerializeField] private string itemId = "Coin";
    [SerializeField] private int amount = 1;
    [SerializeField] private bool destroyAfterCollect = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        GameEventBus.Publish(new ItemCollectedEvent(itemId, amount, other.gameObject));

        if (destroyAfterCollect)
        {
            Destroy(gameObject);
        }
    }

    private void Reset()
    {
        Collider itemCollider = GetComponent<Collider>();
        itemCollider.isTrigger = true;
    }
}