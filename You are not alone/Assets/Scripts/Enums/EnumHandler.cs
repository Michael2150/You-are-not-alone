namespace Enums
{
    public class EnumHelpers
    {
        public static bool CellIsOccupied(CellState state)
        {
            return state is CellState.Active 
                        or CellState.Hallway 
                        or CellState.Room;
        }


        public static bool CellsAreAllowed(CellState allowed_val, CellState received_val)
        {
            switch (allowed_val)
            {
                case CellState.Active:
                    return received_val is CellState.Active 
                        or CellState.Hallway 
                        or CellState.Room;
                case CellState.Any:
                    return true;
                default:
                    return allowed_val == received_val;
            }
        }
    }
    
    public enum CellState
    {
        None = 0,
        Active = 1,
        Hallway = 2,
        Room = 3,
        Any = 4,
        OutOfBounds = 5
    }
}