using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SerializedSingleton<GameManager>
{
    public Action OnStartPlayerTurn;
    public Action OnStartEnemyTurn;

    public bool IsPlayerTurn
    {
        get { return CurrentGameState == GameState.PlayerTurn; }
    }

    [EnumToggleButtons]
    [FoldoutGroup("Config")]
    [ReadOnly]
    public GameState CurrentGameState = GameState.Idle;

    [FoldoutGroup("Config"), SerializeField]
    CommandQueue enemyCommandQueue = new();


    [FoldoutGroup("Reference"), SerializeField, Required] GameObject mainMenu;
    [FoldoutGroup("Reference"), SerializeField, Required] Button playButton;
    [FoldoutGroup("Reference"), SerializeField, Required] Button exitButton;


    protected override void InitAfterAwake()
    {
        playButton.onClick.AddListener(StartPlayerTurn);
        exitButton.onClick.AddListener(Application.Quit);
    }

    private void Start()
    {
        mainMenu.SetActive(true);

        // เมื่อเริ่มเทิร์นศัตรู ให้เริ่มเดินทีละตัว
        OnStartEnemyTurn += enemyCommandQueue.TryExecuteCommands;

        // เมื่อโจมตีเสร็จทุกตัว ให้เริ่มเทิร์นผู้เล่น
        enemyCommandQueue.OnLastCommandExecuted += StartPlayerTurn;

        enemyCommandQueue.OnLastCommandExecuted += CameraManager.Instance.StopCloseUpEnemy;
    }

    private void Update()
    {
    }

    [FoldoutGroup("Config")]
    [Button]
    [GUIColor("green")]
    public void StartPlayerTurn()
    {
        mainMenu.SetActive(false);
        CurrentGameState = GameState.PlayerTurn;
        OnStartPlayerTurn?.Invoke();
    }

    [FoldoutGroup("Config")]
    [Button]
    [GUIColor("red")]
    public void StartEnemyTurn()
    {
        CurrentGameState = GameState.EnemyTurn;
        OnStartEnemyTurn?.Invoke();

    }

    public void AddCommand(Command command, Command.OnCompleteCallback onCompleteCallback = null)
    {
        if (onCompleteCallback != null)
        {
            command.OnComplete(onCompleteCallback);
        }

        enemyCommandQueue.AddCommand(command);
    }

}


