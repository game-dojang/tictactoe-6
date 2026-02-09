
public abstract class BaseState
{
    public abstract void OnEnter();             // 상태 진입 시 호출
    public abstract void HandleMove();          // 플레이어 이동 처리
    public abstract void OnExit();              // 상태 종료 시 호출
    public abstract void HandleNextTurn();      // 다음 턴 처리
}
