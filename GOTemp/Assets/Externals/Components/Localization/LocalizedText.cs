using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GambitLocalization;

public class LocalizedText : MonoBehaviour
{
    [Header("Use one of text components")]
    [SerializeField]
    TextMeshProUGUI tmptext = null;
    [SerializeField]
    UnityEngine.UI.Text text = null;
    [Header("Enter the key from localizations sheet")]
    [SerializeField]
    string key = "";

    private void Start()
    {
        LocalizationManager.Instance.RegisterAction(DisplayText);
    }

    public void DisplayText(LocalizationContext context)
    {
        string s = context.GetText(key);
        if (tmptext == null)
        {
            text.text = s;
            return;
        }
        tmptext.text = s;
    }

    private void OnDestroy()
    {
        LocalizationManager.Instance.UnregisterAction(DisplayText);
    }
}
