using UnityEngine;

namespace Enums
{
    public static class EnumHelpers
    {
        public static bool CellIsOccupied(CellState state)
        {
            return CellsAreAllowed(CellState.Active, state);
        }


        public static bool CellsAreAllowed(CellState allowedVal, CellState receivedVal)
        {
            switch (allowedVal)
            {
                case CellState.Active:
                    return receivedVal is CellState.Active 
                        or CellState.Hallway 
                        or CellState.Room;
                case CellState.Any:
                    return true;
                default:
                    return allowedVal == receivedVal;
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

    public enum PlayerPrefKeys
    {
        HIGH_SCORE = 0,
        HIGH_SCORE_NAME = 1,
        MUSIC_VOLUME = 2,
        SFX_VOLUME = 3,
        DIFFICULTY = 4,
    }
}