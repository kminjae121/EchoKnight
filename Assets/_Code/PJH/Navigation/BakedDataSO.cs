using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Code.Navigation
{
    [CreateAssetMenu(fileName = "Baked Data", menuName = "SO/BakedData", order = 0)]
    public class BakedDataSO : ScriptableObject
    {
        public List<NodeData> points = new();
        private Dictionary<Vector3Int, NodeData> _pointDict;

        private void OnEnable()
        {
            //Initialize();
        }
        
        public void Initialize()
        {
            if (_pointDict == null || _pointDict.Count != points.Count)
                _pointDict = points.ToDictionary(node => node.cellPos);
        }
        
        public void ClearPoints()
        {
            points?.Clear();
            _pointDict?.Clear();
        }
        
        public void AddPoint(Vector3 worldPos, Vector3Int cellPos)
        {
            points.Add(new NodeData(worldPos, cellPos));
        }

        public bool HasNode(Vector3Int cellPos)
            => _pointDict != null && _pointDict.ContainsKey(cellPos);
        
        public bool GetNodeIfExist(Vector3Int cellPos, out NodeData nodeData)
        {
            if (HasNode(cellPos))
            {
                nodeData = _pointDict[cellPos];
                return true;
            }

            nodeData = null;
            return false;
        }
    }
}