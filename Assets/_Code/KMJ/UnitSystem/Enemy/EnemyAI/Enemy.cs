using System.Collections;
using _Code.Core.Managers;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using Code.UnitSystem.SkillSystem;
using EnemySystem;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Events;
using Action = System.Action;

public class Enemy : Unit
{
    [Header("Enemy Refs")]
    [SerializeField] private BehaviorGraphAgent behaviorAgent;
    [SerializeField] private UnitAnimationTrigger triggerCompo;
    [SerializeField] private EnemyGridMovingSystem moveCompo;
    [SerializeField] private SkillComponent skillCompo;
    [SerializeField] private ParticleSystem bloodParticles;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        // Auto-fetch components if null
        if (moveCompo == null) moveCompo = GetComponent<EnemyGridMovingSystem>();
        if (skillCompo == null) skillCompo = GetComponent<SkillComponent>();

        if (triggerCompo != null)
        {
            triggerCompo.OnEnemyAnimationEndTrigger += ChangeIdle;
            triggerCompo.OnEnemyDieEndTrigger += OnDieAnimationFinished;
        }
    }

    protected override void OnDestroy()
    {
        if (triggerCompo != null)
        {
            triggerCompo.OnEnemyAnimationEndTrigger -= ChangeIdle;
            triggerCompo.OnEnemyDieEndTrigger -= OnDieAnimationFinished;
        }
        base.OnDestroy();
    }

    private void Awake()
    {
        if (behaviorAgent == null)
            behaviorAgent = GetComponent<BehaviorGraphAgent>();
        
        SetBlackboardVariable("IsMyTurn", false);
    }

    private void Start()
    {
        Bus<UnitSpawnEvent>.Raise(new UnitSpawnEvent(this));
        ChangeIdle();
    }

    private void SetBlackboardVariable(string name, object value)
    {
        if (behaviorAgent != null && behaviorAgent.BlackboardReference != null)
        {
            behaviorAgent.BlackboardReference.SetVariableValue(name, value);
        }
    }
    
    public void MoveToTarget(Vector3 targetPos, Action onComplete)
    {
        if (moveCompo == null)
        {
            Debug.LogError($"[Enemy] {name}: EnemyGridMovingSystem 없음.");
            onComplete?.Invoke();
            return;
        }

        if (AnimationCompo != null) 
            AnimationCompo.PlaySelectAnimation("MOVE");
        
        StartCoroutine(ProcessMove(targetPos, onComplete));
    }

    private IEnumerator ProcessMove(Vector3 targetPos, Action onComplete)
    {
        bool isMoved = false;
        UnityAction endAction = () => isMoved = true;

        if (moveCompo.OnMoveEndEvent == null) 
            moveCompo.OnMoveEndEvent = new UnityEvent();

        moveCompo.OnMoveEndEvent.AddListener(endAction);
        
        moveCompo.MoveTowardsTarget(targetPos);

        while (!isMoved)
        {
            yield return null;
        }

        moveCompo.OnMoveEndEvent.RemoveListener(endAction);

        ChangeIdle();
        onComplete?.Invoke();
    }
    
    public void UseSkill(string skillName, GameObject target, Action onComplete)
    {
        if (skillCompo == null || skillCompo.skills == null)
        {
            Debug.LogError($"[Enemy] {name}: SkillComponent 없음.");
            onComplete?.Invoke();
            return;
        }

        if (!skillCompo.skills.TryGetValue(skillName, out BaseSkill skill))
        {
            Debug.LogWarning($"[Enemy] 스킬 '{skillName}' 없음. 첫 번째 스킬 사용 시도.");
            var enumerator = skillCompo.skills.Values.GetEnumerator();
            if (enumerator.MoveNext()) skill = enumerator.Current;
        }

        if (skill != null)
        {
            StartCoroutine(ProcessSkill(skill, target, onComplete));
        }
        else
        {
            onComplete?.Invoke();
        }
    }

    private IEnumerator ProcessSkill(BaseSkill skill, GameObject target, Action onComplete)
    {
        bool isSkillEnded = false;
        UnityAction endAction = () => isSkillEnded = true;

        skill.skillEndEvent.AddListener(endAction);

        skill.ForceUseSkill(target);

        while (!isSkillEnded)
        {
            yield return null;
        }

        skill.skillEndEvent.RemoveListener(endAction);
        ChangeIdle();
        onComplete?.Invoke();
    }

    private void OnDieAnimationFinished()
    {
        gameObject.SetActive(false);
        if (StageManager.Instance != null)
            StageManager.Instance.RemoveEnemy(this.gameObject);
    }

    private void ChangeIdle()
    {
        if (AnimationCompo != null)
            AnimationCompo.PlaySelectAnimation("IDLE");
    }

    protected override void Dead()
    {
        DeadEnemy();
        base.Dead();
    }

    public override void OnTurnStart()
    {
        base.OnTurnStart();
        SetBlackboardVariable("IsMyTurn", true);
        
        for (int i = 0; i <= 2; i++)
            Bus<SkillUIEvent>.Raise(new SkillUIEvent(i, null, null, null));
    }

    public void TurnEnd()
    {
        Bus<UnitTurnEndEvent>.Raise(new UnitTurnEndEvent(this));
    }

    protected override void Hit()
    {
        if (bloodParticles != null)
        {
            bloodParticles.gameObject.SetActive(true);
            bloodParticles.Play();
        }

        if (AnimationCompo != null)
        {
            AnimationCompo.RestartFromEntry();
            AnimationCompo.PlaySelectAnimation("HIT");
        }
        base.Hit();
    }

    public void DeadEnemy()
    {
        if (AnimationCompo != null)
            AnimationCompo.PlaySelectAnimation("DIE");
    }
}