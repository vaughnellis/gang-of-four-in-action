namespace CornerStore;

internal interface IChannel
{
    void Send(string recipient, string message);
}
