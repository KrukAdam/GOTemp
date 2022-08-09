using System;
using UnityEngine;

public class DefaultChangeViewButton : DefaultButton
{
    public event Action<DefaultChangeViewButton> ButtonActivated = delegate { };

    public bool ActiveButton => SerializeFields.Image.gameObject.activeSelf;

    [field: SerializeField]
    public DefaultPanel PanelView
    {
        get;
        private set;
    } = null;

    [field: SerializeField]
    public bool IsOnlyClose
    {
        get;
        private set;
    }

    [SerializeField]
    private bool openAndCloseView = false;

    public override void OnClick()
    {
        if (PanelView == null)
        {
            SerializeFields.Image.gameObject.SetActive(!ActiveButton);

            if (ActiveButton)
            {
                ButtonActivated(this);
            }

            return;
        }

        bool activePanel = ActiveButton;

        if (!IsOnlyClose)
        {
            if (openAndCloseView)
            {
                activePanel = !PanelView.IsActive();
            }
            else
            {
                activePanel = true;
            }
        }

        SerializeFields.Image.gameObject.SetActive(activePanel);
        PanelView.SetActive(activePanel);

        if (activePanel)
        {
            ButtonActivated(this);
        }
    }

    public void SetTargetView(DefaultPanel panelView)
    {
        PanelView = panelView;
    }
}
