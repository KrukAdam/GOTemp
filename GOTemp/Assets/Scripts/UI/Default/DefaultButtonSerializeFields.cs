using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DefaultButtonSerializeFields : MonoBehaviour
{
    [field: SerializeField]
    public Button Button
    {
        get;
        private set;
    }

    [field: SerializeField]
    public Image Image
    {
        get;
        set;
    }

    [field: SerializeField]
    public TMP_Text TextMeshPro
    {
        get;
        private set;
    }
}