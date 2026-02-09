using static Constants;

public class GameLogic
{
    // 화면에 Block을 제어하기 위한 변수
    private BlockController _blockController;

    // 보드의 상태
    private PlayerType[,] _board;

    // 플레이어 상태 변수
    public BaseState playerAState;
    public BaseState playerBState;

    // 현재 상태를 나타내는 변수
    private BaseState _currentState;

    public GameLogic(GameType gameType, BlockController blockController)
    {
        // BlockController 할당
        _blockController = blockController;

        // 보드 정보 초기화
        _board = new PlayerType[BOARD_SIZE, BOARD_SIZE];

        // GameType에 따른 초기화 작업 수행
        switch (gameType)
        {
            case GameType.SinglePlay:
                // 싱글 플레이어 모드 초기화 작업
                playerAState = new PlayerState();
                playerBState = new AIState();
                break;
            case GameType.DualPlay:
                // 듀얼 플레이어 모드 초기화 작업
                playerAState = new PlayerState();
                playerBState = new PlayerState();
                break;
        }
    }

    // 턴 바뀔 때 호출되는 메서드 (상태 전환 메서드)
    public void SetState(BaseState newState)
    {
        _currentState?.OnExit();
        _currentState = newState;
        _currentState.OnEnter();
    }
}
