using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Core.Managers;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.UI
{
    public class EventUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI mainTxt;
        [SerializeField] private TextMeshProUGUI selectBtnTxt;
        [SerializeField] private TextMeshProUGUI skipBtnTxt;
        [SerializeField] private TextMeshProUGUI popUpTxt;

        [SerializeField] private Button skipBtn;
        [SerializeField] private Button selectBtn;

        [SerializeField] private List<string> eventTxts;
        [SerializeField] private List<string> applyTxts;
        [SerializeField] private List<string> cancelTxts;

        [SerializeField] private float activeTime = 2;

        [SerializeField] private List<string> failTxt;
        [SerializeField] private List<string> successTxt;

        [SerializeField] private Image thisObjectImg;
        private void OnEnable()
        {
            int randValue = Random.Range(0, eventTxts.Count);
            thisObjectImg = GetComponent<Image>();
            
            selectBtnTxt.text = applyTxts[randValue];
            skipBtnTxt.text = cancelTxts[randValue];

            DOTween.Sequence()
                .Append(popUpTxt.transform.DOScale(1, 1f))
                .Append(popUpTxt.DOFade(0, 0.6f))
                .Append(thisObjectImg.DOFade(1, 1f))
                .Append(mainTxt.DoText(eventTxts[randValue], activeTime))
                .Append(selectBtn.transform.DOScale(1, 1f))
                .Append(skipBtn.transform.DOScale(1, 1f));
            
            int randomValue = Random.Range(0, 3);
            
            skipBtn.onClick.AddListener(HandleSkipBtn);
            selectBtn.onClick.AddListener(() =>HandleSelectBtn(randomValue,randValue));
        }

        private void HandleSkipBtn()
        {
            skipBtn.gameObject.SetActive(false);
            selectBtn.gameObject.SetActive(false);
            DOTween.Sequence()
                .Append(mainTxt.DoText("지나쳤습니다.", activeTime))
                .AppendInterval(1f)
                .Append(mainTxt.DoText("", 0))
                .AppendInterval(0.3f)
                .Append(thisObjectImg.DOFade(0, 1f));
        }

        private void OnDisable()
        {
            skipBtn.onClick.RemoveAllListeners();
            selectBtn.onClick.RemoveAllListeners();
        }

        public void HandleSelectBtn(int value, int randomValue)
        {
            if (value == 1)
            {
                skipBtn.gameObject.SetActive(false);
                selectBtn.gameObject.SetActive(false);
                DOTween.Sequence()
                    .Append(mainTxt.DoText(failTxt[randomValue], activeTime))
                    .AppendInterval(1f)
                    .Append(mainTxt.DoText("", 0))
                    .AppendInterval(0.3f)
                    .Append(thisObjectImg.DOFade(0, 1f));
            }
            else
            {
                skipBtn.gameObject.SetActive(false);
                selectBtn.gameObject.SetActive(false);
                DOTween.Sequence()
                    .Append(mainTxt.DoText(successTxt[randomValue], activeTime))
                    .AppendInterval(1f)
                    .Append(mainTxt.DoText("", 0))
                    .AppendInterval(0.3f)
                    .Append(thisObjectImg.DOFade(0, 1f));
            }
        }
        
    }
}