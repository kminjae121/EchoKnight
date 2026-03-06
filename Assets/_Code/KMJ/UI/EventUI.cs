using System;
using System.Collections;
using System.Collections.Generic;
using _Code.Core.Managers;
using _Code.KMJ.SO;
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
        [SerializeField] private Image eventImg;

        [SerializeField] private Button skipBtn;
        [SerializeField] private Button selectBtn;
        

        [SerializeField] private float activeTime = 1;

        [SerializeField] private List<EventTextSO> eventTexts;

        [SerializeField] private Image thisObjectImg;
        private void OnEnable()
        {
            int randValue = Random.Range(0, eventTexts.Count);
            thisObjectImg = GetComponent<Image>();
            
            selectBtnTxt.text = eventTexts[randValue].ApplyTxt;
            skipBtnTxt.text = eventTexts[randValue].CancelTxt;
            
            eventImg.sprite = eventTexts[randValue].EventImg;

            DOTween.Sequence()
                .Append(popUpTxt.transform.DOScale(1, 1f))
                .Append(popUpTxt.DOFade(0, 0.6f))
                .Append(thisObjectImg.DOFade(1, 0.4f))
                .Append(eventImg.DOFade(255, 0.4f))
                .Append(mainTxt.DoText(eventTexts[randValue].MainTxt, activeTime))
                .Append(selectBtn.transform.DOScale(1, 0.5f))
                .Append(skipBtn.transform.DOScale(1, 0.5f));
            
            int randomValue = Random.Range(0, 3);
            
            skipBtn.onClick.AddListener(() => HandleSkipBtn(randValue));
            selectBtn.onClick.AddListener(() =>HandleSelectBtn(randomValue, randValue));
        }

        private void HandleSkipBtn(int value)
        {
            mainTxt.text = eventTexts[value].SkipTxt;
            
            skipBtn.gameObject.SetActive(false);
            selectBtn.gameObject.SetActive(false);
            DOTween.Sequence()
                .Append(mainTxt.DoText(eventTexts[value].SkipTxt, activeTime))
                .AppendInterval(1)
                .Append(eventImg.DOFade(0, 0.5f))
                .Append(mainTxt.RemoveText(1))
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
                mainTxt.text = eventTexts[randomValue].FailTxt;
                
                skipBtn.gameObject.SetActive(false);
                selectBtn.gameObject.SetActive(false);
                DOTween.Sequence()
                    .Append(mainTxt.DoText(eventTexts[randomValue].FailTxt, activeTime))
                    .AppendInterval(1f)
                    .Append(eventImg.DOFade(0, 0.5f))
                    .Append(mainTxt.RemoveText(1))
                    .AppendInterval(0.2f)
                    .Append(thisObjectImg.DOFade(0, 0.5f));
            }
            else
            {
                skipBtn.gameObject.SetActive(false);
                selectBtn.gameObject.SetActive(false);

                mainTxt.text = eventTexts[randomValue].SuccessTxt;
                
                DOTween.Sequence()
                    .Append(mainTxt.DoText(eventTexts[randomValue].SuccessTxt, activeTime))
                    .AppendInterval(1f)
                    .Append(eventImg.DOFade(0, 0.5f))
                    .Append(mainTxt.RemoveText(1))
                    .AppendInterval(0.2f)
                    .Append(thisObjectImg.DOFade(0, 0.5f));
                
            }
        }
        
    }
}