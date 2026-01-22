using Code.Expedition;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Expedition.UI
{
    public class ExpeditionMapView : MonoBehaviour
    {
        [SerializeField] private ExpeditionNodeView nodePrefab;
        [SerializeField] private Transform currentNodeContainer;
        [SerializeField] private Transform nextNodesContainer;
        
        // Dependencies
        [SerializeField] private ExpeditionManager expeditionManager;

        private void Start()
        {
            if (expeditionManager == null)
                expeditionManager = ExpeditionManager.Instance;

            Bus<ExpeditionNodeArriveEvent>.Subscribe(OnNodeArrive);
            
            RefreshMap();
        }

        private void OnDestroy()
        {
            Bus<ExpeditionNodeArriveEvent>.Unsubscribe(OnNodeArrive);
        }

        private void OnNodeArrive(ExpeditionNodeArriveEvent evt)
        {
            RefreshMap();
        }

        private void RefreshMap()
        {
            ClearContainer(currentNodeContainer);
            ClearContainer(nextNodesContainer);

            RuntimeExpeditionNode currentNode = expeditionManager.GetCurrentNode();

            if (currentNode == null) return;

            CreateNodeView(currentNode, currentNodeContainer, isInteractable: false);

            foreach (var nextNode in currentNode.NextNodes)
            {
                CreateNodeView(nextNode, nextNodesContainer, isInteractable: true);
            }
        }

        private void CreateNodeView(RuntimeExpeditionNode node, Transform parent, bool isInteractable)
        {
            if (nodePrefab == null) return;

            ExpeditionNodeView view = Instantiate(nodePrefab, parent);
            
            if (isInteractable)
                view.Initialize(node, HandleNodeClick);
            else
                view.Initialize(node, null);
        }

        private void HandleNodeClick(RuntimeExpeditionNode node)
        {
            expeditionManager.SelectNode(node);
        }

        private void ClearContainer(Transform container)
        {
            foreach (Transform child in container)
                Destroy(child.gameObject);
        }
    }
}