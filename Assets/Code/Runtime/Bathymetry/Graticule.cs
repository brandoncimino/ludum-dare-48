using System;

using BrandonUtils.UI;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    /// <summary>
    /// I wound up using <see cref="BenthicProfile"/> instead, but this is kinda cute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Graticule<T> {
        private const float DefaultDiameter = 1;

        public readonly Vector2Int Resolution;
        public          Vector2    Dimensions = Vector2.one;

        private GZone<T>[,] _zones;
        public  GZone<T>[,] Zones => _zones ??= new GZone<T>[Resolution.x, Resolution.y];

        public delegate T Massager(GZone<T> gZone);

        public static class Technique {
            public static Func<GZone<float>, float> Sloper(float amplitude, RectTransform.Edge bottomEdge) {
                return (zone) => zone.Value + (zone.Coordinate[bottomEdge.Axis().CoordinateIndex()] * amplitude);
            }

            public static Func<GZone<T>, T> Setter(T newValue) {
                return (zone) => newValue;
            }
        }

        public Graticule(Vector2Int resolution, Vector2? dimensions = null) {
            this.Resolution = resolution;
            this.Dimensions = dimensions.GetValueOrDefault(Vector2.one * DefaultDiameter);
        }

        private static GZone<T>[,] GenerateZones(Vector2Int resolution) {
            var result = new GZone<T>[resolution.x, resolution.y];
            return result;
        }

        public static Graticule<T> Generate(Vector2Int resolution, Func<Vector2Int, T> generatorFunction) {
            var grat = new Graticule<T>(resolution);
            return null;
        }
    }
}
