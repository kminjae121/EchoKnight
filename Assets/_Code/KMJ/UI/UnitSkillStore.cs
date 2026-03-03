using System;
using System.Collections.Generic;
using _Code.KMJ.UnitSystem.involveUnitSO;
using Code.UnitSystem.SkillSystem;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.UI
{
    public class UnitSkillStore : MonoBehaviour
    {
        [SerializeField] private List<SkillSO> skills;

        [SerializeField] private HavingSkillSO havingSkillSO;

        [SerializeField] private List<UnitOwnSkillStorageSO> ownSkillStorage;

        #region UI

        [SerializeField] private List<GameObject> skillUI = new List<GameObject>();
        [SerializeField] private List<Sprite> skillImages;
        [SerializeField] private List<string> skillDescription;
        [SerializeField] private List<string> skillOwnUnit;
        [SerializeField] private List<Button> skillBtn;

        #endregion


        private void Awake()
        {
            skills.ForEach(skill =>
            {
                if (havingSkillSO.HaveSkills.Contains(skill))
                {
                    skills.Remove(skill);
                }
            });
        }

        public void Show()
        {
            int maxCount = skills.Count;
            int[] ran = new int[3];

            while (true)
            {
                ran[0] = Random.Range(0, maxCount);
                ran[1] = Random.Range(0, maxCount);
                ran[2] = Random.Range(0, maxCount);

                if (ran[0] != ran[1] && ran[1] != ran[2] && ran[2] != ran[0])
                    break;
            }

            for (int i = 0; i < skillUI.Count; i++)
            {
                skillImages[i] = skills[ran[i]].skillUIImage;
                skillDescription[i] = skills[ran[i]].SkillDescription;
                skillOwnUnit[i] = skills[ran[i]].unitType.ToString();
                skillBtn[i].onClick.AddListener(SkillBtn);
            }
            
            skillUI.ForEach(UI =>
            {
                UI.SetActive(true);
            });
            
        }

        public void SkillBtn()
        {
            //havingSkillSO.HaveSkills.Add(skills[idx]);
            //skills.Remove(skills[idx]);
            
            
        }
    }
}