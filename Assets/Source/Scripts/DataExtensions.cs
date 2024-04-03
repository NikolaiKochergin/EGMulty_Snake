using UnityEngine;

namespace Source.Scripts
{
    public static class DataExtensions
    {
        public static DetailPositions AsDetailPositionsData(this Vector3[] detailPositions)
        {
            DetailPositions positions = new DetailPositions()
            {
                ds = new Vector2Data[detailPositions.Length]
            };

            for (int i = 0; i < detailPositions.Length; i++)
            {
                positions.ds[i] = new Vector2Data()
                {
                    x = detailPositions[i].x,
                    z = detailPositions[i].z,
                };
            }

            return positions;
        }
    }
}