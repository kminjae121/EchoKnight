using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using DG.Tweening;
using Code.Core.Events.Bus;
using Code.Expedition.Data;
using _00.Core._02.Scripts._01.Manager;

namespace Code.Expedition.Managers
{
    [System.Serializable]
    public struct EventUIMapping
    {
        public EventNodeSO eventNodeData;
        public GameObject uiPanel;
    }

    [System.Serializable]
    public struct NodeTypeSceneMapping
    {
        public NodeType nodeType;
        public string sceneName;
    }

    public class ExpeditionManager : MonoSingleton<ExpeditionManager>
    {
        [Header("2D Map Generation Settings")]
        public MapConfig config;
        public MapView view;

        [Header("Event & Scene Mapping")]
        [SerializeField] private List<EventUIMapping> eventUIMappings;
        [SerializeField] private List<NodeTypeSceneMapping> sceneMappings;

        [Header("Tracker Settings")]
        public bool lockAfterSelecting = false;
        public float enterNodeDelay = 1f;

        public Map.Map CurrentMap { get; private set; }
        public bool Locked { get; set; }
        
        private Canvas _canvas;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            Bus<StageClearEvent>.Subscribe(OnStageCleared);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            Bus<StageClearEvent>.Unsubscribe(OnStageCleared);
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            InitializeMap();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (view != null && CurrentMap != null)
            {
                view.ShowMap(CurrentMap);
                SetAttainableNodes();
                SetLineColors();
            }
        }

        private void InitializeMap()
        {
            if (PlayerPrefs.HasKey("ExpeditionMap"))
            {
                string mapJson = PlayerPrefs.GetString("ExpeditionMap");
                Map.Map map = JsonConvert.DeserializeObject<Map.Map>(mapJson);
                
                if (map.path.Any(p => p.Equals(map.GetBossNode().point)))
                {
                    GenerateNewMap();
                }
                else
                {
                    CurrentMap = map;
                    if (view != null) view.ShowMap(map);
                }
            }
            else
            {
                GenerateNewMap();
            }

            Locked = false;
        }

        public void GenerateNewMap()
        {
            Map.Map map = MapGenerator.GetMap(config);
            CurrentMap = map;
            SaveMap();
            if (view != null) view.ShowMap(map);
        }

        public void SaveMap()
        {
            if (CurrentMap == null) return;

            string json = JsonConvert.SerializeObject(CurrentMap, Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            PlayerPrefs.SetString("ExpeditionMap", json);
            PlayerPrefs.Save();
        }

        public void SelectNode(MapNode mapNode)
        {
            if (Locked) return;

            if (CurrentMap.path.Count == 0)
            {
                if (mapNode.Node.point.y == 0)
                    SendPlayerToNode(mapNode);
                else
                    Debug.LogWarning("첫 번째 층의 노드만 선택할 수 있습니다.");
            }
            else
            {
                Vector2Int currentPoint = CurrentMap.path.Last();
                Node currentNode = CurrentMap.GetNode(currentPoint);

                if (currentNode != null && currentNode.outgoing.Any(point => point.Equals(mapNode.Node.point)))
                    SendPlayerToNode(mapNode);
                else
                    Debug.LogWarning("현재 위치에서 갈 수 없는 노드입니다.");
            }
        }

        private void SendPlayerToNode(MapNode mapNode)
        {
            Locked = lockAfterSelecting;
            CurrentMap.path.Add(mapNode.Node.point);
            SaveMap();
            
            SetAttainableNodes();
            SetLineColors();
            mapNode.ShowSwirlAnimation();

            DOTween.Sequence().AppendInterval(enterNodeDelay).OnComplete(() => EnterStage(mapNode));
        }

        private void EnterStage(MapNode mapNode)
        {
            NodeBlueprint blueprint = mapNode.Blueprint;
            Debug.Log($"노드 진입: {blueprint.name} / 타입: {blueprint.nodeType}");

            if (blueprint.nodeType == NodeType.Mystery || blueprint.nodeType == NodeType.Store || blueprint.nodeType == NodeType.RestSite)
            {
                HandleEventNode(blueprint);
            }
            else
            {
                HandleBattleNode(blueprint);
            }
        }

        private void HandleEventNode(NodeBlueprint blueprint)
        {
            GameObject targetUIPrefab = null;

            if (_canvas == null)
            {
                Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                foreach (var canvas in canvases)
                {
                    if (canvas.gameObject.name == "UI")
                    {
                        _canvas = canvas;
                        break;
                    }
                }
            }

            if (eventUIMappings.Count > 0)
            {
                var mapping = eventUIMappings.FirstOrDefault(m => m.eventNodeData != null && m.eventNodeData.nodeName == blueprint.name);
                targetUIPrefab = mapping.uiPanel != null ? mapping.uiPanel : eventUIMappings[0].uiPanel;
            }

            if (targetUIPrefab != null && _canvas != null)
            {
                GameObject uiInstance = Instantiate(targetUIPrefab, _canvas.transform);
                uiInstance.transform.localPosition = new Vector3(13, -82, -12f);
                uiInstance.SetActive(true);
            }
            else
            {
                Debug.LogWarning("이벤트 UI 프리팹 또는 Canvas 'UI'를 찾을 수 없습니다.");
            }

            Locked = false;
        }

        private void HandleBattleNode(NodeBlueprint blueprint)
        {
            string targetSceneName = string.Empty;

            var mapping = sceneMappings.FirstOrDefault(m => m.nodeType == blueprint.nodeType);
            if (!string.IsNullOrEmpty(mapping.sceneName))
            {
                targetSceneName = mapping.sceneName;
            }
            else
            {
                targetSceneName = "BattleScene";
                Debug.LogWarning($"[{blueprint.nodeType}]에 매핑된 씬이 없어 기본 씬으로 이동합니다.");
            }

            if (SceneChangeManager.Instance != null)
                SceneChangeManager.Instance.ChangeSelectScene(targetSceneName);
            else
                SceneManager.LoadScene(targetSceneName);
        }

        private void OnStageCleared(StageClearEvent evt)
        {
            if (evt.isClear)
            {
                Debug.Log("스테이지 클리어 확인 - 맵 선택 가능 상태로 변경됩니다.");
                Locked = false;
                SaveMap();
            }
        }

        public void SetAttainableNodes() { if (view != null) view.SetAttainableNodes(); }
        public void SetLineColors() { if (view != null) view.SetLineColors(); }
    }
}