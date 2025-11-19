using UnityEngine;

namespace UnitSystem
{
    public class UnitRenderer : MonoBehaviour, IUnitComponent
    {
        [SerializeField] private UnitSO thisUnitSO;
        [SerializeField] private MeshFilter thisMeshFilter;
        [SerializeField] private MeshRenderer thisMeshRenderer;
        [SerializeField] private Animator thisAnimator;
        public void Initialize(Unit owner)
        {
            
        }
        
        
        private void OnValidate()
        {
            if (thisUnitSO != null && thisMeshFilter != null && thisMeshRenderer != null
                && thisAnimator != null)
            {
                thisMeshFilter.mesh = thisUnitSO.characterMesh;
                thisMeshRenderer.material = thisUnitSO.characterMaterial;
                thisAnimator.runtimeAnimatorController = thisUnitSO.animationController;
            }
        }
    }
}