using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Map
{
    [ExecuteAlways]
    [RequireComponent(typeof(DecalProjector))]
    public class MapTileVisual : MonoBehaviour
    {
        private DecalProjector decalProjector;

        [Header("Materials")]
        [SerializeField] private Material walkableMaterial;
        [SerializeField] private Material nonWalkableMaterial;
        [SerializeField] private Material enemyMaterial;
        [SerializeField] private Material obstacleMaterial;

        private void Awake()
        {
            decalProjector = GetComponent<DecalProjector>();
            SetDecalActive(false);
        }

        public void Initialize(Material walkable, Material nonWalkable, Material enemy, Material obstacle, float projectionDepth, uint renderingLayerMask)
        {
            walkableMaterial = walkable;
            nonWalkableMaterial = nonWalkable;
            enemyMaterial = enemy;
            obstacleMaterial = obstacle;
            
            //decalProjector = GetComponent<DecalProjector>();
            //mapTile = GetComponentInParent<MapTile>();

            Vector3 size = decalProjector.size;
            size.z = projectionDepth;
            decalProjector.size = size;
            decalProjector.pivot = new Vector3(0f, 0f, projectionDepth * 0.5f);

            decalProjector.renderingLayerMask = renderingLayerMask;
            decalProjector.enabled = false;
        }

        public void SetDecalActive(bool isActive)
        {
            if (decalProjector == null)
                return;

            decalProjector.enabled = isActive;
        }

        private Material GetTileMaterial(MapTile tile)
        {
            if (tile.HasState(TileState.Enemy))
                return enemyMaterial;
            
            if (tile.HasState(TileState.Obstacle))
                return obstacleMaterial;
            
            if (!tile.HasState(TileState.Walkable))
                return nonWalkableMaterial;
            
            return walkableMaterial;
        }

        public void HandleTileChanged(MapTile tile)
        {
            if (decalProjector == null)
                return;
            
            decalProjector.material = GetTileMaterial(tile);
        }
    }
}
