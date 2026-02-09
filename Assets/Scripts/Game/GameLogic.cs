using static Constants;

public class GameLogic
{
    // 화면에 Block을 제어하기 위한 변수
    public BlockController blockController;

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
        this.blockController = blockController;

        // 보드 정보 초기화
        _board = new PlayerType[BOARD_SIZE, BOARD_SIZE];

        // GameType에 따른 초기화 작업 수행
        switch (gameType)
        {
            case GameType.SinglePlay:
                // 싱글 플레이어 모드 초기화 작업
                playerAState = new PlayerState(true);
                playerBState = new AIState();
                
                // 초기 상태 설정 (예: 플레이어 A부터 시작)
                SetState(playerAState);
                break;
            case GameType.DualPlay:
                // 듀얼 플레이어 모드 초기화 작업
                playerAState = new PlayerState(true);
                playerBState = new PlayerState(false);

                // 초기 상태 설정 (예: 플레이어 A부터 시작)
                SetState(playerAState);
                break;
        }
    }

    // 턴 바뀔 때 호출되는 메서드 (상태 전환 메서드)
    public void SetState(BaseState newState)
    {
        _currentState?.OnExit(this);
        _currentState = newState;
        _currentState.OnEnter(this);
    }

    // 마커 표시를 위한 메서드
    public bool PlaceMarker(int index, PlayerType playerType)
    {
        var row = index / BOARD_SIZE;
        var col = index % BOARD_SIZE;

        // 해당 위치에 이미 마커가 있는지 확인, 뭔가 있으면 false 반환
        if (_board[row, col] != Constants.PlayerType.None) return false;

        blockController.PlaceMarker(index, playerType);
        _board[row, col] = playerType;
        
        return true;
    }

    // 턴 변경
    public void ChangeGameState()
    {
        if (_currentState == playerAState)
        {
            SetState(playerBState);
        }
        else
        {
            SetState(playerAState);
        }
    }

    // 게임 결과 확인
    public void CheckGameResult()
    {
        // 승리 조건 확인 로직 구현 (생략)
    }

    // public bool CheckGameWin(Constants.PlayerType playerType, Constants.PlayerType[,] board)
    // {
        
    // }
}
