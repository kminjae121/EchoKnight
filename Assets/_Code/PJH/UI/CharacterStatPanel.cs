using System.Collections.Generic;
using Code.Core.Events.Bus;
using Code.UnitSystem;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CharacterStatPanel : Panel
    {
        [Header("Equipped Items")]
        [SerializeField] private List<Image> skillIcons;
        [SerializeField] private List<Image> artifactIcons;
        [SerializeField] private Sprite emptySlotSprite;

        [Header("Visual & HP")]
        [SerializeField] private Transform modelSpawnPoint;
        [SerializeField] private Image hpBarFill;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private float hpTweenDuration = 0.3f;

        [Header("Stat & Info")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI classText;
        [SerializeField] private TextMeshProUGUI atkText;
        [SerializeField] private TextMeshProUGUI defText;
        [SerializeField] private TextMeshProUGUI moveSpeedText;
        [SerializeField] private TextMeshProUGUI descriptionText;

        private UnitState _currentUnit;
        private Tween _hpTween;
        private GameObject _spawnedModel;

        public override void Awake()
        {
            base.Awake();
            Bus<CharacterInfoEvent>.Subscribe(HandleCharacterInfo);
        }

        private void OnDestroy()
        {
            Bus<CharacterInfoEvent>.Unsubscribe(HandleCharacterInfo);
            UnsubscribeHpEvent();
        }

        private void HandleCharacterInfo(CharacterInfoEvent evt)
        {
            UnsubscribeHpEvent();

            _currentUnit = evt.Unit;
            if (_currentUnit != null)
            {
                _currentUnit.CurrentHp.OnValueChanged += RefreshHpBar;
                RefreshAllUI();
            }
        }

        private void UnsubscribeHpEvent()
        {
            if (_currentUnit != null)
                _currentUnit.CurrentHp.OnValueChanged -= RefreshHpBar;
        }

        private void RefreshAllUI()
        {
            RefreshInfoTexts();
            RefreshHpBar(0f, _currentUnit.CurrentHp.Value);
            RefreshSkillSlots();
            RefreshArtifactSlots();
            SpawnCharacterModel();
        }

        private void RefreshInfoTexts()
        {
            var data = _currentUnit.Data;
            
            nameText.text = data.UnitName;
            classText.text = data.UnitType.ToString();
            atkText.text = data.AtkDamage.ToString("F1");
            defText.text = data.DefensivePower.ToString("F1");
            moveSpeedText.text = data.MoveSpeed.ToString("F1");
            descriptionText.text = data.UnitDescription;
        }

        private void RefreshHpBar(float prevValue, float nextValue)
        {
            float maxHp = _currentUnit.Data.Maxhealth;
            hpText.text = $"{nextValue:F0} / {maxHp:F0}";

            float fillAmount = maxHp > 0 ? nextValue / maxHp : 0f;

            _hpTween?.Kill();
            _hpTween = hpBarFill
                .DOFillAmount(fillAmount, hpTweenDuration)
                .SetEase(Ease.OutCubic);
        }

        private void RefreshSkillSlots()
        {
            var data = _currentUnit.Data;
            
            for (int i = 0; i < skillIcons.Count; i++)
            {
                if (data.SkillStorage != null && i < data.SkillStorage.skills.Count)
                {
                    skillIcons[i].sprite = data.SkillStorage.skills[i].skillUIImage;
                    skillIcons[i].color = Color.white;
                }
                else
                {
                    skillIcons[i].sprite = emptySlotSprite;
                }
            }
        }

        private void RefreshArtifactSlots()
        {
            for (int i = 0; i < artifactIcons.Count; i++)
            {
                artifactIcons[i].sprite = emptySlotSprite;
            }
        }

        private void SpawnCharacterModel()
        {
            if (_spawnedModel != null)
            {
                Destroy(_spawnedModel);
                _spawnedModel = null;
            }

            if (modelSpawnPoint == null)
            {
                Debug.LogWarning("모델 생성 포인트가 설정되지 않았습니다.");
                return;
            }

            var spawnData = _currentUnit.Data.UnitSpawn;
            
            if (spawnData != null && spawnData.UnitPrefab != null)
            {
                _spawnedModel = Instantiate(spawnData.UnitPrefab, modelSpawnPoint);
                _spawnedModel.transform.localPosition = Vector3.zero;
                _spawnedModel.transform.localRotation = Quaternion.identity;
                
                SetLayerRecursively(_spawnedModel, LayerMask.NameToLayer("UI"));
            }
        }

        private void SetLayerRecursively(GameObject obj, int newLayer)
        {
            if (obj == null) return;
            
            obj.layer = newLayer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, newLayer);
            }
        }
    }
}