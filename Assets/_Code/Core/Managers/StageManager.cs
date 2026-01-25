using System.Collections.Generic;
using UnityEngine;

namespace _Code.Core.Managers
{
    public class StageManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> enemies;

        public int playerCount;
        public static StageManager Instance { get; private set; }

        [SerializeField] private GameObject gameClearUI;
        [SerializeField] private GameObject gameOverUI;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

        }

        public void RemoveEnemy(GameObject enemy)
        {
            enemies.Remove(enemy);
            if (enemies.Count == 0)
            {
                gameClearUI.SetActive(true);
            }
        }

        public void AddPlayerCnt()
        {
            playerCount += 1;
        }

        public void PlayerDie()
        {
            playerCount -= 1;

            if (playerCount <= 0)
            {
                gameOverUI.SetActive(true); 
            }
        }
        

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }
    }
}