public enum GameEventCategory
{
    Player,
    Item,
    Level,
    Quest,
    System
}

public interface IGameEvent
{
    GameEventCategory Category { get; }
    float Time { get; }
}
