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


public class MultiplayManager
{
    private SocketIOUnity _socket;
    private event Action _onMultiplayStateChanged;
    public Action OnReceiveMessage;
    public Action OnOpponentMove;

    public MultiplayManager(Action onMultiplayStateChanged)
    {
        _onMultiplayStateChanged = onMultiplayStateChanged;

    }
}
