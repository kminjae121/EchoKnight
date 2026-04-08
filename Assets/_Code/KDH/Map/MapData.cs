using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Map
{
    [Serializable]
    public class MapData
    {
        public List<MapNode> nodes = new List<MapNode>();
        public List<Vector2Int> path = new List<Vector2Int>();
        public string configName;
    }
}