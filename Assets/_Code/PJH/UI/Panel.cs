using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField] private string id = ""; public string ID => id;
    [SerializeField] private RectTransform container = null;

    public bool IsInitialized { get; private set; }
    public bool IsOpen { get; private set; }

    public Canvas Canvas { get; set; }

    public virtual void Awake()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
        if (IsInitialized) 
            return;
        
        IsInitialized = true;
        Close();
    }

    public virtual void Open()
    {
        if (!IsInitialized)
            Initialize();
        
        transform.SetAsLastSibling();
        container.gameObject.SetActive(true);
        IsOpen = true;
    }

    public virtual void Close()
    {
        if (!IsInitialized)
            Initialize();
        
        container.gameObject.SetActive(false);
        IsOpen = false;
    }
}