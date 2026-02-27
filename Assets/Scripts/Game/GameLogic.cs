using static Constants;
using System;
using UnityEngine;

public class GameLogic : IDisposable
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

    // 멀티 플레이를 처리하는 매니저
    private MultiplayManager _multiplayManager;
    private string _multiplayRoomId;

    // 게임의 결과
    public enum GameResult { None, Win, Lose, Draw }

    // 보드 정보
    public Constants.PlayerType[,] Board { get { return _board; }}

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
                playerBState = new AIState(false);
                
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
            case GameType.MultiPlay:
                // 멀티 플레이어 모드 초기화 작업
                _multiplayManager = new MultiplayManager((state, roomId) =>
                {
                    _multiplayRoomId = roomId;

                    switch (state)
                    {
                        case MultiplayManagerState.CreateRoom:
                            // TODO: "상대방을 기다리고 있습니다." 팝업 표시

                            Debug.Log("방 생성됨, 방 ID: " + _multiplayRoomId);

                            break;
                        case MultiplayManagerState.JoinRoom:

                            Debug.Log("방 참가됨, 방 ID: " + _multiplayRoomId);

                            playerAState = new MultiplayerState(true, _multiplayManager);
                            playerBState = new PlayerState(false, _multiplayManager, _multiplayRoomId);
                            SetState(playerAState);
                            break;
                        case MultiplayManagerState.StartGame:

                            Debug.Log("게임 시작됨, 방 ID: " + _multiplayRoomId);

                            playerAState = new PlayerState(true, _multiplayManager, _multiplayRoomId);
                            playerBState = new MultiplayerState(false, _multiplayManager);
                            SetState(playerAState);
                            break;
                        case MultiplayManagerState.ExitRoom:
                            // TODO: "본인이 방을 나갔습니다." 팝업 표시

                            Debug.Log("본인이 방을 나감, 방 ID: " + _multiplayRoomId);

                            break;
                        case MultiplayManagerState.EndGame:
                            // TODO: "상대방이 접속을 끊었습니다." 팝업 표시

                            Debug.Log("상대방이 접속 끊음, 방 ID: " + _multiplayRoomId);

                            break;
                    }
                });
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
    public GameResult CheckGameResult()
    {
        if (TicTacToeAI.CheckGameWin(PlayerType.Player1, _board)) { return GameResult.Win; }
        if (TicTacToeAI.CheckGameWin(PlayerType.Player2, _board)) { return GameResult.Lose; }
        if (TicTacToeAI.CheckGameDraw(_board)) { return GameResult.Draw; }
        return GameResult.None;
    }

    // 게임오버 처리
    public void EndGame(GameResult gameResult)
    {
        string resultStr = "";
        switch (gameResult)
        {
            case GameResult.Win:
                resultStr = "Player1 승리!";
                break;
            case GameResult.Lose:
                resultStr = "Player2 승리!";
                break;
            case GameResult.Draw:
                resultStr = "무승부";
                break;
        }

        GameManager.Instance.OpenConfirmPanel(resultStr, () =>
        {
            GameManager.Instance.ChangeToMainScene();
        });
    }

    public void Dispose()
    {
        _multiplayManager?.LeaveRoom(_multiplayRoomId);
        _multiplayManager?.Dispose();
    }
}
