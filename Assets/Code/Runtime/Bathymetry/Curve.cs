using System;
using System.Collections.Generic;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    public class Curve {
        public enum Form {
            Quadratic_Concave_Positive,
            Quadratic_Concave_Negative,
            Quadratic_Convex_Positive,
            Quadratic_Convex_Negative,
            Circular_Concave_Positive,
            Circular_Concave_Negative,
            Circular_Convex_Positive,
            Circular_Convex_Negative,
        }

        public static readonly Func<float, float> Quadratic_Concave_Positive = x => Mathf.Pow(x,     2);
        public static readonly Func<float, float> Quadratic_Concave_Negative = x => Mathf.Pow(x - 1, 2);
        public static readonly Func<float, float> Quadratic_Convex_Positive  = x => 1 - Mathf.Pow(x - 1, 2);
        public static readonly Func<float, float> Quadratic_Convex_Negative  = x => 1 - Mathf.Pow(x,     2);

        public static readonly Func<float, float> Circular_Concave_Positive = x => 1 - Mathf.Sqrt(1 - Mathf.Pow(x,     2));
        public static readonly Func<float, float> Circular_Concave_Negative = x => 1 - Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
        public static readonly Func<float, float> Circular_Convex_Positive  = x => Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
        public static readonly Func<float, float> Circular_Convex_Negative  = x => Mathf.Sqrt(1 - Mathf.Pow(x,     2));

        public static readonly Dictionary<Form, Func<float, float>> Curves = new Dictionary<Form, Func<float, float>>() {
            {Form.Quadratic_Concave_Positive, Quadratic_Convex_Positive},
            {Form.Quadratic_Concave_Negative, Quadratic_Concave_Negative},
            {Form.Quadratic_Convex_Positive, Quadratic_Concave_Positive},
            {Form.Quadratic_Convex_Negative, Quadratic_Convex_Negative},

            {Form.Circular_Concave_Positive, Circular_Concave_Positive},
            {Form.Circular_Concave_Negative, Circular_Concave_Negative},
            {Form.Circular_Convex_Positive, Circular_Convex_Positive},
            {Form.Circular_Convex_Negative, Circular_Convex_Negative}
        };
    }
}
