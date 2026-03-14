using Code.Core.Events.Bus;
using Code.UnitSystem.ArtifactSystem;
using GondrLib.ObjectPool.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code.UI
{
    public class ArtifactButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPoolable
    {
        [Header("Pooling Settings")]
        [SerializeField] private PoolingItemSO poolingType;

        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject hoverImage; 

        private ArtifactSO _artifact;
        private bool _isEquipped;
        private GondrLib.ObjectPool.Runtime.Pool _pool;

        public PoolingItemSO PoolingType => poolingType;
        public GameObject GameObject => gameObject;

        public void SetUpPool(GondrLib.ObjectPool.Runtime.Pool pool) => _pool = pool;

        public void ResetItem()
        {
            _artifact = null;
            _isEquipped = false;
        }

        public void ReturnToPool()
        {
            if (_pool != null) _pool.Push(this);
            else Destroy(gameObject);
        }

        public void SetArtifact(ArtifactSO artifact, bool isEquipped)
        {
            _artifact = artifact;
            iconImage.sprite = artifact.artifactIcon;
            _isEquipped = isEquipped;

            iconImage.color = Color.white; 

            if (hoverImage != null) hoverImage.SetActive(false);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_artifact != null && hoverImage != null) 
            {
                hoverImage.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_artifact != null && hoverImage != null) 
            {
                hoverImage.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_artifact == null) return;

            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Bus<ArtifactPopupEvent>.Raise(new ArtifactPopupEvent(_artifact, _isEquipped, eventData.position));
            }
        }
    }
}