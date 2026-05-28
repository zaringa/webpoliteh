class Program
{
    static void Main()
    {
        var npc = new NPC { Position = new Vector3(0, 0, 0) };
        var wander = new WanderStrategy();
        var chase = new ChaseStrategy();
        var evade = new EvadeStrategy();
        
        npc.SetStrategy(wander);
        
        for (int frame = 1; frame <= 15; frame++)
        {
            
            if (frame == 5) 
            {
                npc.PlayerPosition = new Vector3(10, 0, 10);
                Console.WriteLine("[Событие] Игрок обнаружен!");
            }
            if (frame == 10) 
            {
                npc.DangerPosition = new Vector3(1, 0, 1);
                Console.WriteLine("[Событие] Опасность рядом!");
            }
            if (frame == 13) 
            {
                npc.DangerPosition = null;
                Console.WriteLine("[Событие] Опасность устранена");
            }
            
            if (npc.DangerPosition.HasValue && 
                npc.GetDistanceTo(npc.DangerPosition) <= npc.DangerRange)
            {
                if (npc.CurrentStrategy != evade) npc.SetStrategy(evade);
            }
            else if (npc.PlayerPosition.HasValue && 
                     npc.GetDistanceTo(npc.PlayerPosition) <= npc.ChaseRange)
            {
                if (npc.CurrentStrategy != chase) npc.SetStrategy(chase);
            }
            else
            {
                if (npc.CurrentStrategy != wander) npc.SetStrategy(wander);
            }
            
            npc.Update();
            Thread.Sleep(300);
        }
    }
}