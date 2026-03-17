using Code.Core.Events.Bus;
using Code.UnitSystem.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace EnemySystem
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Image healthUI;
        [SerializeField] private Image healthBackGroundUI;

        [SerializeField] private UnitHealth healthCompo;

        [SerializeField] private GameObject targetCamera;
        

        private void Awake()
        {
            Bus<CamMovingEvent>.OnEvent += SetCam;
        }

        private void OnDestroy()
        {
            Bus<CamMovingEvent>.OnEvent -= SetCam;
        }

        private void SetCam(CamMovingEvent evt)
        {
            targetCamera = evt.target;
        }

        private void Update()
        {
            healthUI.fillAmount = healthCompo.MaxHealth > 0 
                ? Mathf.Clamp01(healthCompo.CurrentHealth / healthCompo.MaxHealth) 
                : 0f;
        }
        
        void LateUpdate()
        {
            healthUI.transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                targetCamera.transform.rotation * Vector3.up);
            healthBackGroundUI.transform.LookAt(transform.position + targetCamera.transform.rotation * Vector3.forward,
                targetCamera.transform.rotation * Vector3.up);
        }
    }
}