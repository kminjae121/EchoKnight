using DG.Tweening;
using GondrLib.ObjectPool.Runtime;
using TMPro;
using UI;
using UnityEngine;

namespace _01.Member.KMJ._02.Scripts.Pool
{
    public class PopupText : MonoBehaviour, IPoolable
    {
        [SerializeField] private TextMeshPro popUpText;
        [field: SerializeField] public PoolingItemSO PoolingType { get; private set; }
        public GameObject GameObject => gameObject;

        private GondrLib.ObjectPool.Runtime.Pool _myPool;

        public void SetUpPool(GondrLib.ObjectPool.Runtime.Pool pool)
        {
            _myPool = pool;
        }

        public void ResetItem()
        {
            transform.localScale = Vector3.zero;
            popUpText.alpha = 1f;
        }

        private void LateUpdate()
        {
            Transform mainCamTrm = Camera.main.transform;
            Vector3 camDirtection = (transform.position - mainCamTrm.position).normalized;
            transform.forward = camDirtection;
        }

        public void ShowPopupText(string text, TextInfo textInfo, Vector3 position, float showDuration)
        {
            popUpText.SetText(text);
            popUpText.color = textInfo.textColor;
            popUpText.fontSize = textInfo.fontSize;

            transform.position = position;

            float scaleTime = 0.2f;
            float fdeTime = 1.2f;

            Sequence seq = DOTween.Sequence();
            seq.Append(transform.DOScale(2.5f, scaleTime));
            seq.Append(transform.DOScale(1.2f, scaleTime));
            seq.AppendInterval(showDuration);
            seq.Append(transform.DOScale(0.3f, fdeTime));
            seq.Join(popUpText.DOFade(0, fdeTime));
            seq.Join(transform.DOLocalMoveX(position.y + 2f, fdeTime));
            seq.AppendCallback(() => { _myPool.Push(this); });
        }
    }
}