using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public sealed class SoundEventListener : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip itemCollectedClip;
    [SerializeField] private AudioClip questCompletedClip;
    [SerializeField] private AudioClip levelCompletedClip;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        GameEventBus.SubscribeToCategory(GameEventCategory.Item, OnItemCategoryEvent);
        GameEventBus.Subscribe<QuestCompletedEvent>(OnQuestCompleted);
        GameEventBus.Subscribe<LevelCompletedEvent>(OnLevelCompleted);
    }

    private void OnDisable()
    {
        GameEventBus.UnsubscribeFromCategory(GameEventCategory.Item, OnItemCategoryEvent);
        GameEventBus.Unsubscribe<QuestCompletedEvent>(OnQuestCompleted);
        GameEventBus.Unsubscribe<LevelCompletedEvent>(OnLevelCompleted);
    }

    private void OnItemCategoryEvent(IGameEvent gameEvent)
    {
        PlayClip(itemCollectedClip);
    }

    private void OnQuestCompleted(QuestCompletedEvent questEvent)
    {
        PlayClip(questCompletedClip);
    }

    private void OnLevelCompleted(LevelCompletedEvent levelEvent)
    {
        PlayClip(levelCompletedClip);
    }

    private void PlayClip(AudioClip clip)
    {
        if (clip == null || audioSource == null)
            return;

        audioSource.PlayOneShot(clip);
    }
}