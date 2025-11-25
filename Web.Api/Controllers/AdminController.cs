using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly ILogger<AdminController> logger;
        const int DEFAULT_PRIORITY = 5;


        public AdminController(IOptions<StatusChange> statusChangeOptions, TaskManagerAppDBContext context, ILogger<AdminController> logger)
        {
            statusChange = statusChangeOptions.Value;
            this.context = context;
            this.logger = logger;
        }

        
        [HttpPost("/AddStatus", Name = "AddStatus")]
        public async Task<ActionResult<string>> AddStatus()
        {
            using (logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
            {
                Status pendingStatus = new() { Id = statusChange.PendingId, Name = statusChange.Pending, Code = statusChange.Code1 };
                Status completedStatus = new() { Id = statusChange.CompleteId, Name = statusChange.Complete, Code = statusChange.Code2 };

                context.Add(pendingStatus);
                context.Add(completedStatus);

                await context.SaveChangesAsync();
                logger.LogInformation("Pending and completed status saved to database");
                return Ok("Status' Added");
            }
        }
        

        [HttpPost("/Refresh", Name = "Refresh")]
        public async Task<ActionResult<string>> AddDummyData() {

            using (logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken token = source.Token;

                context.Database.ExecuteSqlRaw("""
                    delete from SubTasks
                    delete from TaskItemStatusHistory
                    delete from TaskWithinList
                    delete from TaskItemNotes
                    delete from Lists
                    delete from TaskItems
                    delete from Statuses
                    delete from users
                    """);
                logger.LogInformation("Successfully removed all previous data in database");

                Status pendingStatus = new() { Id = statusChange.PendingId, Name = statusChange.Pending, Code = statusChange.Code1 };
                Status completedStatus = new() { Id = statusChange.CompleteId, Name = statusChange.Complete, Code = statusChange.Code2 };

                context.Add(pendingStatus);
                context.Add(completedStatus);
                logger.LogInformation("Successfully added pending and completed status'");


                UserDirector userDirector = new UserDirector(statusChange);
                context.AddRange([
                    userDirector.MakeAlexFarmerProfile(),
                    userDirector.MakeJessieHopkinsProfile(),
                    userDirector.MakeAprilRiceProfile(),
                    userDirector.MakeNikoLoganProfile(),
                    userDirector.MakeChuckFinleyProfile(),
                    userDirector.MakeIrenePetersonProfile(),
                ]);
                logger.LogInformation("Successfully created all dummy data");

                await context.SaveChangesAsync(token);
                logger.LogInformation("Successfully saved all changes to database");
                return Ok("Dummy Data Added");
            }
        }
    }
}
