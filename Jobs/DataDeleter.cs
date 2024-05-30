using Microsoft.EntityFrameworkCore;
using Quartz;
using TMAMS_Data_Transmitter.Data;

namespace TMAMS_Data_Transmitter.Jobs
{
    public class DataDeleter : IJob
    {
        private readonly ApplicationDbContext _dbContext;

        public DataDeleter(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Console.Out.WriteLineAsync("******************** DataDeleter called **************************");
            try
            {
                var records = await _dbContext.TestResults.Where(x=>x.IsSynced ==true).ToListAsync();
                if(records.Count() > 0)
                {
                    _dbContext.TestResults.RemoveRange(records);
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync("exception while calling the job");
            }

        }
    }
}
