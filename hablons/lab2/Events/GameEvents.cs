using UnityEngine;

public struct PlayerMovedEvent : IGameEvent
{
    public Vector3 OldPosition { get; }
    public Vector3 NewPosition { get; }
    public float Distance { get; }

    public GameEventCategory Category => GameEventCategory.Player;
    public float Time { get; }

    public PlayerMovedEvent(Vector3 oldPosition, Vector3 newPosition)
    {
        OldPosition = oldPosition;
        NewPosition = newPosition;
        Distance = Vector3.Distance(oldPosition, newPosition);
        Time = UnityEngine.Time.time;
    }
}

public struct ItemCollectedEvent : IGameEvent
{
    public string ItemId { get; }
    public int Amount { get; }
    public GameObject Collector { get; }

    public GameEventCategory Category => GameEventCategory.Item;
    public float Time { get; }

    public ItemCollectedEvent(string itemId, int amount, GameObject collector)
    {
        ItemId = itemId;
        Amount = amount;
        Collector = collector;
        Time = UnityEngine.Time.time;
    }
}

public struct LevelCompletedEvent : IGameEvent
{
    public string LevelName { get; }
    public int ScoreReward { get; }

    public GameEventCategory Category => GameEventCategory.Level;
    public float Time { get; }

    public LevelCompletedEvent(string levelName, int scoreReward)
    {
        LevelName = levelName;
        ScoreReward = scoreReward;
        Time = UnityEngine.Time.time;
    }
}

public struct QuestCompletedEvent : IGameEvent
{
    public string QuestId { get; }

    public GameEventCategory Category => GameEventCategory.Quest;
    public float Time { get; }

    public QuestCompletedEvent(string questId)
    {
        QuestId = questId;
        Time = UnityEngine.Time.time;
    }
}