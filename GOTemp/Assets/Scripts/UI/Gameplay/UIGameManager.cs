using System.Collections.Generic;
using UnityEngine;

public class UIGameManager : MonoBehaviour
{
    [field: SerializeField]
    public CameraPanel CameraPanel { get; private set; }

    [SerializeField]
    private DefaultButton btnCloseTerminal = null;

    private LevelController levelController;
    private List<DefaultPanel> panels = new List<DefaultPanel>();

    public void Setup(LevelController levelController)
    {
        this.levelController = levelController;

        SetPanels();
        InitPanels();

        btnCloseTerminal.SetOnClick(CloseTerminal);
        ShowExitTerminalButton(false);
    }

    private void OnDestroy()
    {
        foreach (var panel in panels)
        {
            panel.RemoveEvents(levelController);
        }
    }

    public void CloseTerminal()
    {
        levelController.ShowTerminal(false);
    }

    public void ShowExitTerminalButton(bool show)
    {
        btnCloseTerminal.gameObject.SetActive(show);
    }

    private void SetPanels()
    {
        panels.Clear();

        panels.Add(CameraPanel);
    }

    private void InitPanels()
    {
        foreach (var panel in panels)
        {
            panel.Init();
            panel.InitEvents(levelController);
        }
    }
}
