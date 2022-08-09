using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public Player Player 
    { 
        get; 
        private set; 
    }
    public List<Enemy> Enemies 
    {
        get; 
        private set; 
    } = new List<Enemy>();
    public List<ActionObject> ActionObjects
    {
        get; 
        private set; 
    } = new List<ActionObject>();


    [field: SerializeField]
    public RespawnManager RespawnManager 
    {
        get; 
        private set; 
    }
    [field: SerializeField]
    public GameCameraController GameCameraController
    {
        get;
        private set;
    }

    [SerializeField]
    private Board board = null;
    [SerializeField] 
    private UIGameManager uIGameManager = null;
    [SerializeField] 
    private TerminalController terminalController = null;

    [Header("-----------------WON CONDITIONS-----------------")]
    [SerializeField] 
    private EventWonData eventWon = null;

    private TurnBasedSystem turnBasedSystem;

    private void Awake()
    {
        turnBasedSystem = new TurnBasedSystem();
    }

    private void Start()
    {
        board.Setup(this);

        RespawnManager.InitEnemiesSpawners(board.GetEnemiesSpawners());
        RespawnManager.InitObjectsSpawners(board.GetObjectsSpawners());
        SetupPlayer();
        SetupEnemies();
        SetupObjects();
        SetupCamera();

        terminalController.Setup(ActionObjects);
        uIGameManager.Setup(this);

    }

    public bool WonLevel()
    {
        if (!eventWon.CheckConditions())
        {
            return false;
        }

        Debug.Log("WON");
        LoadSceneManager.ResetCurrentScene();
        return true;
    }

    public static void LostLevel()
    {
        Debug.Log("LOST");

    //    Player.SetInputControll(false);

        LoadSceneManager.ResetCurrentScene();
    }

    public void CallEventsField(Field field)
    {
        CallFieldEvents(field);
    }

    public void ShowTerminal(bool show)
    {
        terminalController.ShowActionObjectsSignals(show);
        Player.SetInputControll(!show);
        uIGameManager.ShowExitTerminalButton(show);
        GameCameraController.EnabledTerminalCamera(show);
    }

    private void PlayerMoved(Field currentPlayerField)
    {
        CallFieldEvents(currentPlayerField);

        NextTurn();

        EnemiesMove();

        EnemiesEvents(currentPlayerField);
    }

    private void EnemiesMove()
    {
        foreach (var enemy in Enemies)
        {
            enemy.EnemyMovement.Move();
        }
    }

    private void EnemiesEvents(Field currentPlayerStandField)
    {
        foreach (var enemy in Enemies)
        {
            enemy.CheckEvents(currentPlayerStandField);
        }

        CheckEnemyOnField(currentPlayerStandField);
    }

    private void CheckEnemyOnField(Field field)
    {
        if (field.HasEnemy())
        {
            LostLevel();
            return;
        }
    }

    private void CheckWonField(Field field)
    {
        if (field.IsWonField)
        {
            if (WonLevel())
            {
                return;
            }
        }
    }

    private void CallFieldEvents(Field field)
    {
        if(field.Events.Count > 0)
        {
            foreach (var singleEvent in field.Events)
            {
                singleEvent.Execute();
            }
        }

        CheckEnemyOnField(field);
        CheckWonField(field);
    }

    private void NextTurn()
    {
        turnBasedSystem.NextTurn();
    }

    private void SetupPlayer()
    {
        Player = RespawnManager.RespawnPlayer(board.StartField);
        Player.Setup(board, board.StartField.LogicPosition);

        Player.PlayerMoveController.Moved += PlayerMoved;
    }

    private void SetupEnemies()
    {
        Enemies = RespawnManager.RespawnEnemies(board);

        foreach (var enemy in Enemies)
        {
            EnemySniffer enemySniffer = enemy as EnemySniffer;
            if(enemySniffer != null)
            {
                enemySniffer.SetPlayerToFallow(Player);
            }
        }
    }

    private void SetupObjects()
    {
        ActionObjects = RespawnManager.RespawnObjects(board);

        foreach (var singleObject in ActionObjects)
        {
            ActionObjectTerminal terminal = singleObject as ActionObjectTerminal;
            if (terminal != null)
            {
                terminal.Interaction += ShowTerminal;
            }
        }
    }

    private void SetupCamera()
    {
        float space = board.GetLenghtFromLongestSide()  / 2;
        space = board.GetSpaceBetweenFields() * space;

        GameCameraController.Setup(space);

        GameCameraController.TurnCamera += Player.PlayerMoveController.SetMoveAtLookDirection;
    }
}
