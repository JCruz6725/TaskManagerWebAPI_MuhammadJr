namespace Web.Api
{
    public class LicenseType
    {
        public Guid FreeId { get; set; }
        public string Free {  get; set; }
        public Guid PaidId { get; set; }
        public string Paid { get; set; }
    }
}
