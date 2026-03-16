using System.Collections.Generic;
using Code.Core.Events.Bus;
using GondrLib.Dependencies;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;

namespace Code.UI
{
    public class BattleUI : MonoBehaviour
    {
        [SerializeField] private PoolingItemSO attackSlotPrefab;
        [SerializeField] private RectTransform attackTrm;
        [Inject] private PoolManagerMono _poolManager;

        private readonly List<AttackSlotUI> _slots = new();

        private void Awake()
        {
            Bus<SkillUIEvent>.Subscribe(SetSkill);
            Bus<SetAtkUIEvent>.Subscribe(SetAttackUI);
        }

        private void OnDestroy()
        {
            Bus<SkillUIEvent>.Unsubscribe(SetSkill);
            Bus<SetAtkUIEvent>.Unsubscribe(SetAttackUI);
        }

        private void SetAttackUI(SetAtkUIEvent evt)
        {
        }

        private void SetSkill(SkillUIEvent evt)
        {
            int skillCount = evt.Skills.Count;

            // 부족한 슬롯 생성
            while (_slots.Count < skillCount)
            {
                var slot = _poolManager.Pop<AttackSlotUI>(attackSlotPrefab);
                slot.transform.SetParent(attackTrm, false);
                slot.Initialize(this);
                _slots.Add(slot);
            }

            // 데이터 세팅
            for (int i = 0; i < skillCount; ++i)
            {
                _slots[i].gameObject.SetActive(true);
                _slots[i].SetSkill(evt.Skills[i], evt.SkillCompo);
            }

            // 남는 슬롯 비활성화
            for (int i = skillCount; i < _slots.Count; ++i)
                _slots[i].gameObject.SetActive(false);
        }
    }
}