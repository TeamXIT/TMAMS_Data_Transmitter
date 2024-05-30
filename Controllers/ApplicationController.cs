using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TMAMS_Data_Transmitter.Data;
using TMAMS_Data_Transmitter.Requests;

namespace TMAMS_Data_Transmitter.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        public ApplicationController(ApplicationDbContext context) 
        {
            _context=context;
        }

        [HttpPost("create-stream")]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStreamRequest request)
        {
            var response = new BaseResponse();
            try
            {
                await _context.TestResults.AddAsync(new Models.TestResult() { 
                     CreatedAt = DateTime.UtcNow,
                     IsSynced = false,
                     Data=request.InputStream,
                });
                response.Status = true;
                response.StatusMessage = "Success";
                await _context.SaveChangesAsync();
                return StatusCode(StatusCodes.Status200OK);
            }
            catch (Exception ex)
            {
                response.StatusMessage = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
        [HttpPost("get-all")]
        public async Task<IActionResult> GetAllAsync()
        {
            var response = new BaseResponse();
            try
            {
                var results=await _context.TestResults.ToListAsync();
                response.Status = true;
                response.StatusMessage = "Success";
                response.Data = results;
                return StatusCode(StatusCodes.Status200OK, response);
            }
            catch (Exception ex)
            {
                response.StatusMessage = ex.Message;
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
        }
    }
}
