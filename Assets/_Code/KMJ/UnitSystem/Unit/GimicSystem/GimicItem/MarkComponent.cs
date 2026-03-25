using Code.Core.Events.Bus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UnitSystem.GimicSystem
{
    public class MarkComponent : MonoBehaviour
    {
        [SerializeField] private Image markUI;
        [SerializeField] private TextMeshProUGUI markText;
        [SerializeField] private Image killUI;
         public bool isMarking { get; private set; } 

        private void Start()
        {
            Bus<SetMarkEvent>.Subscribe(SetMark);
        }
        private void OnDestroy()
        {
            Bus<SetMarkEvent>.Unsubscribe(SetMark);
        }

        private void SetMark(SetMarkEvent evt)
        {
            isMarking = true;
            
            markText.text = evt.cnt.ToString();

            if (evt.cnt == 4)
                killUI.gameObject.SetActive(true);
            
            
            markUI.gameObject.SetActive(true);

            if (evt.cnt == 5)
            {
                killUI.gameObject.SetActive(false);
                markUI.gameObject.SetActive(false);
                markText.gameObject.SetActive(false);
            }
        }
    }
}