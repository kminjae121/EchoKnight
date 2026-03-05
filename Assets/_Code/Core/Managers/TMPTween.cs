using DG.Tweening;
using TMPro;

namespace _Code.Core.Managers
{
    public static class TMPTween
    {
        public static Tweener DoText(this TextMeshProUGUI thisTmp, string text, float duration)
        {
            int length = 0;
            
            return DOTween.To(
                () => length,
                x =>
                {
                    length = x;
                    thisTmp.text = text.Substring(0, length);
                },
                text.Length,
                duration
            ).SetEase(Ease.Linear);
        }
    }
}