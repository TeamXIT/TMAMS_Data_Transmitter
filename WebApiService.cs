namespace TMAMS_Data_Transmitter
{
    public class WebApiService : BackgroundService
    {
        private readonly IHost _host;

        public WebApiService(IHost host)
        {
            _host = host;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _host.RunAsync(stoppingToken);
        }
    }
}
