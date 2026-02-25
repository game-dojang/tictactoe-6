using UnityEngine;

public class MultiplayState : BaseState
{
    private Constants.PlayerType _playerType;

    public MultiplayState(bool isFirstPlayer)
    {
        _playerType = isFirstPlayer ? Constants.PlayerType.Player1 : Constants.PlayerType.Player2;
    }

    public override void HandleMove(GameLogic gameLogic, int index)
    {
        
    }

    public override void HandleNextTurn(GameLogic gameLogic)
    {
        
    }

    public override void OnEnter(GameLogic gameLogic)
    {
        
    }

    public override void OnExit(GameLogic gameLogic)
    {
        
    }
}
