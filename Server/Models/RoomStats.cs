namespace tic_tac_toe.backend.Models
{
    public class RoomStats
    {
        public int WinX { get; set; } = 0;
        public int WinO { get; set; } = 0;
        public int Draw { get; set; } = 0;

        public void UpdateStats(Outcome outcome)
        {
            switch (outcome)
            {
                case Outcome.WinX:
                    WinX++;
                    break;
                case Outcome.WinO:
                    WinO++;
                    break;
                case Outcome.Draw:
                    Draw++; 
                    break;
            }
        }
    }
}
