namespace MVCEventManager.Models.OtherModels
{
    public class MessageModel
    {
        public string Message { get; set; }
        public DateTime Expires { get; set; }

        public MessageModel()
        {
            Expires = DateTime.Now.AddSeconds(20);
        }
    }
}
