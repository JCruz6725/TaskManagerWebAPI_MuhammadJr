using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Web.Api.Persistence;
using Web.Api.Persistence.Models;
using Web.Api.Util;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly StatusChange statusChange;
        private readonly TaskManagerAppDBContext context;
        const int DEFAULT_PRIORITY = 5;


        public AdminController(IOptions<StatusChange> statusChangeOptions, TaskManagerAppDBContext context)
        {
            statusChange = statusChangeOptions.Value;
            this.context = context;
        }

        
        [HttpPost("/AddStatus", Name = "AddStatus")]
        public async Task<ActionResult<string>> AddStatus()
        {
            Status pendingStatus = new() { Id = statusChange.PendingId, Name = statusChange.Pending, Code = statusChange.Code1 };
            Status completedStatus = new() { Id = statusChange.CompleteId, Name = statusChange.Complete, Code = statusChange.Code2 };

            context.Add(pendingStatus);
            context.Add(completedStatus);

            await context.SaveChangesAsync();
            return Ok("Status Added");
        }
        

        [HttpPost("/AddDummyData", Name = "AddDummyData")]
        public async Task<ActionResult<string>> AddDummyData() {
            UserDirector userDirector = new UserDirector(statusChange);
            context.AddRange([
                userDirector.MakeChuckFinleyProfile(),
                userDirector.MakeJessieHopkinsProfile(),
                userDirector.MakeAlexFarmerProfile(),
                userDirector.MakeAprilRiceProfile(),
                userDirector.MakeNikoLogamProfile(),
            ]);

            await context.SaveChangesAsync();
            return Ok("Dummy Data Added");
        }
    }
}
