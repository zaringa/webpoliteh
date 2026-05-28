public class NPC
{
    public Vector3 Position { get; set; }
    public Vector3? PlayerPosition { get; set; }
    public Vector3? DangerPosition { get; set; }
    
    public float ChaseRange { get; } = 8f;
    public float DangerRange { get; } = 5f;
    public float AttackDistance { get; } = 1.5f;
    
    public INpcStrategy CurrentStrategy { get; private set; }
    
    public void SetStrategy(INpcStrategy strategy)
    {
        CurrentStrategy?.Exit(this);
        CurrentStrategy = strategy;
        CurrentStrategy?.Enter(this);
        Console.WriteLine($"[NPC] Стратегия: {strategy.Name}");
    }
    
    public void Update()
    {
        CurrentStrategy?.Execute(this);
    }
    
    public float GetDistanceTo(Vector3? target) => 
        target.HasValue ? Vector3.Distance(Position, target.Value) : float.PositiveInfinity;
    
    public void Attack() => Console.WriteLine($"[NPC] Атака игрока!");
    public void WanderTo(Vector3 point) => Console.WriteLine($"[NPC] Идёт к {point}");
    public void ChaseTo(Vector3 point) => Console.WriteLine($"[NPC] приследует {point}");
    public void EvadeTo(Vector3 point) => Console.WriteLine($"[NPC] убегает к {point}");
}

public struct Vector3
{
    public float X, Y, Z;
    public Vector3(float x, float y, float z) { X = x; Y = y; Z = z; }
    public static float Distance(Vector3 a, Vector3 b) => 
        (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2) + Math.Pow(a.Z - b.Z, 2));
    public override string ToString() => $"({X:F1}, {Y:F1}, {Z:F1})";
}