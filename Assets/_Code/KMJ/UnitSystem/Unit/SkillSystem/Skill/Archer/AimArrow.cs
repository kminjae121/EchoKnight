using System.Collections;
using _Code.KMJ.UnitSystem.Unit.UnitComponent;
using Code.Core.Events.Bus;
using Code.UnitSystem.SkillSystem;
using UnitSystem;
using UnityEngine;

public class AimArrow : BasicUnitSkill
{
    [SerializeField] private GameObject _ArrowPrefab;
    
    private UnitAnimation animtionCompo;

    private GameObject _target;

    private bool isHorizontal = false;
    protected override void Start()
    {
        base.Start();
        triggerCompo.OnAimArrowTrigger += MakeArrow;
        triggerCompo.OnAimArrowEndTrigger += SkillEnd;
        skillEvent.AddListener(AttackAction);
        animtionCompo = _owner.GetUnitCompo<UnitAnimation>();
    }

    protected override void OnDestroy()
    {
        triggerCompo.OnAimArrowTrigger -= MakeArrow;
        triggerCompo.OnAimArrowEndTrigger -= SkillEnd;
        skillEvent.RemoveListener(AttackAction);
        base.OnDestroy();
    }

    public void AttackAction(GameObject target)
    {
        StartCoroutine(FireArrowAction());
        skillStartEvent?.Invoke();
        _target = target;
    }
    
    private IEnumerator FireArrowAction()
    {
        yield return new WaitForSeconds(0.3f);
        yield return new WaitForSeconds(0.1f);
        animtionCompo.PlaySelectAnimation("AIM");
    }

    public override void Update()
    {
        base.Update();
        if (UnityEngine.Input.GetKeyDown(KeyCode.R))
        {
            if (_isAct)
            {
                if (isHorizontal == false)
                {
                    float x = _verticalCheckBoxSize.x;

                    _verticalCheckBoxSize.x = _verticalCheckBoxSize.z;
                    _verticalCheckBoxSize.z = x;

                    ShowSkillRange();
                }
                else
                {
                    float z = _verticalCheckBoxSize.z;

                    _verticalCheckBoxSize.z = _verticalCheckBoxSize.x;
                    _verticalCheckBoxSize.x = z;
                    ShowSkillRange();
                }
            }
        }
    }

    private void SkillEnd()
    {
        skillEndEvent?.Invoke();
        animtionCompo.PlaySelectAnimation("IDLE");
        Bus<SetAtkUIEvent>.Raise(new SetAtkUIEvent(false));
        Bus<UnitCamSettingEvent>.Raise(new UnitCamSettingEvent(null, false,new Vector3(0.1f,0.1f,0.1f)));
        Bus<UnitSetMoveEvent>.Raise(new UnitSetMoveEvent(true));
    }
    
    public void MakeArrow()
    {
        impulseSource.GenerateImpulse(0.8f);  
        Vector3 pos = _unitBase.transform.position;

        pos.y += 2f;
    
        GameObject shootItem = Instantiate(_ArrowPrefab, pos, Quaternion.identity);
        ShootItem shootItemCompo = shootItem.GetComponent<ShootItem>();
        shootItemCompo.SetTarget(_target);
        shootItemCompo.SetDamageData(_damageData);
        Vector3 slashRot = transform.rotation.eulerAngles;
    
        shootItem.transform.rotation = Quaternion.Euler(slashRot);
        _target = null;
    }
}