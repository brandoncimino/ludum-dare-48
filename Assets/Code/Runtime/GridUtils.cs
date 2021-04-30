using System;

using UnityEngine;

namespace Code.Runtime {
    /// <summary>
    /// https://i.pinimg.com/736x/b3/4c/77/b34c77ffc95e0af61320ea0306d334d4--ocean-unit-oceans.jpg
    /// https://www.researchgate.net/figure/Formation-of-steady-state-raked-linear-dunes-from-a-local-sand-source-a-d-Elongating_fig1_313018248
    /// https://ascelibrary.org/doi/abs/10.1061/%28ASCE%29SU.1943-5428.0000223
    /// https://en.wikipedia.org/wiki/Devil%27s_stovepipe
    /// https://io9.gizmodo.com/explore-the-worlds-most-detailed-map-of-the-seafloor-r-1642315933
    /// https://data.noaa.gov/onestop/collections?q=coasts
    /// </summary>
    public static class GridUtils {
        public delegate TResult CellProcessor<in TInput, out TResult>(Vector2Int coordinate, TInput cell);

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
        /// The <paramref name="cellProcessor"/> is a function that processes a 2d coordinate and the current cell contents
        /// </summary>
        /// <param name="originalGrid"></param>
        /// <param name="cellProcessor"></param>
        /// <typeparam name="TOld"></typeparam>
        /// <typeparam name="TNew"></typeparam>
        /// <returns></returns>
        public static TNew[,] ProcessGrid<TOld, TNew>(this TOld[,] originalGrid, CellProcessor<TOld, TNew> cellProcessor) {
            var newGrid = new TNew[originalGrid.GetLength(0), originalGrid.GetLength(1)];
            for (var x = 0; x < originalGrid.GetLength(0); x++) {
                for (var y = 0; y < originalGrid.GetLength(0); y++) {
                    newGrid[x, y] = cellProcessor.Invoke(new Vector2Int(x, y), originalGrid[x, y]);
                }
            }

            return newGrid;
        }
    }
}
