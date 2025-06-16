using BusinessLayer;

namespace MVCEventManager.Models
{
    public class DateViewModel
    {
        public List<Event> Events { get; set; }
        public int Year { get; set; }
        public int MonthIndex { get; set; }
        public string Month { get; set; }
        public int Day { get; set; }
    }
}
