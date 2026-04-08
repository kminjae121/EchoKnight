using System.Collections.Generic;
using UnityEngine;

namespace Code.Map
{
    public class MapView : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private MapConfigSO mapConfig;
        [SerializeField] private List<MapNodeDataSO> nodeVisuals;
        [SerializeField] private MapPlayerTracker playerTracker;

        [Header("Prefabs")]
        [SerializeField] private MapNodeView nodePrefab;
        [SerializeField] private MapLineView linePrefab;

        [Header("Containers")]
        [SerializeField] private Transform nodesContainer;
        [SerializeField] private Transform linesContainer;

        private MapData _currentMap;
        private List<MapNodeView> _nodeViews = new List<MapNodeView>();
        private Dictionary<string, MapLineView> _lineViews = new Dictionary<string, MapLineView>();

        private void Start()
        {
            if (playerTracker != null)
            {
                playerTracker.OnNodeSelected += HandleNodeSelected;
            }
        }

        private void OnDestroy()
        {
            if (playerTracker != null)
            {
                playerTracker.OnNodeSelected -= HandleNodeSelected;
            }
        }

        public void GenerateAndDrawMap()
        {
            ClearMap();
            
            _currentMap = MapGenerator.GenerateMap(mapConfig);
            if (_currentMap == null) return;

            if (playerTracker != null)
            {
                playerTracker.Initialize(_currentMap);
            }

            DrawMap();
            RefreshViews();
        }

        private void DrawMap()
        {
            foreach (var node in _currentMap.nodes)
            {
                foreach (var outPoint in node.outgoing)
                {
                    MapNode targetNode = _currentMap.nodes.Find(n => n.point == outPoint);
                    if (targetNode != null)
                    {
                        DrawLine(node, targetNode);
                    }
                }
            }

            foreach (var node in _currentMap.nodes)
            {
                DrawNode(node);
            }
        }

        private void DrawNode(MapNode node)
        {
            MapNodeDataSO visualData = nodeVisuals.Find(v => v.nodeType == node.nodeType);
            MapNodeView view = Instantiate(nodePrefab, nodesContainer);
            view.Setup(node, visualData, playerTracker);
            _nodeViews.Add(view);
        }

        private void DrawLine(MapNode startNode, MapNode endNode)
        {
            MapLineView line = Instantiate(linePrefab, linesContainer);
            
            Vector2 startPos = startNode.position * 100f;
            Vector2 endPos = endNode.position * 100f;
            
            line.DrawLine(startPos, endPos);
            _lineViews.Add(GetLineKey(startNode.point, endNode.point), line);
        }

        private void HandleNodeSelected(MapNode node)
        {
            RefreshViews();
        }

        private void RefreshViews()
        {
            if (_currentMap == null) return;

            Vector2Int currentPoint = new Vector2Int(-1, -1);
            if (_currentMap.path.Count > 0)
            {
                currentPoint = _currentMap.path[_currentMap.path.Count - 1];
            }

            MapNode currentNode = _currentMap.nodes.Find(n => n.point == currentPoint);

            foreach (var view in _nodeViews)
            {
                bool isVisited = _currentMap.path.Contains(view.NodeData.point);
                bool isAvailable = false;

                if (currentNode == null)
                {
                    isAvailable = view.NodeData.point.y == 0;
                }
                else
                {
                    isAvailable = currentNode.outgoing.Contains(view.NodeData.point);
                }

                view.SetState(isAvailable, isVisited);
            }

            for (int i = 0; i < _currentMap.path.Count - 1; i++)
            {
                string key = GetLineKey(_currentMap.path[i], _currentMap.path[i + 1]);
                if (_lineViews.ContainsKey(key))
                {
                    _lineViews[key].SetState(true);
                }
            }
        }

        private string GetLineKey(Vector2Int start, Vector2Int end)
        {
            return $"{start.x},{start.y}-{end.x},{end.y}";
        }

        private void ClearMap()
        {
            foreach (Transform child in nodesContainer) Destroy(child.gameObject);
            foreach (Transform child in linesContainer) Destroy(child.gameObject);
            
            _nodeViews.Clear();
            _lineViews.Clear();
        }
    }
}