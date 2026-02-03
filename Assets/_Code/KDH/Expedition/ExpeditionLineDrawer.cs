using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(LineRenderer))]
public class ExpeditionLineDrawer : MonoBehaviour
{
    private LineRenderer _lineRenderer;

    [Header("Settings")]
    [SerializeField] private float waveHeight = 0.5f;
    [SerializeField] private float waveFrequency = 3f;
    [SerializeField] private int pointsCount = 50;

    private void Awake()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.positionCount = pointsCount;
        _lineRenderer.useWorldSpace = true;
    }

    public void DrawWavyLine(Vector3 startPos, Vector3 endPos)
    {
        _lineRenderer.positionCount = pointsCount;
        Vector3 direction = (endPos - startPos).normalized;
        float distance = Vector3.Distance(startPos, endPos);

        Vector3 perpendicular = new Vector3(-direction.y, direction.x, 0).normalized;

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