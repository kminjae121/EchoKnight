using System.Collections.Generic;
using Code.Expedition;
using Code.Expedition.Data;
using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Expedition.UI
{
    public class ExpeditionEventPopup : MonoBehaviour
    {
        [SerializeField] private GameObject contentRoot;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image eventImage;
        [SerializeField] private Transform choiceContainer;
        [SerializeField] private Button choiceButtonPrefab;

        private void Awake()
        {
            Bus<ShowEventPopupEvent>.Subscribe(OnShowEventPopup);
            ClosePopup();
        }

        private void OnDestroy()
        {
            Bus<ShowEventPopupEvent>.Unsubscribe(OnShowEventPopup);
        }

        private void OnShowEventPopup(ShowEventPopupEvent evt)
        {
            OpenPopup(evt.EventData);
        }

        private void OpenPopup(EventNodeSO data)
        {
            contentRoot.SetActive(true);
            titleText.text = data.title;
            descriptionText.text = data.description;
            
            if (data.eventImage != null)
            {
                eventImage.sprite = data.eventImage;
                eventImage.gameObject.SetActive(true);
            }
            else
            {
                eventImage.gameObject.SetActive(false);
            }

            CreateChoices(data.choices);
        }

        private void CreateChoices(List<EventChoice> choices)
        {
            foreach (Transform child in choiceContainer)
            {
                Destroy(child.gameObject);
            }

            if (choices == null) return;

            foreach (var choice in choices)
            {
                Button btn = Instantiate(choiceButtonPrefab, choiceContainer);
                TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (btnText != null) btnText.text = choice.choiceText;

                btn.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }

        private void OnChoiceSelected(EventChoice choice)
        {
            Debug.Log($"이벤트 선택: {choice.choiceText}");
            
            ClosePopup();
            ExpeditionManager.Instance.CompleteCurrentNode();
        }

        private void ClosePopup()
        {
            contentRoot.SetActive(false);
        }
    }
}