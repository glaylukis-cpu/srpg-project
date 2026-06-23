using UnityEngine;

namespace SRPG.Visual
{
    public static class BoardProjection
    {
        private const float HorizontalStep = 0.6f;
        private const float VerticalStep = 0.32f;
        private const float TileWidth = 1.2f;
        private const float TileHeight = 0.64f;
        private const int SortingStep = 10;
        private const int TileSortingBase = -600;
        private const int UnitSortingBase = -200;

        public static Vector3 GridToIsoWorld(Vector2Int coordinates, float cellSize)
        {
            var worldX = (coordinates.x - coordinates.y) * HorizontalStep * cellSize;
            var worldY = (coordinates.x + coordinates.y) * VerticalStep * cellSize;
            return new Vector3(worldX, worldY, 0f);
        }

        public static Vector3 GetIsoDirection(Vector2Int from, Vector2Int to)
        {
            var offset = GridToIsoWorld(to, 1f) - GridToIsoWorld(from, 1f);
            var length = System.Math.Sqrt(offset.x * offset.x + offset.y * offset.y);
            if (length <= 0.001)
            {
                return Vector3.zero;
            }

            return new Vector3((float)(offset.x / length), (float)(offset.y / length), 0f);
        }

        public static Vector3 GetUnitWorldPosition(Vector2Int coordinates, float cellSize)
        {
            return GridToIsoWorld(coordinates, cellSize) + new Vector3(0f, TileHeight * cellSize * 0.4f, -0.1f);
        }

        public static Vector3 GetTileScale(float cellSize)
        {
            return new Vector3(TileWidth * cellSize, TileHeight * cellSize * 2f, 1f);
        }

        public static Vector3 GetBoardCenter(int width, int height, float cellSize)
        {
            var maxX = width > 0 ? width - 1 : 0;
            var maxY = height > 0 ? height - 1 : 0;
            var centerX = (maxX - maxY) * HorizontalStep * cellSize * 0.5f;
            var centerY = (maxX + maxY) * VerticalStep * cellSize * 0.5f;
            return new Vector3(centerX, centerY, 0f);
        }

        public static float GetBoardWidth(int width, int height, float cellSize)
        {
            var span = (width > 0 ? width - 1 : 0) + (height > 0 ? height - 1 : 0);
            return span * HorizontalStep * cellSize + TileWidth * cellSize;
        }

        public static float GetBoardHeight(int width, int height, float cellSize)
        {
            var span = (width > 0 ? width - 1 : 0) + (height > 0 ? height - 1 : 0);
            return span * VerticalStep * cellSize + TileHeight * cellSize;
        }

        public static int GetTileSortingOrder(Vector2Int coordinates)
        {
            return TileSortingBase - (coordinates.x + coordinates.y) * SortingStep;
        }

        public static int GetUnitSortingOrder(Vector2Int coordinates)
        {
            return UnitSortingBase - (coordinates.x + coordinates.y) * SortingStep;
        }
    }
}
