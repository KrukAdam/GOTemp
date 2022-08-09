using UnityEngine;
using Cinemachine;
using GambitUtils;
using System;

public class GameCameraController : MonoBehaviour
{
    public Action<EDirection> TurnCamera = delegate { };

    [SerializeField]
    private CinemachineVirtualCamera terminalCamera = null;
    [SerializeField]
    private CinemachineVirtualCamera cameraDown = null;
    [SerializeField]
    private CinemachineVirtualCamera cameraUp = null;
    [SerializeField]
    private CinemachineVirtualCamera cameraLeft = null;
    [SerializeField]
    private CinemachineVirtualCamera cameraRight = null;
    [Header("-----CAM SETTINGS-----")]
    [SerializeField]
    private Vector3 increasePosInTerminal = Vector3.zero;
    [SerializeField]
    private Vector3 increasePosDownCamera = Vector3.zero;
    [SerializeField]
    private Vector3 increasePosUpCamera = Vector3.zero;
    [SerializeField]
    private Vector3 increasePosLeftCamera = Vector3.zero;
    [SerializeField]
    private Vector3 increasePosRightCamera = Vector3.zero;

    private float distanceFromTarget = 3.0f;
    private Vector3 targetPos;
    private Vector3 posInTerminal;
    private EDirection currentCameraDirection = EDirection.Down;

    public void Setup(float space)
    {
        float yPos = space * 2;
        if (yPos < 10)
        {
            //Set min distance 
            yPos = 10;
        }

        distanceFromTarget = yPos * 1.5f;

        Vector3 newCameraPos = new Vector3(yPos / 2.5f, yPos, yPos / -5);
        targetPos = newCameraPos;

        newCameraPos = new Vector3(targetPos.x + increasePosDownCamera.x, targetPos.y + increasePosDownCamera.y, targetPos.z + increasePosDownCamera.z);
        cameraDown.gameObject.transform.position = newCameraPos;

        newCameraPos = new Vector3(targetPos.x /-2 + increasePosLeftCamera.x, targetPos.y + increasePosLeftCamera.y, targetPos.z * -2 + increasePosLeftCamera.y);
        cameraLeft.gameObject.transform.position = newCameraPos;

        newCameraPos = new Vector3(targetPos.y + increasePosRightCamera.x, targetPos.y + increasePosRightCamera.y, targetPos.z * -2f + increasePosRightCamera.z);
        cameraRight.gameObject.transform.position = newCameraPos;

        newCameraPos = new Vector3(targetPos.x + increasePosUpCamera.x, targetPos.y + increasePosUpCamera.y, targetPos.y + increasePosUpCamera.z);
        cameraUp.gameObject.transform.position = newCameraPos;

        posInTerminal = new Vector3(targetPos.x + increasePosInTerminal.x, distanceFromTarget + increasePosInTerminal.y, targetPos.z * -2 + increasePosInTerminal.z);
        terminalCamera.gameObject.transform.position = posInTerminal;

        //Start look camera
        EnabledCamera(EDirection.Down);
    }

    public void TurnCameraLeft()
    {
   
        EnabledCamera(Direction.GetRightFromDirection(currentCameraDirection));
    }

    public void TurnCameraRight()
    {
        EnabledCamera(Direction.GetLeftFromDirection(currentCameraDirection));
    }

    public void EnabledTerminalCamera(bool enabled)
    {
        if (enabled)
        {
            EnabledCamera(currentCameraDirection, true);
        }
        else
        {
            EnabledCamera(currentCameraDirection);
        }
    }

    private void EnabledCamera(EDirection camDirection, bool onTerminalCam = false)
    {
        currentCameraDirection = camDirection;

        cameraDown.SetGameObjectActive(false);
        cameraUp.SetGameObjectActive(false);
        cameraLeft.SetGameObjectActive(false);
        cameraRight.SetGameObjectActive(false);
        terminalCamera.SetGameObjectActive(false);

        if (onTerminalCam)
        {
            terminalCamera.SetGameObjectActive(true);
            return;
        }

        switch (camDirection)
        {
            case EDirection.None:
                break;
            case EDirection.Left:
                cameraLeft.SetGameObjectActive(true);
                break;
            case EDirection.Right:
                cameraRight.SetGameObjectActive(true);
                break;
            case EDirection.Top:
                cameraUp.SetGameObjectActive(true);
                break;
            case EDirection.Down:
                cameraDown.SetGameObjectActive(true);
                break;
            case EDirection.Count:
                break;
            default:
                break;
        }

        TurnCamera(currentCameraDirection);
    }

}
