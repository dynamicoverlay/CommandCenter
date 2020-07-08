namespace Shared.Events.Twitch.RequestsResponses
{
    public interface RequestChannelsToMonitor
    {
            
    }

    public interface ResponseChannelsToMonitor
    {
        string[] Channels { get; }
    }
}