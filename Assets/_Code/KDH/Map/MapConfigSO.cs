using System.Collections.Generic;
using UnityEngine;

namespace Code.Map
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Map/MapConfig")]
    public class MapConfigSO : ScriptableObject
    {
        [Header("Grid Settings")]
        public int gridWidth = 7;
        public int numOfLayers = 15;
        public int startingNodesCount = 3;
        public int preBossRestSiteLayer = 14;

        [Header("Jitter Settings")]
        public float nodeSpacingX = 1.5f;
        public float nodeSpacingY = 2.0f;
        public float positionJitterX = 0.3f;
        public float positionJitterY = 0.3f;

        [Header("Generation Rules")]
        public int minEliteLayer = 5;
        public List<MapNodeBlueprint> defaultBlueprints = new List<MapNodeBlueprint>();
    }
}