using UnityEngine;

public sealed class EventLogger : MonoBehaviour
{
    private void OnEnable()
    {
        GameEventBus.SubscribeToAll(OnAnyEvent);
    }

    private void OnDisable()
    {
        GameEventBus.UnsubscribeFromAll(OnAnyEvent);
    }

    private void OnAnyEvent(IGameEvent gameEvent)
    {
        Debug.Log(
            $"[EVENT] {gameEvent.GetType().Name} | " +
            $"Категория: {gameEvent.Category} | " +
            $"Время: {gameEvent.Time:F2}"
        );
    }
}