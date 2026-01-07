using System.Collections.Generic;
using Code.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _00.Core._02.Scripts._01.Manager
{
    public class SceneChangeManager : MonoSingleton<SceneChangeManager>
    {
        [SerializeField] private List<string> scenesName;

        private int _currentSceneIdx;


        public void ChangeSceneIdx(int idx)
        {
            SceneManager.LoadScene(idx);

            _currentSceneIdx = idx;
        }

        public void ChangeNextScene()
        {
            _currentSceneIdx += 1;
            SceneManager.LoadScene(scenesName[_currentSceneIdx]);
        }

        public void ChangeSelectScene(string sceneName)
        {
            if (!scenesName.Contains(sceneName))
                return;
            
            int sceneIdx = scenesName.IndexOf(sceneName);
            
            SceneManager.LoadScene(sceneName);

            _currentSceneIdx = sceneIdx;
        }
    }   
}