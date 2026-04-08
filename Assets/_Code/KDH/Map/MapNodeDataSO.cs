using UnityEngine;

namespace Code.Map
{
    [CreateAssetMenu(fileName = "MapNodeData", menuName = "Map/MapNodeData")]
    public class MapNodeDataSO : ScriptableObject
    {
        [Header("Node Info")]
        public MapNodeType nodeType;
        public Sprite nodeIcon;
        public float iconScale = 1.0f;
    }
}