using UnityEngine;

public class CameraPanel : DefaultPanel
{
    [SerializeField]
    private CameraButton btnTurnLeftCamera = null;
    [SerializeField]
    private CameraButton btnTurnRightCamera = null;

    public override void InitEvents(LevelController levelController)
    {
        GameCameraController cameraController = levelController.GameCameraController;

        btnTurnLeftCamera.SetOnClick(cameraController.TurnCameraLeft);
        btnTurnRightCamera.SetOnClick(cameraController.TurnCameraRight);
    }
}
