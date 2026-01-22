using _00.Core._02.Scripts._01.Manager;
using Code.Expedition;
using Code.Expedition.Data;
using UnityEngine;

namespace Code.Expedition.Logic
{
    public class BattleNodeLogic : INodeLogic
    {
        public void Execute(RuntimeExpeditionNode node)
        {
            if (node.Data is BattleNodeSO battleData)
            {
                Debug.Log($"전투 노드 진입: {battleData.nodeName}");
                BattleContext.Instance.SetContext(battleData);
                SceneChangeManager.Instance.ChangeSelectScene(battleData.battleSceneName);
            }
            else
            {
                Debug.LogError("전투 노드 데이터 캐스팅 실패");
            }
        }
    }
}