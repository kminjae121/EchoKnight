using UnityEngine;
using System;

namespace Code.Map
{
    public class MapPlayerTracker : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private MapData currentMapData;
        [SerializeField] private Vector2Int currentNodePoint = new Vector2Int(-1, -1);

        public event Action<MapNode> OnNodeSelected;

        public void Initialize(MapData mapData)
        {
            currentMapData = mapData;
            currentNodePoint = new Vector2Int(-1, -1);
        }

        public void SelectNode(MapNode targetNode)
        {
            if (CanMoveTo(targetNode))
            {
                currentNodePoint = targetNode.point;
                currentMapData.path.Add(targetNode.point);
                OnNodeSelected?.Invoke(targetNode);
            }
            else
            {
                Debug.LogWarning("이동할 수 없는 노드입니다.");
            }
        }

        private bool CanMoveTo(MapNode targetNode)
        {
            if (currentNodePoint.x == -1 && currentNodePoint.y == -1)
            {
                return targetNode.point.y == 0;
            }

            MapNode currentNode = GetNode(currentNodePoint);
            if (currentNode != null)
            {
                return currentNode.outgoing.Contains(targetNode.point);
            }

            return false;
        }

        public MapNode GetNode(Vector2Int point)
        {
            return currentMapData?.nodes.Find(n => n.point == point);
        }
    }
}