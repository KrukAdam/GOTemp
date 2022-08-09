using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(DefaultButtonSerializeFields))]
public class DefaultButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    [field: SerializeField]
    public DefaultButtonSerializeFields SerializeFields
    {
        get;
        private set;
    }

    protected virtual void Awake()
    {
        if (SerializeFields == null)
        {
            SerializeFields = GetComponent<DefaultButtonSerializeFields>();
        }

        SerializeFields.Button.onClick.AddListener(OnClick);
    }

    public virtual void Init(LevelController levelController) { }

    public virtual void OnClick() { }

    public virtual void OnPointerEnter(PointerEventData eventData) { }

    public virtual void OnPointerExit(PointerEventData eventData) { }

    public virtual void OnPointerUp(PointerEventData eventData) { }

    public virtual void OnPointerDown(PointerEventData eventData) { }

    public virtual void SetInteractable(bool interactable)
    {
        SerializeFields.Button.interactable = interactable;
    }

    public void SetOnClick(UnityAction func)
    {
        SerializeFields.Button.onClick.RemoveAllListeners();
        SerializeFields.Button.onClick.AddListener(func);
    }

    public void AddOnClick(UnityAction func)
    {
        SerializeFields.Button.onClick.AddListener(func);
    }
}
