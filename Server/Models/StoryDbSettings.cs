namespace tic_tac_toe.backend.Models
{
    public class StoryDbSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string StoriesCollectionName { get; set; } = null!;
    }
}
