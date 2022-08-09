using System;
using UnityEngine;


[RequireComponent(typeof(DefaultPanelSerializeFields))]
public class DefaultPanel : MonoBehaviour
{
    [field: SerializeField] 
    public DefaultPanelSerializeFields SerializeFields 
    { 
        get; 
        private set; 
    }

    public virtual void Awake()
    {
        if (SerializeFields == null)
        {
            SerializeFields = GetComponent<DefaultPanelSerializeFields>();
        }

        if (SerializeFields.Panel == null)
        {
            SerializeFields.SetPanel(gameObject);
        }
    }

    public virtual void Start() { }
    public virtual void Init() { }
    public virtual void InitEvents(LevelController levelController) { }
    public virtual void RemoveEvents(LevelController levelController) { }

    public virtual void SetActive(bool isActive)
    {
        SerializeFields.Panel?.SetActive(isActive);
    }

    public virtual bool IsActive()
    {
        return SerializeFields.Panel.activeSelf;
    }
}