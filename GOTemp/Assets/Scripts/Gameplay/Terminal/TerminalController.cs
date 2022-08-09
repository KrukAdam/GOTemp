using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TerminalController : MonoBehaviour
{
    [SerializeField]
    private Camera cameraMain = null;
    [SerializeField] 
    private TerminalLineController terminalLineController = null;
    [SerializeField] 
    private LayerMask signalMask;

    private List<ActionObject> actionObjects = new List<ActionObject>();
    private ActionObjectSignal currentSignalSelected = null;
    private Ray ray;
    private RaycastHit hit;
    private int layer = 7;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ray = cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out hit))
            {
                // layer = LayerMask.GetMask("Signal");
                if (hit.collider.gameObject.layer == layer)
                {
                    ActionObjectSignal actionObjectSignal = hit.collider.gameObject.GetComponent<ActionObjectSignal>();
                    if (actionObjectSignal != null)
                    {
                        SignalConnect(actionObjectSignal);
                    }
                }
            }
        }
    }

    public void Setup(List<ActionObject> actionObjects)
    {
        this.actionObjects = actionObjects;

        SetupLine();
    }

    public void ShowActionObjectsSignals(bool show)
    {
        foreach (var singleObject in actionObjects)
        {
            singleObject.ShowSignalsDiode(show);
        }

        terminalLineController.ShowLines(show);
    }

    private void SetupLine()
    {
        foreach (var singleObject in actionObjects)
        {
            foreach (var signalObject in singleObject.ActionObjectSignals)
            {
                if (signalObject.SignalData.ActionObject != null && signalObject.TerminalLine == null)
                {
                    ActionObjectSignal mainSignalObject = signalObject.SignalData.ActionObject.GetSignalObject(signalObject.RootActionObject, EActionObjectSignalType.Send);
                    if (mainSignalObject != null)
                    {
                        TerminalLine terminalLine = terminalLineController.DrawLine(signalObject.transform.position, mainSignalObject.transform.position);
                        signalObject.TerminalLine = terminalLine;
                        mainSignalObject.TerminalLine = terminalLine;
                    }
                }
            }
        }

        terminalLineController.ShowLines(false);
    }

    private void SignalConnect(ActionObjectSignal actionObjectSignal)
    {
        ActionObjectSignalData actionObjectSignalData = actionObjectSignal.SignalData;

        if (actionObjectSignalData.ActionObject != null)
        {
            currentSignalSelected = actionObjectSignal;
        }
        else
        {
            if (currentSignalSelected == null)
            {
                return;
            }

            if (!actionObjectSignal.RootActionObject.CheckCanBindSignal(currentSignalSelected.SignalData))
            {
                Debug.Log("Cant bind two same signal from this same object to this action object.");
                return;
            }

            ActionObjectSignal mainSignalObject = currentSignalSelected.SignalData.ActionObject.GetSignalObject(currentSignalSelected.RootActionObject, currentSignalSelected.SignalData.SignalType);
            //Add new signal
            actionObjectSignal.SetSignal(new ActionObjectSignalData(mainSignalObject.RootActionObject, currentSignalSelected.SignalData.SignalType));
            mainSignalObject.SignalData.ActionObject = actionObjectSignal.RootActionObject;
            //Remove old signal
            currentSignalSelected.RemoveSignal();
            //Draw new line
            TerminalLine terminalLine = terminalLineController.DrawLine(actionObjectSignal.transform.position, mainSignalObject.transform.position);
            actionObjectSignal.TerminalLine = terminalLine;
            mainSignalObject.TerminalLine = terminalLine;

            //Remove old line
            terminalLineController.RemoveLine(currentSignalSelected.TerminalLine);
            currentSignalSelected.TerminalLine = null;

            currentSignalSelected = null;
        }
    }
}
