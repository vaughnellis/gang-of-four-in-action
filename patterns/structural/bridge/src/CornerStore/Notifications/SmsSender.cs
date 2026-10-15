namespace CornerStore;

internal class SmsSender : IChannel
{
    public void Send(string recipient, string message)
    {
        Log.Info("sms to " + recipient + ": " + message);
    }
}