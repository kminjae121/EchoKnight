using System.Collections.Generic;
using Code.Expedition;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Expedition.UI
{
    public class ExpeditionMapView : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private ExpeditionNodeView nodePrefab;
        [SerializeField] private ExpeditionLineDrawer lineDrawerPrefab;

        [Header("Containers")]
        [SerializeField] private Transform currentNodeContainer;
        [SerializeField] private Transform nextNodesContainer;
        [SerializeField] private Transform linesContainer;
        
        [Header("Dependencies")]
        [SerializeField] private ExpeditionManager expeditionManager;

        // 노드와 뷰를 매핑하여 카메라 이동 타겟을 찾기 위함
        private Dictionary<RuntimeExpeditionNode, ExpeditionNodeView> _nodeViewMap = new Dictionary<RuntimeExpeditionNode, ExpeditionNodeView>();

        private void Start()
        {
            if (expeditionManager == null)
                expeditionManager = ExpeditionManager.Instance;

            Bus<ExpeditionNodeArriveEvent>.Subscribe(OnNodeArrive);
            Bus<ExpeditionNodeSelectEvent>.Subscribe(OnNodeSelected);
            
            RefreshMap();
        }

        private void OnDestroy()
        {
            Bus<ExpeditionNodeArriveEvent>.Unsubscribe(OnNodeArrive);
            Bus<ExpeditionNodeSelectEvent>.Unsubscribe(OnNodeSelected);
        }

        private void OnNodeArrive(ExpeditionNodeArriveEvent evt)
        {
            RefreshMap();
        }

        private void OnNodeSelected(ExpeditionNodeSelectEvent evt)
        {
            if (_nodeViewMap.TryGetValue(evt.SelectedNode, out ExpeditionNodeView view))
            {
                Bus<CamMovingEvent>.Raise(new CamMovingEvent(view.gameObject));
            }
        }

        private void RefreshMap()
        {
            ClearMap();

            RuntimeExpeditionNode currentNode = expeditionManager.GetCurrentNode();

            if (currentNode == null) return;

            ExpeditionNodeView currentView = CreateNodeView(currentNode, currentNodeContainer, isInteractable: false);
            if (currentView != null) _nodeViewMap.Add(currentNode, currentView);
            
            foreach (var nextNode in currentNode.NextNodes)
            {
                ExpeditionNodeView nextView = CreateNodeView(nextNode, nextNodesContainer, isInteractable: true);
                if (nextView != null)
                {
                    _nodeViewMap.Add(nextNode, nextView);
                    DrawConnectionLine(currentView.transform.position, nextView.transform.position);
                }
            }
        }

        private ExpeditionNodeView CreateNodeView(RuntimeExpeditionNode node, Transform parent, bool isInteractable)
        {
            if (nodePrefab == null) return null;

            ExpeditionNodeView view = Instantiate(nodePrefab, parent);
            
            if (isInteractable)
                view.Initialize(node, HandleNodeClick);
            else
                view.Initialize(node, null);
                
            return view;
        }

        private void DrawConnectionLine(Vector3 startPos, Vector3 endPos)
        {
            if (lineDrawerPrefab == null) return;

            ExpeditionLineDrawer drawer = Instantiate(lineDrawerPrefab, linesContainer);
            drawer.DrawWavyLine(startPos, endPos);
        }

        private void HandleNodeClick(RuntimeExpeditionNode node)
        {
            expeditionManager.SelectNode(node);
        }

        private void ClearMap()
        {
            _nodeViewMap.Clear();
            ClearContainer(currentNodeContainer);
            ClearContainer(nextNodesContainer);
            ClearContainer(linesContainer);
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);
        }
    }
}