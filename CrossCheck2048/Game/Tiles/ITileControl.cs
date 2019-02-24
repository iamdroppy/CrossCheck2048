namespace CrossCheck2048.Game.Tiles
{
    public interface ITileControl
    {
        int Score { get; }
        bool MoveTiles(DirectionEnum direction);
        void CreateTile();
        void Reset();
        bool IsGameFinished();
    }
}