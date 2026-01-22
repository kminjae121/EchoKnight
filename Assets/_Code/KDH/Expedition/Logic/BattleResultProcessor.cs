using System.Collections;
using _00.Core._02.Scripts._01.Manager;
using Code.Expedition;
using Code.Core.Events.Bus;
using UnityEngine;

namespace Code.Expedition.Logic
{
    public class BattleResultProcessor : MonoBehaviour
    {
        [SerializeField] private float victoryReturnDelay = 3.0f;
        [SerializeField] private float defeatReturnDelay = 3.0f;
        [SerializeField] private string lobbySceneName = "LobbyScene";

        private void OnEnable()
        {
            Bus<StageClearEvent>.Subscribe(OnStageClear);
        }

        private void OnDisable()
        {
            Bus<StageClearEvent>.Unsubscribe(OnStageClear);
        }

        private void OnStageClear(StageClearEvent evt)
        {
            if (evt.isClear)
            {
                Debug.Log("전투 승리! 맵으로 복귀합니다.");
                StartCoroutine(ProcessVictoryRoutine());
            }
            else
            {
                Debug.Log("전투 패배. 원정이 종료됩니다.");
                StartCoroutine(ProcessDefeatRoutine());
            }
        }

        private IEnumerator ProcessVictoryRoutine()
        {
            yield return new WaitForSeconds(victoryReturnDelay);

            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.CompleteCurrentNode();
                
                string mapScene = ExpeditionManager.Instance.MapSceneName;
                SceneChangeManager.Instance.ChangeSelectScene(mapScene);
            }
        }

        private IEnumerator ProcessDefeatRoutine()
        {
            yield return new WaitForSeconds(defeatReturnDelay);

            if (ExpeditionManager.Instance != null)
            {
                ExpeditionManager.Instance.FailExpedition();
            }

            if (SceneChangeManager.Instance != null)
            {
                SceneChangeManager.Instance.ChangeSelectScene(lobbySceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(lobbySceneName);
            }
        }
    }
}