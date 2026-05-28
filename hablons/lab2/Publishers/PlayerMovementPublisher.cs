using UnityEngine;

public sealed class PlayerMovementPublisher : MonoBehaviour
{
    [SerializeField] private float minDistanceForEvent = 0.25f;

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;
    }

    private void Update()
    {
        Vector3 currentPosition = transform.position;
        float distance = Vector3.Distance(lastPosition, currentPosition);

        if (distance >= minDistanceForEvent)
        {
            GameEventBus.Publish(new PlayerMovedEvent(lastPosition, currentPosition));
            lastPosition = currentPosition;
        }
    }
}