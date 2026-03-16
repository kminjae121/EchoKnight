using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Map
{
    [ExecuteAlways]
    [RequireComponent(typeof(DecalProjector))]
    public class MapTileVisual : MonoBehaviour
    {
        private DecalProjector decalProjector;
        private MapTile mapTile;

        [Header("Materials")]
        [SerializeField] private Material walkableMaterial;
        [SerializeField] private Material nonWalkableMaterial;
        [SerializeField] private Material enemyMaterial;
        [SerializeField] private Material obstacleMaterial;

        private void Awake()
        {
            decalProjector = GetComponent<DecalProjector>();
            mapTile = GetComponentInParent<MapTile>();
        }

        private void OnEnable()
        {
            if (mapTile != null)
                mapTile.OnTileStateChanged += HandleTileChanged;
        }

        private void OnDisable()
        {
            if (mapTile != null)
                mapTile.OnTileStateChanged -= HandleTileChanged;
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
            
            decalProjector.material = GetTileMaterial(mapTile);
        }

        public void SetDecalActive(bool isActive)
        {
            if (decalProjector == null)
                decalProjector = GetComponent<DecalProjector>();

            decalProjector.enabled = isActive;
        }

        private Material GetTileMaterial(MapTile tile)
        {
            if (tile.HasEnemy)
                return enemyMaterial;
            
            if (tile.HasObstacle)
                return obstacleMaterial;
            
            if (!tile.IsWalkable)
                return nonWalkableMaterial;
            
            return walkableMaterial;
        }
        
        private void HandleTileChanged(MapTile tile)
        {
            if (decalProjector == null)
                return;
            
            decalProjector.material = GetTileMaterial(tile);
        }
    }
}