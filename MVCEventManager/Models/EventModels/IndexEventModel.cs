using BusinessLayer;

namespace MVCEventManager.Models.EventModels
{
    public class IndexEventModel
    {
        public List<Event> Events { get; set; }
        public string SortOrder { get; set; }
        public string SearchQuery { get; set; }
    }
}
