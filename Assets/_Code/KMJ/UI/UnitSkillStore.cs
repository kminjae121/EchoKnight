using System;
using System.Collections.Generic;
using _Code.Core.Managers;
using _Code.KMJ.UnitSystem.involveUnitSO;
using Code.Items;
using Code.Managers;
using Code.UnitSystem.SkillSystem;
using DG.Tweening;
using Input;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.UI   
{
    public class UnitSkillStore : MonoBehaviour
    {

        [SerializeField] private GameObject storeObject;

        [SerializeField] private Transform storePos;
        [SerializeField] private Transform upPos;
        
        [SerializeField] private InputReader input;
        
        [SerializeField] private List<SkillSO> skills;

        [SerializeField] private HavingSkillSO havingSkillSO;

        [SerializeField] private List<ItemSO> items;

        [SerializeField] private TextMeshProUGUI goldTxt;

        #region StoreUI

        [SerializeField] private List<GameObject> skillUI = new List<GameObject>();
        [SerializeField] private List<Image> skillImages;
        [SerializeField] private List<TextMeshProUGUI> skillDescription;
        [SerializeField] private List<TextMeshProUGUI> skillPrice;
        [SerializeField] private List<TextMeshProUGUI> skillOwnUnit;
        [SerializeField] private List<Button> skillBtn;
        #endregion

        #region ItemUI

        [SerializeField] private List<GameObject> itemUIs = new List<GameObject>();
        [SerializeField] private List<Image> itemImgs;
        [SerializeField] private List<TextMeshProUGUI> itemDes;
        [SerializeField] private List<Button> itemBtns;

        #endregion

        private void Awake()
        {
        }

        private void OnEnable()
        {
            skills.RemoveAll(skill => havingSkillSO.HaveSkills.Contains(skill));

            storeObject.transform.DOMove(storePos.position, 1f);
            Show();
            
            RandomChild();

            input.OnCancelEvent += CancelUI;

            goldTxt.text = $"골드 : {PlayerManager.Instance.Gold.ToString()}";    
        }

        private void OnDisable()
        {
            input.OnCancelEvent -= CancelUI;
        }

        public void CancelUI()
        {
            DOTween.Sequence()
                .Append(storeObject.transform.DOMove(upPos.position, 1f))
                .OnComplete(() => storeObject.SetActive(false));
           
            GoodsManager.Instance.AddSkill();
        }


        private void RandomChild()
        {
            var parent = transform;
            int n = parent.childCount;

            for (int i = 0; i < n; i++)
            {
                parent.GetChild(0).SetSiblingIndex(Random.Range(0, n));
            }
        }
        public void Show()
        {
            skillUI.ForEach(UI =>
            {
                UI.SetActive(true);
            });
            itemUIs.ForEach(UI =>
            {
                UI.SetActive(true);
            });
            
            int[] randomIdx = SetRandomIdx();
            int[] randomItemIdx = SetRandomIdxItem();

            SetSkillUI(randomIdx);
            SetItemUI(randomItemIdx);
        }

        private void SetItemUI(int[] randomIdx)
        {
            for (int i = 0; i < itemUIs.Count; i++)
            {
                if (i >= items.Count)
                {
                    itemBtns[i].gameObject.SetActive(false);
                    continue;
                }
                
                itemImgs[i].sprite = items[randomIdx[i]].itemIcon;
                
                itemDes[i].text = items[randomIdx[i]].itemDesc;
                
                itemBtns[i].GetComponent<StoreItemBtn>().SetItem(items[randomIdx[i]]);
            }
        }

        private int[] SetRandomIdx()
        {
            int maxCount = skills.Count;
            
            int[] idx = new int[10];

            if (maxCount <= 0)
                return idx;
            
            int pickCount = Mathf.Min(maxCount, 5);
            
            int[] pool = new int[maxCount];
            
            for (int i = 0; i < maxCount; i++)
                pool[i] = i;
            
            for (int i = 0; i < pickCount; i++)
            {
                int j = Random.Range(i, maxCount); 
                (pool[i], pool[j]) = (pool[j], pool[i]);
                idx[i] = pool[i];
            }

            return idx;
        }
        
        private int[] SetRandomIdxItem()
        {
            int maxCount = items.Count;
            
            int[] idx = new int[10];

            if (maxCount <= 0)
                return idx;
            
            int pickCount = Mathf.Min(maxCount, 5);
            
            int[] pool = new int[maxCount];
            
            for (int i = 0; i < maxCount; i++)
                pool[i] = i;
            
            for (int i = 0; i < pickCount; i++)
            {
                int j = Random.Range(i, maxCount); 
                (pool[i], pool[j]) = (pool[j], pool[i]);
                idx[i] = pool[i];
            }

            return idx;
        }
        
        
        private void SetSkillUI(int[] ran)
        {
            for (int i = 0; i < skillUI.Count; i++)
            {
                if (i >= skills.Count)
                {
                    skillBtn[i].gameObject.SetActive(false);
                    continue;
                }

                skillImages[i].sprite = skills[ran[i]].skillUIImage;
                
                skillDescription[i].text = skills[ran[i]].SkillDescription;
                
                skillOwnUnit[i].text = skills[ran[i]].unitType.ToString();

                skillPrice[i].text = $"{skills[ran[i]].skillPrice.ToString()} 골드";
              
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
            
            GoodsManager.Instance.GetSkill(skillInfo);
            goldTxt.text = $"골드 : {PlayerManager.Instance.Gold.ToString()}";
            
            EventSystem.current.currentSelectedGameObject.gameObject.SetActive(false);
        }
    }
}