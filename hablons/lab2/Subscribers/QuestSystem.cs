using System;
using UnityEngine;

public sealed class QuestSystem : MonoBehaviour
{
    [SerializeField] private string questId = "CollectCoinsQuest";
    [SerializeField] private string requiredItemId = "Coin";
    [SerializeField] private int requiredAmount = 5;

    private int currentAmount;
    private bool completed;

    private void OnEnable()
    {
        GameEventBus.Subscribe<ItemCollectedEvent>(OnItemCollected);
    }

    private void OnDisable()
    {
        GameEventBus.Unsubscribe<ItemCollectedEvent>(OnItemCollected);
    }

    private void OnItemCollected(ItemCollectedEvent itemEvent)
    {
        if (completed)
            return;

        bool isRequiredItem = string.Equals(
            itemEvent.ItemId,
            requiredItemId,
            StringComparison.OrdinalIgnoreCase
        );

        if (!isRequiredItem)
            return;

        currentAmount += itemEvent.Amount;

        Debug.Log($"Квест {questId}: собрано {currentAmount}/{requiredAmount}");

        if (currentAmount >= requiredAmount)
        {
            completed = true;
            GameEventBus.Publish(new QuestCompletedEvent(questId));
        }
    }
}