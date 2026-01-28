namespace UDP_Server.Networking.Packets;
public enum MessageType
{
    JoinRequest = 0,
    JoinConfirmation = 1,
    JoinSuccess = 2,
    stateUpdate = 3,
    JoinFailure = 4
}