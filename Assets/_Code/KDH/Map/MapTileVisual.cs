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

        private void Update()
        {
            UpdateVisual();
        }

        public void Initialize(
            Material walkable, 
            Material nonWalkable, 
            Material enemy, 
            Material obstacle, 
            float projectionDepth,
            uint renderingLayerMask)
        {
            walkableMaterial = walkable;
            nonWalkableMaterial = nonWalkable;
            enemyMaterial = enemy;
            obstacleMaterial = obstacle;
            
            decalProjector = GetComponent<DecalProjector>();
            mapTile = GetComponentInParent<MapTile>();

            Vector3 size = decalProjector.size;
            size.z = projectionDepth;
            decalProjector.size = size;
            decalProjector.pivot = new Vector3(0f, 0f, projectionDepth * 0.5f);

            decalProjector.renderingLayerMask = renderingLayerMask;
            
            UpdateVisual();
        }

        private void UpdateVisual()
        {
            if (decalProjector == null || mapTile == null) return;

            decalProjector.material = GetTileMaterial();
        }

        private Material GetTileMaterial()
        {
            if (mapTile.HasEnemy)
                return enemyMaterial;
            
            if (mapTile.HasObstacle)
                return obstacleMaterial;
            
            if (!mapTile.IsWalkable)
                return nonWalkableMaterial;
            
            return walkableMaterial;
        }
    }
}