using System;
using UnityEngine;

namespace Code.UI
{
    public class MarkUI : MonoBehaviour
    {
        [SerializeField] private GameObject markUI;

        private GameObject cam;
        
        
        public void SetObject(GameObject cam)
        {
            this.cam = cam;
        }

        private void Update()
        {
            if(markUI != null)
               markUI.transform.LookAt(cam.transform);
        }
    }
}