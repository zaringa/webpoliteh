public interface INpcStrategy
{
    void Enter(NPC npc);
    void Execute(NPC npc);
    void Exit(NPC npc);
    string Name { get; }
}