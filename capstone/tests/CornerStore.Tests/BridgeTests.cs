namespace CornerStore.Tests;

public class BridgeTests
{
    [Theory]
    [InlineData("email")]
    [InlineData("sms")]
    public void OrderConfirmedMessage_SendsThroughConfiguredChannel(string channelName)
    {
        var channel = new RecordingChannel(channelName);
        var message = new OrderConfirmedMessage(channel);
        var customer = new Customer { Name = "Ava", Region = "AU" };

        message.Send(customer, "ord-1", 25m);

        Assert.Equal(channelName, channel.Name);
        Assert.Equal("Ava", channel.Recipient);
        Assert.Equal("order ord-1 confirmed, total A$25.00", channel.Message);
    }

    [Theory]
    [InlineData("email")]
    [InlineData("sms")]
    public void OrderShippedMessage_SendsThroughConfiguredChannel(string channelName)
    {
        var channel = new RecordingChannel(channelName);
        var message = new OrderShippedMessage(channel);
        var customer = new Customer { Name = "Ava" };

        message.Send(customer, "ord-1", "ship-1");

        Assert.Equal(channelName, channel.Name);
        Assert.Equal("Ava", channel.Recipient);
        Assert.Equal("order ord-1 shipped, tracking ship-1", channel.Message);
    }

    private sealed class RecordingChannel(string name) : IChannel
    {
        public string Name { get; } = name;

        public string Recipient { get; private set; } = "";

        public string Message { get; private set; } = "";

        public void Send(string recipient, string message)
        {
            Recipient = recipient;
            Message = message;
        }
    }
}
