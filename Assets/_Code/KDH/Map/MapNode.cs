using System;
using System.Collections.Generic;
using UnityEngine;

namespace Code.Map
{
    [Serializable]
    public class MapNode
    {
        public Vector2Int point;
        public Vector2 position;
        public MapNodeType nodeType;
        public List<Vector2Int> incoming = new List<Vector2Int>();
        public List<Vector2Int> outgoing = new List<Vector2Int>();

        public MapNode(Vector2Int point)
        {
            this.point = point;
        }
    }
}