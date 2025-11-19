using UnityEngine;

namespace UnitSystem
{
    public class UnitRenderer : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private UnitSO thisUnitSO;
        [SerializeField] private Animator thisAnimator;
        public void Initialize(Unit owner)
        {
            
        }
        
        
        private void OnValidate()
        {
            if (thisUnitSO != null && thisAnimator != null)
            {
                thisAnimator.runtimeAnimatorController = thisUnitSO.animationController;
            }
        }
    }
}