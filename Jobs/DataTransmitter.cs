using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Xml;
using TMAMS_Data_Transmitter.Data;
using TMAMS_Data_Transmitter.Services;

namespace TMAMS_Data_Transmitter.Jobs
{
    public class DataTransmitter : IJob
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly MqttClientService _mqttService;

        public DataTransmitter(ApplicationDbContext dbContext, MqttClientService mqttService)
        {
            _dbContext = dbContext;
            _mqttService = mqttService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("************  DataTransmitter called ***************88");
            try
            {
                var allRecords = await _dbContext.TestResults.Where(x => x.IsSynced == false).ToListAsync();
                if (allRecords.Count() > 0)
                {
                    foreach (var record in allRecords)
                    {
                        if (record == null)
                            continue;
                        var data =await _mqttService.PublishAsync("MQTT_C_B_Demo", record.Data);
                        if (data)
                        {
                           record.IsSynced=true;
                           record.UpdatedAt= DateTime.UtcNow;
                          _dbContext.Update(record);
                           await _dbContext.SaveChangesAsync();
                           await Console.Out.WriteLineAsync("data is sent to cm_mqtt_broker");
                        }
                        else
                        {
                            await Console.Out.WriteLineAsync("CM MQTT Broker Connection Not happening");
                        }
                    }
                }
                else
                {
                    await Console.Out.WriteLineAsync("No stream of records in the database.. its checking for records");
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("exception while calling the job");
            }
        }
    }
}
