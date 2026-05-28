using UnityEngine;

public sealed class GameUIEventListener : MonoBehaviour
{
    private void OnEnable()
    {
        GameEventBus.Subscribe<ItemCollectedEvent>(OnItemCollected);
        GameEventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted);
        GameEventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<ItemCollectedEvent>(OnItemCollected);
        GameEventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
        GameEventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
    }

    private void OnItemCollected(ItemCollectedEvent itemEvent)
    {
        Debug.Log($"подобран предмет {itemEvent.ItemId} x{itemEvent.Amount}");
    }

    private void OnQuestCompleted(QuestCompletedEvent questEvent)
    {
        Debug.Log($"квест выполнен — {questEvent.QuestId}");
    }

    private void OnLevelCompleted(LevelCompletedEvent levelEvent)
    {
        Debug.Log($"уровень завершён — {levelEvent.LevelName}, награда: {levelEvent.ScoreReward}");
    }
}