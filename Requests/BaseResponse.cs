namespace TMAMS_Data_Transmitter.Requests
{
    public class BaseResponse
    {
        public bool Status {  get; set; }

        public string StatusMessage { get; set; }

        public dynamic Data { get; set; }
    }
}
