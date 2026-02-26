using System;
using Newtonsoft.Json;
using SocketIOClient;

public class RoomData
{
    [JsonProperty("roomId")]
    public string roomId { get; set; }
}

public class UserData
{
    [JsonProperty("usedId")]
    public string userId { get; set; }
}

public class MoveData
{
    [JsonProperty("position")]
    public int position { get; set; }
}

public enum MultiplayManagerState
{
    CreateRoom,     // 방 생성
    JoinRoom,       // 방 참가
    StartGame,      // 두 유저가 방에 모두 들어와서 게임을 시작할 때
    ExitRoom,       // 자신이 방 빠져 나왔을 때
    EndGame         // 상대방이 접속을 끊거나 방을 나갔을 때
}

public class MultiplayManager: IDisposable
{
    private SocketIOUnity _socket;
    private event Action<MultiplayManagerState, string> _onMultiplayStateChanged;
    public Action<MoveData> OnOpponentMove;

    public MultiplayManager(Action<MultiplayManagerState, string> onMultiplayStateChanged)
    {
        _onMultiplayStateChanged = onMultiplayStateChanged;

        var uri = new Uri(Constants.SocketURL);
        _socket = new SocketIOUnity(uri, new SocketIOOptions
        {
            Transport = SocketIOClient.Transport.TransportProtocol.WebSocket
        });

        _socket.On("createRoom", CreateRoom);
        _socket.On("joinRoom", JoinRoom);
        _socket.On("startGame", StartGame);
        _socket.On("exitRoom", ExitRoom);
        _socket.On("endGame", EndGame);
        _socket.On("doOpponent", DoOpponent);

        // 서버에 접속
        _socket.Connect();
    }

    // 클라이언트가 서버에 접속했더니 아무도 없어서 방을 새롭게 만들었을 때 서버가 호출해주는 함수
    private void CreateRoom(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
        _onMultiplayStateChanged?.Invoke(MultiplayManagerState.CreateRoom, data.roomId);
    }

    // 클라이언트가 서버에 접속했더니 대기 중인 방이 있어서 그 방에 참가했을 때 서버가 호출해주는 함수
    private void JoinRoom(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
        _onMultiplayStateChanged?.Invoke(MultiplayManagerState.JoinRoom, data.roomId);
    }

    // 방에 참가한 유저가 게임을 시작할 때 서버가 호출해주는 함수
    private void StartGame(SocketIOResponse response)
    {
        var data = response.GetValue<RoomData>();
        _onMultiplayStateChanged?.Invoke(MultiplayManagerState.StartGame, data.roomId);   
    }

    // 방에 참가한 유저가 방을 나갔을 때 서버가 호출해주는 함수
    private void ExitRoom(SocketIOResponse response)
    {
        _onMultiplayStateChanged?.Invoke(MultiplayManagerState.ExitRoom, null);
    }

    // 방에 참가한 유저가 접속을 끊었을 때 서버가 호출해주는 함수
    private void EndGame(SocketIOResponse response)
    {
        _onMultiplayStateChanged?.Invoke(MultiplayManagerState.EndGame, null);
    }

    // 상대방이 게임에서 움직임을 보냈을 때 서버가 호출해주는 함수
    private void DoOpponent(SocketIOResponse response)
    {
        var data = response.GetValue<MoveData>();
        OnOpponentMove?.Invoke(data);
    }

    public void Dispose()
    {
        if (_socket != null)
        {
            _socket.Disconnect();
            _socket.Dispose();
        }
    }
}
