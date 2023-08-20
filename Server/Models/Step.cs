namespace tic_tac_toe.backend.Models
{
    public class Step
    {
        public Step(int idx, char symbol)
        {
            Idx = idx;
            Symbol = symbol;
        }

        public int Idx { get; set; }
        public char Symbol { get; set; }
    }
}
