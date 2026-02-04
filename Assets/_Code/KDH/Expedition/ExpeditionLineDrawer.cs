using UnityEngine;

namespace Code.Expedition
{
    [RequireComponent(typeof(LineRenderer))]
    public class ExpeditionLineDrawer : MonoBehaviour
    {
        [Header("Wave Settings")]
        [SerializeField] private float waveHeight = 0.5f;
        [SerializeField] private float waveFrequency = 3f;
        [SerializeField] private int pointsCount = 50;

        [Header("Dotted Line Settings")]
        [SerializeField] private float textureScale = 1.0f;

        private LineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            InitializeLineRenderer();
        }

        private void InitializeLineRenderer()
        {
            _lineRenderer.positionCount = pointsCount;
            _lineRenderer.useWorldSpace = true;
            _lineRenderer.textureMode = LineTextureMode.Tile;
        }

        public void DrawWavyLine(Vector3 startPos, Vector3 endPos)
        {
            _lineRenderer.positionCount = pointsCount;
            
            float distance = Vector3.Distance(startPos, endPos);
            Vector3 direction = (endPos - startPos).normalized;
            Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0).normalized;

            if (_lineRenderer.material != null)
                _lineRenderer.material.mainTextureScale = new Vector2(distance * textureScale, 1f);

            for (int i = 0; i < pointsCount; i++)
            {
                float t = (float)i / (pointsCount - 1);
                Vector3 point = Vector3.Lerp(startPos, endPos, t);
                
                float wave = Mathf.Sin(t * Mathf.PI * waveFrequency * 2) * waveHeight;
                float dampener = Mathf.Sin(t * Mathf.PI); 

                point += perpendicular * wave * dampener;
                point.z = startPos.z; 

                _lineRenderer.SetPosition(i, point);
            }
        }
    }
}