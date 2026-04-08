using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Code.Map
{
    public class MapNodeView : MonoBehaviour, IPointerClickHandler
    {
        [Header("Components")]
        [SerializeField] private Image iconImage;
        [SerializeField] private RectTransform rectTransform;

        [Header("State Colors")]
        [SerializeField] private Color normalColor = Color.gray;
        [SerializeField] private Color availableColor = Color.white;
        [SerializeField] private Color visitedColor = Color.black;

        public MapNode NodeData { get; private set; }
        private MapPlayerTracker _tracker;

        public void Setup(MapNode nodeData, MapNodeDataSO visualData, MapPlayerTracker tracker)
        {
            NodeData = nodeData;
            _tracker = tracker;

            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            rectTransform.anchoredPosition = nodeData.position * 100f; 

            if (visualData != null && iconImage != null)
            {
                iconImage.sprite = visualData.nodeIcon;
                rectTransform.localScale = Vector3.one * visualData.iconScale;
            }
            
            SetState(false, false);
        }

        public void SetState(bool isAvailable, bool isVisited)
        {
            if (iconImage != null)
            {
                if (isVisited) iconImage.color = visitedColor;
                else if (isAvailable) iconImage.color = availableColor;
                else iconImage.color = normalColor;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_tracker != null)
            {
                _tracker.SelectNode(NodeData);
            }
        }
    }
}