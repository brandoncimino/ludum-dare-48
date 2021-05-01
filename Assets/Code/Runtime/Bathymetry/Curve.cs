using System;
using System.Collections.Generic;

using UnityEngine;

namespace Code.Runtime.Bathymetry {
    /// <summary>
    /// Contains <see cref="Func{T,TResult}"/>s that produce curves with simple shapes from x=0 -> x=1, i.e. they:
    /// <ul>
    /// <li>Start at (0,0) or (0,1)</li>
    /// <li>End at (1,0) or (1,1)</li>
    /// </ul>
    ///
    /// <b>NOTE:</b>
    /// If you are reading this Nicole, feel free to replace my laymath with realmath!
    /// </summary>
    ///
    public class Curve {
        public enum Form {
            Quadratic_Dippy_Positive,
            Quadratic_Dippy_Negative,
            Quadratic_Bumpy_Positive,
            Quadratic_Bumpy_Negative,
            Circular_Dippy_Positive,
            Circular_Dippy_Negative,
            Circular_Bumpy_Positive,
            Circular_Bumpy_Negative,
        }

        public static readonly Func<float, float> Quadratic_Dippy_Positive = x => Mathf.Pow(x,     2);
        public static readonly Func<float, float> Quadratic_Dippy_Negative = x => Mathf.Pow(x - 1, 2);
        public static readonly Func<float, float> Quadratic_Bumpy_Positive = x => 1 - Mathf.Pow(x - 1, 2);
        public static readonly Func<float, float> Quadratic_Bumpy_Negative = x => 1 - Mathf.Pow(x,     2);

        public static readonly Func<float, float> Circular_Dippy_Positive = x => 1 - Mathf.Sqrt(1 - Mathf.Pow(x,     2));
        public static readonly Func<float, float> Circular_Dippy_Negative = x => 1 - Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
        public static readonly Func<float, float> Circular_Bumpy_Positive = x => Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));
        public static readonly Func<float, float> Circular_Bumpy_Negative = x => Mathf.Sqrt(1 - Mathf.Pow(x,     2));

        public static readonly Dictionary<Form, Func<float, float>> Curves = new Dictionary<Form, Func<float, float>>() {
            {Form.Quadratic_Dippy_Positive, Quadratic_Dippy_Positive},
            {Form.Quadratic_Dippy_Negative, Quadratic_Dippy_Negative},
            {Form.Quadratic_Bumpy_Positive, Quadratic_Bumpy_Positive},
            {Form.Quadratic_Bumpy_Negative, Quadratic_Bumpy_Negative},

            {Form.Circular_Dippy_Positive, Circular_Dippy_Positive},
            {Form.Circular_Dippy_Negative, Circular_Dippy_Negative},
            {Form.Circular_Bumpy_Positive, Circular_Bumpy_Positive},
            {Form.Circular_Bumpy_Negative, Circular_Bumpy_Negative}
        };
    }
}
