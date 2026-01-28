using System.Collections.Generic;
using System.Linq;
using Code.Core.Debugs;
using Code.Lobby;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class CarriageUI : MonoBehaviour
    {
        [SerializeField] private Image carriagePanel;
        [SerializeField] private CharacterRecruitmentUI characterPanel;
        [SerializeField] private Button backButton;
        [SerializeField] private Transform characterContainer;

        private List<CarriageCharacterUI> characterList;

        private void Start()
        {
            carriagePanel.gameObject.SetActive(false);
            
            backButton.onClick.AddListener(HandleBackButton);
            
            // 임시
            characterList = characterContainer.GetComponentsInChildren<CarriageCharacterUI>().ToList();

            foreach (var character in characterList)
                character.OnUnitButtonClicked += HandleCharacterButton;
        }

        private void OnDestroy()
        {
            backButton.onClick.RemoveListener(HandleBackButton);
            
            foreach (var character in characterList)
                character.OnUnitButtonClicked -= HandleCharacterButton;
        }

        private void HandleBackButton()
        {
            carriagePanel.gameObject.SetActive(false);
        }

        private void HandleCharacterButton(RecruitUnitData data)
        {
            UnityLogger.Log("캐릭터 버튼 클릭");
            characterPanel.SetCharacter(data.unitInfoSO);
            characterPanel.gameObject.SetActive(true);
        }
    }
}