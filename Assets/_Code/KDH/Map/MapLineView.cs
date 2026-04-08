using UnityEngine;
using UnityEngine.UI;

namespace Code.Map
{
    [RequireComponent(typeof(Image))]
    public class MapLineView : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Image lineImage;
        [SerializeField] private RectTransform rectTransform;

        [Header("Settings")]
        [SerializeField] private Color normalColor = Color.gray;
        [SerializeField] private Color pathColor = Color.white;

        private void Awake()
        {
            if (lineImage == null) lineImage = GetComponent<Image>();
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        }

        public void DrawLine(Vector2 start, Vector2 end)
        {
            Vector2 direction = end - start;
            float distance = direction.magnitude;

            rectTransform.position = start + (direction / 2f);
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rectTransform.rotation = Quaternion.Euler(0, 0, angle);
            
            rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y);
            
            SetState(false);
        }

        public void SetState(bool isPath)
        {
            if (lineImage != null)
            {
                lineImage.color = isPath ? pathColor : normalColor;
            }
        }
    }
}