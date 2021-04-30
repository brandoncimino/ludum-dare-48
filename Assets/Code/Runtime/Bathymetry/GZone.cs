using UnityEngine;

namespace Code.Runtime {
    /// <summary>
    /// Not to be confused with <a href="https://en.wikipedia.org/wiki/G-spot">GSpot</a>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GZone<T> {
        public Vector2Int Coordinate;
        public T          Value;
    }
}