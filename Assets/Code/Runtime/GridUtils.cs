using System;

using UnityEngine;

namespace Code.Runtime {
    public static class GridUtils {
        public static T[,] GenerateGrid<T>(Vector2 dimensions, Func<Vector2, T> coordinateProcessor) {
            var result = new T[(int) dimensions.x, (int) dimensions.y];

            for (var x = 0; x < dimensions.x; x++) {
                for (var y = 0; y < dimensions.y; y++) {
                    result[x, y] = coordinateProcessor.Invoke(new Vector2(x, y));
                }
            }

            return result;
        }

        /// <summary>
        /// The <paramref name="cellProcessor"/> is a function that process a 2d coordinate and the current cell contents
        /// </summary>
        /// <param name="originalGrid"></param>
        /// <param name="cellProcessor"></param>
        /// <typeparam name="TOld"></typeparam>
        /// <typeparam name="TNew"></typeparam>
        /// <returns></returns>
        public static TNew[,] ProcessGrid<TOld, TNew>(this TOld[,] originalGrid, Func<Vector2, TOld, TNew> cellProcessor) {
            var newGrid = new TNew[originalGrid.GetLength(0), originalGrid.GetLength(1)];
            for (var x = 0; x < originalGrid.GetLength(0); x++) {
                for (var y = 0; y < originalGrid.GetLength(0); y++) {
                    newGrid[x, y] = cellProcessor.Invoke(new Vector2(x, y), originalGrid[x, y]);
                }
            }

            return newGrid;
        }
    }
}
