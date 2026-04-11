using System.Collections.Generic;
using UnityEngine;

namespace Code.Map
{
    [CreateAssetMenu(fileName = "MapConfig", menuName = "Map/MapConfig")]
    public class MapConfigSO : ScriptableObject
    {
        public string configName;
        public int gridWidth = 7;
        public int numOfLayers = 15;
        
        [Header("Path Settings")]
        [Tooltip("1층에서 시작하는 노드의 개수")]
        public int numOfStartingNodes = 3; 
        [Tooltip("보스방으로 모이기 직전 층의 노드 개수")]
        public int numOfPreBossNodes = 3;  
        [Tooltip("경로를 꼬이게 만드는 추가 경로의 수 (0이면 단순해짐)")]
        public int extraPaths = 1;         

        [Header("Layer Settings")]
        public int preBossRestSiteLayer = 13;
        public int minEliteLayer = 5;
        
        [Header("Position Settings")]
        public float nodeSpacingX = 2f;
        public float nodeSpacingY = 2f;
        public float positionJitterX = 0.5f;
        public float positionJitterY = 0.5f;
        
        [Header("Node Blueprints")]
        public List<MapNodeBlueprint> defaultBlueprints;
    }
}