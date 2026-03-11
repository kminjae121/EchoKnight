using System.Collections.Generic;
using _Code.KMJ.UnitSystem.involveUnitSO;
using Code.Core;
using Code.Items;
using Code.UnitSystem.SkillSystem;

namespace _Code.Core.Managers
{
    public class GoodsManager : MonoSingleton<GoodsManager>
    {
        public HavingSkillSO havingSkillSO;
        public List<UnitSkillStorageSO> ownSkillStorage;
        private Dictionary<UnitType, UnitSkillStorageSO> storageDict = new Dictionary<UnitType, UnitSkillStorageSO>();
        public List<SkillSO> skills;
        public List<ItemSO> items;
        
        protected override void Awake()
        {
            base.Awake();
            ownSkillStorage.ForEach(storage =>
            {
                storageDict.Add(storage.uniType, storage);
            });
        }

        public void AddSkill()
        {
            skills.ForEach(skill =>
            {
                havingSkillSO.HaveSkills.Add(skill);
                
                switch (skill.unitType)
                {
                    case UnitType.Knight:
                        storageDict.GetValueOrDefault(UnitType.Knight).skills.Add(skill);
                        break;
                    case UnitType.Archer:
                        storageDict.GetValueOrDefault(UnitType.Archer).skills.Add(skill);
                        break;
                    case UnitType.Bandlt:
                        storageDict.GetValueOrDefault(UnitType.Bandlt).skills.Add(skill);
                        break;
                    case UnitType.Magician:
                        storageDict.GetValueOrDefault(UnitType.Magician).skills.Add(skill);
                        break;
                    case UnitType.None:
                        break;
                }
            });

            skills.Clear();
        }
        
        public void GetItem(ItemSO item)
        {
            items.Add(item);
        }

        public void GetSkill(SkillSO skill)
        {
            skills.Add(skill);
        }
    }
}