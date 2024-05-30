namespace TMAMS_Data_Transmitter.Requests
{
    public class MqttSettings
    {
        public string ClientId { get; set; }
        public string Broker { get; set; }
        public int Port { get; set; }
    }
}
