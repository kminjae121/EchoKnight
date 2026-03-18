using Code.Map;
using UnityEngine;

namespace Code.Core.Interfaces
{
    public interface IMapTile
    {
        Vector2Int GridPos { get; }
        Vector3 WorldPos { get; }
        
        bool HasState(TileState state);

        void SetState(TileState state, bool value);

        void SetDecalActive(bool isActive);
        void SetOverlay(TileOverlayType overlayType);
        void ClearOverlay();
    }
}

