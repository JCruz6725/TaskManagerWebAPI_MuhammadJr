namespace Web.Api
{
    public class StatusChange
    {
        public Guid CompleteId { get; set; } 
        public Guid PendingId { get; set; }
        public string Complete { get; set; } 
        public int Code2 { get; set; }
        public string Pending { get; set; }
        public int Code1 { get; set; }   
    }
}
