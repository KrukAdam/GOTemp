using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    public List<ObjectSpawner> SpawnersObjects
    {
        get => spawnersObjects; 
    }

    [SerializeField]
    private Player playerPrefab = null;

    [Header("---------------ENEMIES---------------")]
    [SerializeField]
    private Enemy enemyWarcherPrefab = null;
    [SerializeField]
    private Enemy enemyPairPrefab = null;
    [SerializeField]
    private Enemy enemyTowerPrefab = null;
    [SerializeField]
    private Enemy enemyRunnerPrefab = null;
    [SerializeField]
    private Enemy enemyPatrolPrefab = null;
    [SerializeField]
    private Enemy enemySnifferPrefab = null;
    [SerializeField]
    private Enemy enemyHunterPrefab = null;

    [Header("---------------OBJECTS---------------")]
    [SerializeField]
    private ActionObject objectStillCameraPrefab = null;
    [SerializeField]
    private ActionObject objectRotatingCameraPrefab = null;
    [SerializeField]
    private ActionObject objectSwitchPrefab = null;
    [SerializeField]
    private ActionObject objectPressurePlatePrefab = null;
    [SerializeField]
    private ActionObject objectFanPassagePrefab = null;
    [SerializeField]
    private ActionObject objectConveyorBeltPrefab = null;
    [SerializeField]
    private ActionObject objectPlatformPrefab = null;
    [SerializeField] 
    private ActionObject objectDoorPrefab = null;
    [SerializeField]
    private ActionObject objectLaserPrefab = null;
    [SerializeField]
    private ActionObject objectTerminalPrefab = null;
    [SerializeField]
    private ActionObject objectAlarmPrefab = null;
    [SerializeField]
    private ActionObject objectDecoyPrefab = null;

    [SerializeField][HideInInspector] 
    private List<EnemySpawner> spawnersEnemies = new List<EnemySpawner>();
    [SerializeField][HideInInspector]
    private List<ObjectSpawner> spawnersObjects = new List<ObjectSpawner>();

    private List<Enemy> enemies = new List<Enemy>();
    private List<ActionObject> actionObjects = new List<ActionObject>();

    public void InitEnemiesSpawners(List<EnemySpawner> enemySpawners)
    {
        this.spawnersEnemies = enemySpawners;
    }

    public void InitObjectsSpawners(List<ObjectSpawner> objectSpawners)
    {
        this.spawnersObjects = objectSpawners;
    }

    public Player RespawnPlayer(Field fieldToRespawn)
    {
        Player player = Instantiate(playerPrefab);
        player.transform.position = fieldToRespawn.StandTransform.position;

        return player;
    }

    public List<Enemy> RespawnEnemies(Board board)
    {
        List<int> nullsSpawnersIndex = new List<int>();

        for (int i = 0; i < spawnersEnemies.Count; i++)
        {
            if (spawnersEnemies[i] != null)
            {
                Enemy enemy = RespawnEnemy(spawnersEnemies[i].EnemyType, spawnersEnemies[i].GridToSpawn.Field);
                if (enemy == null)
                {
                    Debug.LogError($"Enemy is null. Check Enemy on {spawnersEnemies[i].GridToSpawn.LogicPosition} - {spawnersEnemies[i].EnemyType}");
                    return null;
                }

                enemy.Setup(spawnersEnemies[i], board);
                enemies.Add(enemy);
            }
            else
            {
                nullsSpawnersIndex.Add(i);
            }
        }

        return enemies;
    }

    public List<ActionObject> RespawnObjects(Board board)
    {
        if(spawnersObjects.Count <= 0)
        {
            return actionObjects;
        }

        //Respawn Object
        List<ObjectSpawner> nullsSpawners = new List<ObjectSpawner>();
        for (int i = 0; i < spawnersObjects.Count; i++)
        {
            if(spawnersObjects[i] != null)
            {
                ActionObject actionObject = RespawnObject(spawnersObjects[i].ObjectType, spawnersObjects[i]);
                ActionPathObject actionPathObject = actionObject as ActionPathObject;
                ObjectPathSpawner objectPathSpawner = spawnersObjects[i] as ObjectPathSpawner;

                if (actionObject == null)
                {
                    Debug.LogError($"Object is null. Check Object on {spawnersObjects[i].GridToSpawn.LogicPosition} - {spawnersObjects[i].ObjectType}");
                    return null;
                }

                actionObjects.Add(actionObject);

                if (actionPathObject == null)
                {
                    spawnersObjects[i].GridToSpawn.Field.ActionObject = actionObject;
                }
                else
                {
                    objectPathSpawner.PathGridToSpawn.FieldPath.ActionObject = actionPathObject;
                }
            }
            else
            {
                nullsSpawners.Add(spawnersObjects[i]);
            }
        }

        foreach (var item in nullsSpawners)
        {
            spawnersObjects.Remove(item);
        }

        //Bin child interaction objects
        for (int i = 0; i < spawnersObjects.Count; i++)
        {
            for (int z = 0; z < spawnersObjects.Count; z++)
            {
                if (spawnersObjects[i].ActionObjectSpawnerInteraction == spawnersObjects[z])
                {
                    actionObjects[i].ActionObjectInterac = actionObjects[z];
                }

                if (spawnersObjects[i].ActionObjectSpawnerSignal1 == spawnersObjects[z] || spawnersObjects[i].ActionObjectSpawnerSignal2 == spawnersObjects[z])
                {
                    actionObjects[i].ActionObjectsSignalDatas.Add(new ActionObjectSignalData(actionObjects[z], EActionObjectSignalType.Send));
                    actionObjects[z].ActionObjectsSignalDatas.Add(new ActionObjectSignalData(actionObjects[i], EActionObjectSignalType.Get));
                }
            }
        }

        for (int i = 0; i < spawnersObjects.Count; i++)
        {
            if (actionObjects[i].ActionObjectsSignalDatas.Count < actionObjects[i].ActionObjectSignals.Count)
            {
                int emptySignlas = actionObjects[i].ActionObjectSignals.Count - actionObjects[i].ActionObjectsSignalDatas.Count;
                if (emptySignlas > 0)
                {
                    for (int e = 0; e < emptySignlas; e++)
                    {
                        actionObjects[i].ActionObjectsSignalDatas.Add(new ActionObjectSignalData(null, EActionObjectSignalType.None));
                    }

                }
            }
        }

        //Setup objects
        for (int i = 0; i < spawnersObjects.Count; i++)
        {
            actionObjects[i].Setup(spawnersObjects[i], board);
        }

        return actionObjects;
    }

    private Enemy RespawnEnemy(EEnemyType enemyType, Field fieldToRespawn)
    {
        Enemy enemy;

        switch (enemyType)
        {
            case EEnemyType.None:
                return null;
            case EEnemyType.Watcher:
                enemy = Instantiate(enemyWarcherPrefab);
                enemy.transform.position = fieldToRespawn.StandTransform.position;

                return enemy;
            case EEnemyType.Runner:
                enemy = Instantiate(enemyRunnerPrefab);
                enemy.transform.position = fieldToRespawn.StandTransform.position;

                return enemy;
            case EEnemyType.Tower:
                enemy = Instantiate(enemyTowerPrefab);
                enemy.transform.position = fieldToRespawn.StandTransform.position;

                return enemy;
            case EEnemyType.Pair:
                enemy = Instantiate(enemyPairPrefab);
                enemy.transform.position = fieldToRespawn.StandTransform.position;

                return enemy;
            case EEnemyType.Patrol:
                enemy = Instantiate(enemyPatrolPrefab);
                enemy.transform.position = fieldToRespawn.StandTransform.position;

                return enemy;
            case EEnemyType.Sniffer:
                enemy = Instantiate(enemySnifferPrefab);
                enemy.transform.position = fieldToRespawn.StandTransform.position;

                return enemy;
            case EEnemyType.Hunter:
                enemy = Instantiate(enemyHunterPrefab);
                enemy.transform.position = fieldToRespawn.StandTransform.position;

                return enemy;
            case EEnemyType.Count:
            default:
                return null;
        }
    }

    private ActionObject RespawnObject(EActionObjectType objectType, ObjectSpawner objectSpawner)
    {
        ObjectPathSpawner objectPathSpawner = objectSpawner as ObjectPathSpawner;
        Vector3 respPos;
        if(objectPathSpawner == null)
        {
            respPos = objectSpawner.GridToSpawn.Field.StandTransform.position;
        }
        else
        {
            respPos = objectPathSpawner.PathGridToSpawn.FieldPath.StandTransform.position;
        }

        ActionObject actionObject = null;

        switch (objectType)
        {
            case EActionObjectType.None:
                break;
            case EActionObjectType.Switch:
                actionObject = Instantiate(objectSwitchPrefab);
                break;
            case EActionObjectType.PressurePlate:
                actionObject = Instantiate(objectPressurePlatePrefab);
                break;
            case EActionObjectType.StillCamera:
                actionObject = Instantiate(objectStillCameraPrefab);
                break;
            case EActionObjectType.RotatingCamera:
                actionObject = Instantiate(objectRotatingCameraPrefab);
                break;
            case EActionObjectType.Laser:
                actionObject = Instantiate(objectLaserPrefab);
                break;
            case EActionObjectType.Alarm:
                actionObject = Instantiate(objectAlarmPrefab);
                break;
            case EActionObjectType.Doors:
                actionObject = Instantiate(objectDoorPrefab);
                break;
            case EActionObjectType.FanPassage:
                actionObject = Instantiate(objectFanPassagePrefab);
                break;
            case EActionObjectType.ConveyorBelt:
                actionObject = Instantiate(objectConveyorBeltPrefab);
                break;
            case EActionObjectType.Platform:
                actionObject = Instantiate(objectPlatformPrefab);
                break;
            case EActionObjectType.Terminal:
                actionObject = Instantiate(objectTerminalPrefab);
                break;
            case EActionObjectType.Decoy:
                actionObject = Instantiate(objectDecoyPrefab);
                break;
            case EActionObjectType.Count:
                break;
            default:
                break;
        }

        if(actionObject == null)
        {
            Debug.LogError($"You have object on map with type: NONE/COUNT. Change type or remove object - {objectSpawner.gameObject.name} ");
            return null;
        }

        actionObject.transform.position = respPos;

        return actionObject;

    }
}
