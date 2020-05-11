namespace KemiNETShared.Requests
{
    public class PingRequest
    {
        public string Type { get; set; }
        
        public string MessageString { get; set; }

        public PingRequest(string messageType, string messageString)
        {
            Type = messageType;
            MessageString = messageString;
        }
    }
}