using Code.Core.Interfaces;
using UnityEngine;

namespace Code.Map
{
    public class MapTile : MonoBehaviour, IMapTile
    {
        [SerializeField] private Vector2Int gridPos;
        [SerializeField] private TileState tileState;

        public Vector2Int GridPos => gridPos;
        public Vector3 WorldPos => transform.position;

        private MapTileVisual _visual;

        private void Awake()
        {
            _visual = GetComponentInChildren<MapTileVisual>();
            RefreshVisual();
        }

        private void OnValidate()
        {
            RefreshVisual();
        }

        public void Initialize(Vector2Int pos)
        {
            gridPos = pos;

            if (tileState == TileState.None)
                tileState = TileState.Walkable;

            RefreshVisual();
        }

        public bool HasState(TileState state)
            => (tileState & state) == state;

        public void SetState(TileState state, bool value)
        {
            if (value)
                tileState |= state;
            else
                tileState &= ~state;

            RefreshVisual();
        }

        public void SetDecalActive(bool isActive)
        {
            _visual?.SetDecalActive(isActive);
        }

        private void RefreshVisual()
        {
            _visual?.HandleTileChanged(this);
        }
    }
}