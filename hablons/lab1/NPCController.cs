using UnityEngine;
using UnityEngine.AI;

public interface INpcStrategy
{
    void Enter(NPCController npc);
    void Execute(NPCController npc);
    void Exit(NPCController npc);
}

public sealed class WanderStrategy : INpcStrategy
{
    private float nextPointTime;

    public void Enter(NPCController npc)
    {
        npc.Agent.speed = npc.WanderSpeed;
        npc.Agent.stoppingDistance = 0.2f;
        npc.SetAnimationSpeed(0.35f);

        PickNewPoint(npc);
    }

    public void Execute(NPCController npc)
    {
        bool reachedPoint =
            !npc.Agent.pathPending &&
            npc.Agent.remainingDistance <= npc.Agent.stoppingDistance + 0.2f;

        if (reachedPoint || Time.time >= nextPointTime)
        {
            PickNewPoint(npc);
        }
    }

    public void Exit(NPCController npc)
    {
        // Здесь можно остановить звуки шагов
    }

    private void PickNewPoint(NPCController npc)
    {
        Vector3 randomPoint = npc.HomePosition + Random.insideUnitSphere * npc.WanderRadius;
        randomPoint.y = npc.transform.position.y;

        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, npc.WanderRadius, NavMesh.AllAreas))
        {
            npc.Agent.SetDestination(hit.position);
        }

        nextPointTime = Time.time + Random.Range(npc.MinWanderWait, npc.MaxWanderWait);
    }
}

public sealed class ChaseStrategy : INpcStrategy
{
    public void Enter(NPCController npc)
    {
        npc.Agent.speed = npc.ChaseSpeed;
        npc.Agent.stoppingDistance = npc.AttackDistance;
        npc.SetAnimationSpeed(1f);
    }

    public void Execute(NPCController npc)
    {
        if (npc.Player == null)
            return;

        npc.Agent.SetDestination(npc.Player.position);

        if (npc.DistanceToPlayer <= npc.AttackDistance)
        {
            npc.OnPlayerReached();
        }
    }

    public void Exit(NPCController npc)
    {
        npc.Agent.stoppingDistance = 0.2f;
    }
}

public sealed class EvadeStrategy : INpcStrategy
{
    private float nextUpdateTime;

    public void Enter(NPCController npc)
    {
        npc.Agent.speed = npc.EvadeSpeed;
        npc.Agent.stoppingDistance = 0.2f;
        npc.SetAnimationSpeed(1.2f);

        PickEscapePoint(npc);
    }

    public void Execute(NPCController npc)
    {
        bool reachedPoint =
            !npc.Agent.pathPending &&
            npc.Agent.remainingDistance <= npc.Agent.stoppingDistance + 0.3f;

        if (reachedPoint || Time.time >= nextUpdateTime)
        {
            PickEscapePoint(npc);
        }
    }

    public void Exit(NPCController npc)
    {
        // Здесь можно вернуть обычную анимацию или отключить эффект паники.
    }

    private void PickEscapePoint(NPCController npc)
    {
        Vector3 directionFromDanger = npc.transform.position - npc.ThreatPosition;

        if (directionFromDanger.sqrMagnitude < 0.01f)
        {
            directionFromDanger = -npc.transform.forward;
        }

        Vector3 escapeTarget =
            npc.transform.position + directionFromDanger.normalized * npc.EvadeDistance;

        if (NavMesh.SamplePosition(escapeTarget, out NavMeshHit hit, npc.EvadeDistance, NavMesh.AllAreas))
        {
            npc.Agent.SetDestination(hit.position);
        }

        nextUpdateTime = Time.time + 0.35f;
    }
}

public sealed class NPCController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform danger;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator animator;

    [Header("Distances")]
    [SerializeField] private float chaseRange = 8f;
    [SerializeField] private float losePlayerRange = 12f;
    [SerializeField] private float dangerRange = 5f;
    [SerializeField] private float safeDangerRange = 8f;
    [SerializeField] private float attackDistance = 1.5f;

    [Header("Movement Speeds")]
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float evadeSpeed = 5f;

    [Header("Wandering")]
    [SerializeField] private float wanderRadius = 10f;
    [SerializeField] private float minWanderWait = 2f;
    [SerializeField] private float maxWanderWait = 5f;

    [Header("Evading")]
    [SerializeField] private float evadeDistance = 7f;

    [Header("Strategy Switching")]
    [SerializeField] private float minStrategyDuration = 0.5f;
    [SerializeField] private bool logStrategyChanges = true;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;

    private readonly INpcStrategy wanderStrategy = new WanderStrategy();
    private readonly INpcStrategy chaseStrategy = new ChaseStrategy();
    private readonly INpcStrategy evadeStrategy = new EvadeStrategy();

    private INpcStrategy currentStrategy;

    private Vector3 homePosition;
    private float lastStrategyChangeTime = -999f;
    private float nextAttackTime;

    public Transform Player => player;
    public Transform Danger => danger;
    public NavMeshAgent Agent => agent;

    public Vector3 HomePosition => homePosition;

    public float WanderSpeed => wanderSpeed;
    public float ChaseSpeed => chaseSpeed;
    public float EvadeSpeed => evadeSpeed;

    public float WanderRadius => wanderRadius;
    public float MinWanderWait => minWanderWait;
    public float MaxWanderWait => maxWanderWait;

    public float EvadeDistance => evadeDistance;
    public float AttackDistance => attackDistance;

    public float DistanceToPlayer =>
        player == null ? float.PositiveInfinity : Vector3.Distance(transform.position, player.position);

    public float DistanceToDanger =>
        danger == null ? float.PositiveInfinity : Vector3.Distance(transform.position, danger.position);

    public Vector3 ThreatPosition =>
        danger != null ? danger.position : transform.position - transform.forward;

    private void Awake()
    {
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        homePosition = transform.position;

        if (losePlayerRange < chaseRange)
        {
            losePlayerRange = chaseRange + 1f;
        }

        if (safeDangerRange < dangerRange)
        {
            safeDangerRange = dangerRange + 1f;
        }
    }

    private void Start()
    {
        ChangeStrategy(wanderStrategy, true);
    }

    private void Update()
    {
        INpcStrategy desiredStrategy = ChooseStrategy();

        ChangeStrategy(desiredStrategy);

        currentStrategy?.Execute(this);
    }

    private INpcStrategy ChooseStrategy()
    {
        if (danger != null)
        {
            bool dangerIsClose = DistanceToDanger <= dangerRange;
            bool stillEscaping = currentStrategy == evadeStrategy && DistanceToDanger <= safeDangerRange;

            if (dangerIsClose || stillEscaping)
            {
                return evadeStrategy;
            }
        }

        if (player != null)
        {
            bool playerIsClose = DistanceToPlayer <= chaseRange;
            bool stillChasing = currentStrategy == chaseStrategy && DistanceToPlayer <= losePlayerRange;

            if (playerIsClose || stillChasing)
            {
                return chaseStrategy;
            }
        }

        return wanderStrategy;
    }

    private void ChangeStrategy(INpcStrategy nextStrategy, bool force = false)
    {
        if (currentStrategy == nextStrategy)
            return;

        if (!force && currentStrategy != null)
        {
            bool changedTooRecently = Time.time - lastStrategyChangeTime < minStrategyDuration;

            if (changedTooRecently)
                return;
        }

        currentStrategy?.Exit(this);

        currentStrategy = nextStrategy;
        lastStrategyChangeTime = Time.time;

        currentStrategy.Enter(this);

        if (logStrategyChanges)
        {
            Debug.Log($"{name}: новая стратегия — {currentStrategy.GetType().Name}");
        }
    }

    public void SetAnimationSpeed(float value)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", value);
        }
    }

    public void OnPlayerReached()
    {
        if (Time.time < nextAttackTime)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Debug.Log($"{name}: атакует игрока");

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 center = Application.isPlaying ? homePosition : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dangerRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, wanderRadius);
    }
}