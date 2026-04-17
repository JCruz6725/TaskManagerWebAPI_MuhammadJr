namespace Web.Api.Util
{
    public class DateTimeFaker
    {
        public int incrementor { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now.AddYears(-1);

        public DateTime GetDateAndAdvance()
        {
            return StartDate.AddDays(incrementor++);
        }
    }
}
