using GambitUtils.UI;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EEnemyType EnemyType = EEnemyType.Watcher;
    public EDirection StartLookDirection = EDirection.Top;

    [HideInInspector]
    public FieldGrid GridToSpawn = null;

    [ConditionalHide("showTurnLeft", true)]
    public bool TurnLeft = false;

    [SerializeField, HideInInspector]
    private bool showTurnLeft = false;

    public void SetupEnemyData()
    {
        showTurnLeft = false;

        switch (EnemyType)
        {
            case EEnemyType.None:
                break;
            case EEnemyType.Watcher:
                break;
            case EEnemyType.Runner:
                break;
            case EEnemyType.Tower:
                break;
            case EEnemyType.Pair:
                break;
            case EEnemyType.Patrol:
                showTurnLeft = true;
                break;
            case EEnemyType.Sniffer:
                break;
            case EEnemyType.Hunter:
                break;
            case EEnemyType.Count:
                break;
            default:
                break;
        }
    }

}
