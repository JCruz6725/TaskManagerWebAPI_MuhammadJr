using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ModelLibrary;
using Web.Api.Persistence;
using Web.Api.scaffolding_temp_folder;
using Web.Api.Util;

namespace Web.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly StatusChange statusChange;
        private readonly PurposeTypeOptions purposeTypeOptions;
        private readonly LicenseTypeOptions licenseTypeOptions;
        private readonly TaskManagerAppDBContext context;
        private readonly ILogger<AdminController> logger;
        const int DEFAULT_PRIORITY = 5;


        public AdminController(IOptions<StatusChange> statusChangeOptions, IOptions<PurposeTypeOptions> purposeTypeOptions, IOptions<LicenseTypeOptions> licenseTypeOptions, TaskManagerAppDBContext context, ILogger<AdminController> logger)
        {
            statusChange = statusChangeOptions.Value;
            this.purposeTypeOptions = purposeTypeOptions.Value;
            this.licenseTypeOptions = licenseTypeOptions.Value;
            this.context = context;
            this.logger = logger;
        }

        
        [HttpPost("/AddStatus", Name = "AddStatus")]
        public async Task<ActionResult<string>> AddStatus()
        {
            try
            {
                using (logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    Status pendingStatus = new() { Id = statusChange.PendingId, Name = statusChange.Pending, Code = statusChange.Code1 };
                    Status completedStatus = new() { Id = statusChange.CompleteId, Name = statusChange.Complete, Code = statusChange.Code2 };

                    context.Add(pendingStatus);
                    context.Add(completedStatus);

                    await context.SaveChangesAsync();
                    logger.LogInformation("Pending and completed status saved to database");
                    return Ok(new {res = "Status Added"});
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Adding status process failed: {ex.Message}");
                return StatusCode(500);
            }
        }
        

        [HttpPost("/Refresh", Name = "Refresh")]
        public async Task<ActionResult<string>> AddDummyData() {
            try
            {
                using (logger.BeginScope(new Dictionary<string, object> { ["TransactionId"] = HttpContext.TraceIdentifier, }))
                {
                    CancellationTokenSource source = new CancellationTokenSource();
                    CancellationToken token = source.Token;
                    DateTimeFaker dateTimeFaker = new DateTimeFaker();

                    context.Database.ExecuteSqlRaw("""
                        delete from SubTasks
                        delete from TaskItemStatusHistory
                        delete from TaskWithinList
                        delete from TaskItemNotes
                        delete from Lists
                        delete from TaskItems
                        delete from Statuses
                        delete from DeviceData
                        delete from Address
                        delete from Password
                        delete from Profile
                        delete from PurposeTypes
                        delete from LicenseTypes
                        delete from users
                        """);
                    logger.LogInformation("Successfully removed all previous data in database");

                    Status pendingStatus = new() { Id = statusChange.PendingId, Name = statusChange.Pending, Code = statusChange.Code1 };
                    Status completedStatus = new() { Id = statusChange.CompleteId, Name = statusChange.Complete, Code = statusChange.Code2 };

                    context.Add(pendingStatus);
                    context.Add(completedStatus);
                    logger.LogInformation("Successfully added pending and completed status'");

                    PurposeType educationPurpose = new() { Id = purposeTypeOptions.EducationId, PurposeTitle = purposeTypeOptions.Education };
                    PurposeType workPurpose = new() {  Id = purposeTypeOptions.WorkId, PurposeTitle = purposeTypeOptions.Work };
                    PurposeType personalPurpose = new() { Id = purposeTypeOptions.PersonalId, PurposeTitle= purposeTypeOptions.Personal };

                    context.Add(educationPurpose);
                    context.Add(workPurpose);
                    context.Add(personalPurpose);
                    logger.LogInformation("Successfully added education, work, and personal purpose types");

                    LicenseType freeLicense = new() { Id = licenseTypeOptions.FreeId, LicenseTitle = licenseTypeOptions.Free };
                    LicenseType paidLicense = new() { Id = licenseTypeOptions.PaidId, LicenseTitle = licenseTypeOptions.Paid };

                    context.Add(freeLicense);
                    context.Add(paidLicense);
                    logger.LogInformation("Successfully added free and paid licensing types");


                    UserDirector userDirector = new UserDirector(statusChange, purposeTypeOptions, licenseTypeOptions, dateTimeFaker);
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
                    return Ok(new {res = "Dummy Data Refreshed"});
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Refresh process failed: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
