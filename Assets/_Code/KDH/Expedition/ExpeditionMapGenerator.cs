using System.Collections.Generic;
using Code.Expedition.Data;
using UnityEngine;

namespace Code.Expedition
{
    public class ExpeditionMapGenerator : MonoBehaviour
    {
        [SerializeField] private List<ExpeditionNodeSO> availableNodeTypes;
        [SerializeField] private int mapDepth = 10; 

        public RuntimeExpeditionNode GenerateMap()
        {
            if (availableNodeTypes == null || availableNodeTypes.Count == 0)
            {
                Debug.LogError("맵 생성 실패: 사용 가능한 노드 데이터(SO)가 없습니다.");
                return null;
            }

            RuntimeExpeditionNode startNode = CreateNode(ExpeditionNodeType.Start, 0);
            
            List<RuntimeExpeditionNode> previousLayer = new List<RuntimeExpeditionNode> { startNode };

            for (int i = 1; i < mapDepth; i++)
            {
                List<RuntimeExpeditionNode> currentLayer = new List<RuntimeExpeditionNode>();
                int layerNodeCount = Random.Range(2, 4); 

                for (int j = 0; j < layerNodeCount; j++)
                {
                    ExpeditionNodeType randomType = GetRandomNodeType();
                    RuntimeExpeditionNode newNode = CreateNode(randomType, i);
                    currentLayer.Add(newNode);

                    foreach (var prevNode in previousLayer)
                    {
                        prevNode.AddConnection(newNode);
                    }
                }
                previousLayer = currentLayer;
            }

            return startNode;
        }

        private RuntimeExpeditionNode CreateNode(ExpeditionNodeType type, int layerIndex)
        {
            ExpeditionNodeSO data = availableNodeTypes.Find(x => x.nodeType == type);
            if (data == null)
                data = availableNodeTypes[0]; 
            return new RuntimeExpeditionNode(data, layerIndex);
        }

        private ExpeditionNodeType GetRandomNodeType()
        {
            int rnd = Random.Range(1, 4); 
            return (ExpeditionNodeType)rnd;
        }
    }
}