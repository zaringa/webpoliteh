using UnityEngine;

[RequireComponent(typeof(Collider))]
public sealed class LevelFinishTrigger : MonoBehaviour
{
    [SerializeField] private string levelName = "Level_1";
    [SerializeField] private int scoreReward = 1000;

    private bool completed;

    private void OnTriggerEnter(Collider other)
    {
        if (completed)
            return;

        if (!other.CompareTag("Player"))
            return;

        completed = true;

        GameEventBus.Publish(new LevelCompletedEvent(levelName, scoreReward));
    }

    private void Reset()
    {
        Collider triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
    }
}