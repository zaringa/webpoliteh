public class EvadeStrategy : INpcStrategy
{
    public string Name => "Evade";
    
    public void Enter(NPC npc) => Console.WriteLine(" ПАНИКА!");
    
    public void Execute(NPC npc)
    {
        if (!npc.DangerPosition.HasValue) return;
        
        var direction = new Vector3(
            npc.Position.X - npc.DangerPosition.Value.X,
            0,
            npc.Position.Z - npc.DangerPosition.Value.Z
        );
        
        var escapePoint = new Vector3(
            npc.Position.X + direction.X * 2,
            npc.Position.Y,
            npc.Position.Z + direction.Z * 2
        );
        
        npc.EvadeTo(escapePoint);
        npc.Position = escapePoint;
    }
    
    public void Exit(NPC npc) => Console.WriteLine("   Успокоился");
}