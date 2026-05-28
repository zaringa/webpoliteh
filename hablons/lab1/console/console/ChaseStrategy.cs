public class ChaseStrategy : INpcStrategy
{
    public string Name => "Chase";
    
    public void Enter(NPC npc) => Console.WriteLine(" Начал погоню!");
    
    public void Execute(NPC npc)
    {
        if (!npc.PlayerPosition.HasValue) return;
        
        npc.ChaseTo(npc.PlayerPosition.Value);
        npc.Position = npc.PlayerPosition.Value;
        
        if (npc.GetDistanceTo(npc.PlayerPosition) <= npc.AttackDistance)
        {
            npc.Attack();
        }
    }
    
    public void Exit(NPC npc) => Console.WriteLine("  Прекратил погоню");
}