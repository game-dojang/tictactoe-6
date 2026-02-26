
public static class Constants
{
    public const string SCENE_MAIN = "Main";
    public const string SCENE_GAME = "Game";

    public enum GameType { SinglePlay, DualPlay, MultiPlay }
    public enum PlayerType { None, Player1, Player2 }

    // 보드 크기
    public const int BOARD_SIZE = 3;

    // 서버 주소
    public const string ServerURL = "http://localhost:3000";    // HTTP 서버 주소
    public const string SocketURL = "ws://localhost:3000";      // Socket.IO 서버 주소
}
