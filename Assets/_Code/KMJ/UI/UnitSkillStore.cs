using System;
using System.Collections.Generic;
using _Code.KMJ.UnitSystem.involveUnitSO;
using Code.Managers;
using Code.UnitSystem.SkillSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.UI
{
    public class UnitSkillStore : MonoBehaviour
    {
        [SerializeField] private List<SkillSO> skills;

        [SerializeField] private HavingSkillSO havingSkillSO;

        [SerializeField] private List<UnitOwnSkillStorageSO> ownSkillStorage;
        
        private Dictionary<UnitType, UnitOwnSkillStorageSO> storageDict = new Dictionary<UnitType, UnitOwnSkillStorageSO>();

        #region UI

        [SerializeField] private List<GameObject> skillUI = new List<GameObject>();
        [SerializeField] private List<Image> skillImages;
        [SerializeField] private List<TextMeshProUGUI> skillDescription;
        [SerializeField] private List<TextMeshProUGUI> skillOwnUnit;
        [SerializeField] private List<Button> skillBtn;

        #endregion



        private void Start()
        {
            skills.RemoveAll(skill => havingSkillSO.HaveSkills.Contains(skill));
            
            ownSkillStorage.ForEach(storage =>
            {
                storageDict.Add(storage.uniType, storage);
            });

            Show();            
        }

        public void Show()
        {
            skillUI.ForEach(UI =>
            {
                UI.SetActive(true);
            });
            
            int maxCount = skills.Count;
            int[] ran = new int[10];

            while (true)
            {
                ran[0] = Random.Range(0, maxCount);
                ran[1] = Random.Range(0, maxCount);
                ran[2] = Random.Range(0, maxCount);
                ran[3] = Random.Range(0, maxCount);
                ran[4] = Random.Range(0, maxCount);

                if (ran[0] != ran[1] && ran[0] != ran[2] &&
                    ran[0] != ran[3] && ran[0] != ran[4] &&
                    ran[1] != ran[2] && ran[1] != ran[3] &&
                    ran[1] != ran[4] && ran[2] != ran[3] &&
                    ran[2] != ran[4] && ran[3] != ran[4])
                {
                    break;
                }
            }

            for (int i = 0; i < skillUI.Count; i++)
            {
                if (i >= skills.Count)
                {
                    skillBtn[i].gameObject.SetActive(false);
                }
                skillImages[i].sprite = skills[ran[i]].skillUIImage;
                
                skillDescription[i].text = skills[ran[i]].SkillDescription;
                skillOwnUnit[i].text = skills[ran[i]].unitType.ToString();
              
                skillBtn[i].onClick.RemoveAllListeners();
                int idx = i;
                skillBtn[i].onClick.AddListener(() => SkillBtn(ran[idx])); 
                
            }
            
        }

        private void SkillBtn(int idx)
        {
            if (skills.Count <= 0)
                return;

            
            SkillSO skillInfo = skills[idx];

            if (skillInfo.skillPrice < PlayerManager.Instance.Gold)
                return;
            
            PlayerManager.Instance.RemoveGold(skillInfo.skillPrice);
            
            havingSkillSO.HaveSkills.Add(skillInfo);
            
            skills.Remove(skillInfo);

            switch (skillInfo.unitType)
            {
                case UnitType.Archer:
                    UnitOwnSkillStorageSO archerstorageSO = storageDict.GetValueOrDefault(UnitType.Archer);
                    //archerstorageSO.skills.Add(skillInfo);
                    break;
                
                case UnitType.Knight:
                    UnitOwnSkillStorageSO knightstorageSO = storageDict.GetValueOrDefault(UnitType.Knight);
                    //knightstorageSO.skills.Add(skillInfo);
                    break;
                
                case UnitType.Magician:
                    UnitOwnSkillStorageSO magicianstorageSO = storageDict.GetValueOrDefault(UnitType.Magician);
                    //magicianstorageSO.skills.Add(skillInfo);
                    break;
                case UnitType.Bandlt:
                    UnitOwnSkillStorageSO bandltstorageSO = storageDict.GetValueOrDefault(UnitType.Bandlt);
                    //bandltstorageSO.skills.Add(skillInfo);
                    break;
                
                case UnitType.None:
                    break;
            }
            
            EventSystem.current.currentSelectedGameObject.gameObject.SetActive(false);
        }
    }
}