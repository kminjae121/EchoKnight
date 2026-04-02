using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class GameStartUI : MonoBehaviour
    {
        [SerializeField] private Image img;

        public void StartGame()
        {
            img.DOFade(0, 2f)
                .SetEase(Ease.InQuint)
                .OnComplete(() =>
                {
                    img.gameObject.SetActive(false);
                });
        }
        
    }
}