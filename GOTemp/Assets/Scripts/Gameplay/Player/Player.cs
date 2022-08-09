using UnityEngine;

public class Player : MonoBehaviour
{
    [field: SerializeField]
    public PlayerMoveController PlayerMoveController 
    {
        get;
        private set; 
    }
    [field: SerializeField]
    public PlayerActionController PlayerActionController 
    {
        get;
        private set;
    }

    public void Setup(Board board, Vector2Int logicPos)
    {
        PlayerMoveController.Setup(board, logicPos);
        PlayerActionController.Setup();
    }

    public void SetInputControll(bool hasControll)
    {
        PlayerMoveController.SetCanMove(hasControll);
        PlayerActionController.CanUse(hasControll);
    }
}
