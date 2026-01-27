using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code.UI
{
    public class BattleButton : MonoBehaviour
    {
        public void StartBattle()
        {
            SceneManager.LoadScene("KMJ");
        }
    }
}