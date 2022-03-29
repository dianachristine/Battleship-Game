namespace BattleShipBrain
{
    public struct BoardSquareState
    {
        public bool IsShip { get; set; }
        public bool IsBomb { get; set; }

        public override string ToString()
        {
            switch (IsEmpty: IsShip, IsBomb)
            {
                case (false, false):
                    return " ";
                case (false, true):
                    return "*";
                case (true, false):
                    return "O";
                case (true, true):
                    return "X";
            }
        }
    }
    
    
}