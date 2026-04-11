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

        public void AddIncoming(Vector2Int p)
        {
            if (!incoming.Contains(p)) incoming.Add(p);
        }

        public void AddOutgoing(Vector2Int p)
        {
            if (!outgoing.Contains(p)) outgoing.Add(p);
        }

        public void RemoveIncoming(Vector2Int p)
        {
            incoming.Remove(p);
        }

        public void RemoveOutgoing(Vector2Int p)
        {
            outgoing.Remove(p);
        }

        public bool HasNoConnections()
        {
            return incoming.Count == 0 && outgoing.Count == 0;
        }
    }
}