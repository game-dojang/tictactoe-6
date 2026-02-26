using UnityEngine;

public class MultiplayerState : BaseState
{
    private Constants.PlayerType _playerType;
    private MultiplayManager _multiplayManager;

    public MultiplayerState(bool isFirstPlayer, MultiplayManager multiplayManager)
    {
        _playerType = isFirstPlayer ? Constants.PlayerType.Player1 : Constants.PlayerType.Player2;
        _multiplayManager = multiplayManager;
    }

    public override void HandleMove(GameLogic gameLogic, int index)
    {
        ProcessMove(gameLogic, index, _playerType);
    }

    public override void HandleNextTurn(GameLogic gameLogic)
    {
        gameLogic.ChangeGameState();
    }

    public override void OnEnter(GameLogic gameLogic)
    {
        
    }

    public override void OnExit(GameLogic gameLogic)
    {
        
    }
}
