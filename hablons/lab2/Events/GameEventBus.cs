using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameEventBus
{
    private static readonly Dictionary<Type, Delegate> typedSubscribers =
        new Dictionary<Type, Delegate>();

    private static readonly Dictionary<GameEventCategory, Action<IGameEvent>> categorySubscribers =
        new Dictionary<GameEventCategory, Action<IGameEvent>>();

    private static Action<IGameEvent> allSubscribers;

    public static void Subscribe<TEvent>(Action<TEvent> listener)
        where TEvent : IGameEvent
    {
        Type eventType = typeof(TEvent);

        if (typedSubscribers.TryGetValue(eventType, out Delegate existing))
        {
            typedSubscribers[eventType] = Delegate.Combine(existing, listener);
        }
        else
        {
            typedSubscribers[eventType] = listener;
        }
    }

    public static void Unsubscribe<TEvent>(Action<TEvent> listener)
        where TEvent : IGameEvent
    {
        Type eventType = typeof(TEvent);

        if (!typedSubscribers.TryGetValue(eventType, out Delegate existing))
            return;

        Delegate current = Delegate.Remove(existing, listener);

        if (current == null)
        {
            typedSubscribers.Remove(eventType);
        }
        else
        {
            typedSubscribers[eventType] = current;
        }
    }

    public static void SubscribeToCategory(GameEventCategory category, Action<IGameEvent> listener)
    {
        if (categorySubscribers.TryGetValue(category, out Action<IGameEvent> existing))
        {
            categorySubscribers[category] = existing + listener;
        }
        else
        {
            categorySubscribers[category] = listener;
        }
    }

    public static void UnsubscribeFromCategory(GameEventCategory category, Action<IGameEvent> listener)
    {
        if (!categorySubscribers.TryGetValue(category, out Action<IGameEvent> existing))
            return;

        Action<IGameEvent> current = existing - listener;

        if (current == null)
        {
            categorySubscribers.Remove(category);
        }
        else
        {
            categorySubscribers[category] = current;
        }
    }

    public static void SubscribeToAll(Action<IGameEvent> listener)
    {
        allSubscribers += listener;
    }

    public static void UnsubscribeFromAll(Action<IGameEvent> listener)
    {
        allSubscribers -= listener;
    }

    public static void Publish<TEvent>(TEvent gameEvent)
        where TEvent : IGameEvent
    {
        PublishToTypedSubscribers(gameEvent);
        PublishToCategorySubscribers(gameEvent);
        PublishToAllSubscribers(gameEvent);
    }

    public static void Clear()
    {
        typedSubscribers.Clear();
        categorySubscribers.Clear();
        allSubscribers = null;
    }

    private static void PublishToTypedSubscribers<TEvent>(TEvent gameEvent)
        where TEvent : IGameEvent
    {
        Type eventType = typeof(TEvent);

        if (!typedSubscribers.TryGetValue(eventType, out Delegate listeners))
            return;

        foreach (Delegate listener in listeners.GetInvocationList())
        {
            try
            {
                ((Action<TEvent>)listener).Invoke(gameEvent);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }

    private static void PublishToCategorySubscribers(IGameEvent gameEvent)
    {
        if (!categorySubscribers.TryGetValue(gameEvent.Category, out Action<IGameEvent> listeners))
            return;

        InvokeSafely(listeners, gameEvent);
    }

    private static void PublishToAllSubscribers(IGameEvent gameEvent)
    {
        InvokeSafely(allSubscribers, gameEvent);
    }

    private static void InvokeSafely(Action<IGameEvent> listeners, IGameEvent gameEvent)
    {
        if (listeners == null)
            return;

        foreach (Delegate listener in listeners.GetInvocationList())
        {
            try
            {
                ((Action<IGameEvent>)listener).Invoke(gameEvent);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }
    }
}