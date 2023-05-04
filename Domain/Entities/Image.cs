namespace Domain.Entities
{
    public class Image
    {
        public int ImageId { get; set; }
        public Guid UserId { get; set; }
        public string Url { get; set; }
        public int Order { get; set; }
        public UserProfile User { get; set; }
    }
}
