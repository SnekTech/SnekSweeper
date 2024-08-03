using System;

namespace SnekSweeper.GridSystem;

public partial class Grid
{
    public static class EventBus
    {
        #region input events

        public static event Action<(int i, int j)>? CellPrimaryReleasedAt;
        public static event Action<(int i, int j)>? CellPrimaryDoubleClickedAt;
        public static event Action<(int i, int j)>? CellSecondaryReleased;

        #endregion

        #region invoke methods

        public static void InvokeCellPrimaryReleasedAt((int i, int j) gridIndex) =>
            CellPrimaryReleasedAt?.Invoke(gridIndex);

        public static void InvokeCellPrimaryDoubleClickedAt((int i, int j) gridIndex) =>
            CellPrimaryDoubleClickedAt?.Invoke(gridIndex);

        public static void InvokeCellSecondaryReleasedAt((int i, int j) gridIndex) =>
            CellSecondaryReleased?.Invoke(gridIndex);

        #endregion
    }
}