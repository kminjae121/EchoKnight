using UnityEngine;

namespace Code.Utils
{
    public static class DistanceUtils
    {
        public static float GetEuclideanDistance(Vector2Int start, Vector2Int destination)
            => Vector2Int.Distance(start, destination);

        public static float GetEuclideanDistance(Vector3Int start, Vector3Int destination)
            => Vector3Int.Distance(start, destination);
    }
}
