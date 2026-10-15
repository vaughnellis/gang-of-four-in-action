namespace CornerStore;

internal class EmailSender
{
    public void Send(string recipient, string message)
    {
        Log.Info("email to " + recipient + ": " + message);
    }
}