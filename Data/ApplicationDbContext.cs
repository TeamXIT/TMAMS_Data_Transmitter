using Microsoft.EntityFrameworkCore;
using System.Xml;
using TMAMS_Data_Transmitter.Models;

namespace TMAMS_Data_Transmitter.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<TestResult> TestResults { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }

}
