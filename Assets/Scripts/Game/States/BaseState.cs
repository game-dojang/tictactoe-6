
public abstract class BaseState
{
    public abstract void OnEnter(GameLogic gameLogic);                      // 상태 진입 시 호출
    public abstract void HandleMove(GameLogic gameLogic, int index);        // 플레이어 이동 처리
    public abstract void OnExit(GameLogic gameLogic);                       // 상태 종료 시 호출
    public abstract void HandleNextTurn(GameLogic gameLogic);               // 다음 턴 처리

    public void ProcessMove(GameLogic gameLogic, int index, Constants.PlayerType playerType)
    {
        // 특정 위치에 마커 표시
        gameLogic.PlaceMarker(index, playerType);

        // 턴 전환
        HandleNextTurn(gameLogic);
    }
}
