
public class PlayerState : BaseState
{
    private Constants.PlayerType _playerType;

    public PlayerState(bool isFirstPlayer)
    {
        _playerType = isFirstPlayer ? Constants.PlayerType.Player1 : Constants.PlayerType.Player2;
    }

    // 턴 변경
    public override void HandleNextTurn(GameLogic gameLogic)
    {
        gameLogic.ChangeGameState();
    }

    public override void OnEnter(GameLogic gameLogic)
    {
        // 상태 진입 시 로직 구현
        gameLogic.blockController.onBlockClicked = (blockIndex) =>
        {
            // 블록이 클릭되었을 때 처리할 로직
            HandleMove(gameLogic, blockIndex);
        };
    }

    public override void HandleMove(GameLogic gameLogic, int index)
    {
        ProcessMove(gameLogic, index, _playerType);
    }

    public override void OnExit(GameLogic gameLogic)
    {
    }
}
