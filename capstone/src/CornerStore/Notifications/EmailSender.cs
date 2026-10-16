namespace CornerStore;

internal class EmailSender : IChannel
{
    public void Send(string recipient, string message)
    {
        Log.Instance.Info("email to " + recipient + ": " + message);
    }
}