using GambitLocalization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedImage : MonoBehaviour
{
    [SerializeField]
    Image imageComponent = null;
    [SerializeField]
    string key = "";

    private void Start()
    {
        LocalizationManager.Instance.RegisterAction(DisplayImage);
    }

    public void DisplayImage(LocalizationContext context)
    {
        Sprite s = context.GetSprite(key);
        imageComponent.sprite = s;
    }

    private void OnDestroy()
    {
        LocalizationManager.Instance.UnregisterAction(DisplayImage);
    }
}
