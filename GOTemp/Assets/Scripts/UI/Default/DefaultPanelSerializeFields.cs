using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultPanelSerializeFields : MonoBehaviour
{
    [field: SerializeField] public GameObject Panel { get; private set; }

    public void SetPanel(GameObject gameObject)
    {
        Panel = gameObject;
    }
}
