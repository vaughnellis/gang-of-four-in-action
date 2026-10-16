namespace CornerStore;

internal class SmsSender : IChannel
{
    public void Send(string recipient, string message)
    {
        Log.Instance.Info("sms to " + recipient + ": " + message);
    }
}