using System;
using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class EnemyHpUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI damageTxt;
        [SerializeField] private TextMeshProUGUI damageInfoTxt;
        [SerializeField] private Slider hpSlider;
        [SerializeField] private GameObject _enemyInfo;

        [SerializeField] private Image enemyImage;
        
        private void Awake()
        {
            Bus<EnemyHpInfo>.Subscribe(SetHp);
        }

        public void SetHp(EnemyHpInfo evt)
        {
            if (evt.isActive == false)
            {
                _enemyInfo.SetActive(false);    
            }
            else
            {
                _enemyInfo.SetActive(true);

                float hp = evt.hp - evt.damage - evt.plusDamage;

                if (hp <= 0)
                {
                    hp = 0;
                }

                enemyImage.sprite = evt.sprite;
                
                damageTxt.text = $"{hp}";
                damageInfoTxt.text = $"{evt.hp} - ({evt.damage} + {evt.plusDamage})";

                hpSlider.value = evt.lastValue;   
            }
        }
    }
}