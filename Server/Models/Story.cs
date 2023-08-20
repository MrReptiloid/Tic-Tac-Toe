using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace tic_tac_toe.backend.Models
{
    public class Story
    {
        public Story()
        {
            StartTime = DateTime.Now;
            _steps = new List<Step>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } 
        public int StoryId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        [BsonElement("Winner")]
        private Outcome _winner { get; set; }
        [BsonElement("Steps")]
        private List<Step> _steps { get; set; }

        public void AddSteps(List<Step> steps)
        {
            _steps = steps;
        }

        public void SetWinner(Outcome winner)
        {
            _winner = winner;
        }

        public List<Step> GetSteps()
        {
            return _steps;
        }

        public Outcome GetWinner()
        {
            return _winner;
        }

        public static List<StoryDTO> ToSerializabledStories(List<Story> stories)
        {
            List<StoryDTO> serializabledStories = new List<StoryDTO>();

            foreach (var story in stories) 
                serializabledStories.Add(new StoryDTO(story));

            return serializabledStories;
        }
    }

    public class StoryDTO
    {
        public StoryDTO(Story story)
        {
            Id = story.Id;
            StoryId = story.StoryId;
            RoomId = story.RoomId;
            StartTime = story.StartTime;
            EndTime = story.EndTime;
            Winner = story.GetWinner();
            Steps = story.GetSteps();
        }

        public string? Id { get; set; }
        public int StoryId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Outcome Winner { get; set; }
        public List<Step> Steps { get; set; }
    }
}
