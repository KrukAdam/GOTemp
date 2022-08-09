using UnityEngine;

public class ActionObjectSignal : MonoBehaviour
{
    public TerminalLine TerminalLine 
    {
        get;
        set;
    }

    public ActionObjectSignalData SignalData 
    {
        get;
        private set; 
    }

    public ActionObject RootActionObject 
    {
        get;
        private set;
    }

    [SerializeField] 
    private MeshRenderer meshRenderer = null;
    [SerializeField] 
    private Material emptySignalMaterial = null;
    [SerializeField]
    private Material sendSignalMaterial = null;
    [SerializeField] 
    private Material getSignalMaterial = null;

    public static EActionObjectSignalType GetOppositeSignal(EActionObjectSignalType signal)
    {
        switch (signal)
        {
            case EActionObjectSignalType.None:
                break;
            case EActionObjectSignalType.Send:
                return EActionObjectSignalType.Get;
            case EActionObjectSignalType.Get:
                return EActionObjectSignalType.Send;
            default:
                break;
        }

        return EActionObjectSignalType.None;
    }

    public void Setup(ActionObjectSignalData actionObjectSignalData, ActionObject rootObject)
    {
        SignalData = actionObjectSignalData;
        this.RootActionObject = rootObject;
        SetMaterial(SignalData.SignalType);
    }

    public void SetSignal(ActionObjectSignalData actionObjectSignalData)
    {
        SignalData = actionObjectSignalData;
        RootActionObject.ActionObjectsSignalDatas.Add(actionObjectSignalData);
        SetMaterial(SignalData.SignalType);
    }

    public void RemoveSignal()
    {
        RootActionObject.ActionObjectsSignalDatas.Remove(SignalData);
        SignalData.ActionObject = null;
        SetMaterial(EActionObjectSignalType.None);
    }

    public void Show(bool show)
    {
        gameObject.SetActive(show);
    }

    private void SetMaterial(EActionObjectSignalType signalType)
    {
        switch (signalType)
        {
            case EActionObjectSignalType.None:
                meshRenderer.material = emptySignalMaterial;
                break;
            case EActionObjectSignalType.Send:
                meshRenderer.material = sendSignalMaterial;
                break;
            case EActionObjectSignalType.Get:
                meshRenderer.material = getSignalMaterial;
                break;
            default:
                break;
        }
    }

}
