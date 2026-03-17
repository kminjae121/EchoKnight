using Code.Core.Events.Bus;
using Code.Items;
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

        private EquipmentItemSO _equipmentItem;
        private bool _isEquipped;
        private bool _isSelected;
        private GondrLib.ObjectPool.Runtime.Pool _pool;

        public PoolingItemSO PoolingType => poolingType;
        public GameObject GameObject => gameObject;

        private void Awake()
        {
            Bus<ArtifactPopupEvent>.Subscribe(HandlePopupEvent);
        }

        private void OnDestroy()
        {
            Bus<ArtifactPopupEvent>.Unsubscribe(HandlePopupEvent);
        }

        public void SetUpPool(GondrLib.ObjectPool.Runtime.Pool pool) => _pool = pool;

        public void ResetItem()
        {
            _equipmentItem = null;
            _isEquipped = false;
            _isSelected = false;
            if (hoverImage != null) hoverImage.SetActive(false);
        }

        public void ReturnToPool()
        {
            if (_pool != null) _pool.Push(this);
            else Destroy(gameObject);
        }

        public void SetArtifact(EquipmentItemSO equipmentItem, bool isEquipped)
        {
            _equipmentItem = equipmentItem;
            iconImage.sprite = equipmentItem.itemIcon;
            _isEquipped = isEquipped;

            iconImage.color = Color.white; 

            if (hoverImage != null) hoverImage.SetActive(false);
        }

        private void HandlePopupEvent(ArtifactPopupEvent evt)
        {
            if (_equipmentItem != null && evt.EquipmentItem == _equipmentItem)
            {
                _isSelected = true;
                if (hoverImage != null) hoverImage.SetActive(true);
            }
            else
            {
                _isSelected = false;
                if (hoverImage != null) hoverImage.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (_equipmentItem != null && hoverImage != null) 
            {
                hoverImage.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (_equipmentItem != null && hoverImage != null && !_isSelected) 
            {
                hoverImage.SetActive(false);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_equipmentItem == null) return;

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Bus<ArtifactPopupEvent>.Raise(new ArtifactPopupEvent(_equipmentItem, _isEquipped, eventData.position));
            }
        }
    }
}