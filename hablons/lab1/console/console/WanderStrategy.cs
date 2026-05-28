public class WanderStrategy : INpcStrategy
{
    public string Name => "Wander";
    private int _steps = 0;
    
    public void Enter(NPC npc) => Console.WriteLine(" Начал блуждание");
    
    public void Execute(NPC npc)
    {
        _steps++;
        if (_steps % 3 == 0)
        {
            var randomPoint = new Vector3(
                npc.Position.X + Random.Shared.Next(-5, 6),
                npc.Position.Y,
                npc.Position.Z + Random.Shared.Next(-5, 6)
            );
            npc.WanderTo(randomPoint);
            npc.Position = randomPoint;
        }
    }
    
    public void Exit(NPC npc) => Console.WriteLine(" Закончил блуждание");
}