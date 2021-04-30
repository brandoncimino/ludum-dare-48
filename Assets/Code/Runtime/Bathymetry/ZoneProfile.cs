using System;
using System.Collections.Generic;

namespace Code.Runtime.Bathymetry {
    [Serializable]
    public class ZoneProfile : IComparable<ZoneProfile> {
        public  float              Amplitude;
        public  float              GeographicDistance;
        public  ZoneType           ZoneType;
        private Func<float, float> Equation;
        private Curve.Form         CurveForm;

        public ZoneProfile(Func<float, float> equation) {
            this.Equation = equation;
        }

        public ZoneProfile(Curve.Form curveForm) {
            this.CurveForm = curveForm;
            this.Equation  = Curve.Curves[curveForm];
        }

        public int CompareTo(ZoneProfile other) {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            return ReferenceEquals(null, other) ? 1 : ZoneType.CompareTo(other.ZoneType);
        }

        public void SetEquation(Func<float, float> equation) {
            this.Equation = equation;
        }

        public float Invoke(float pointInZone) {
            float result = Equation.Invoke(pointInZone);
            return result;
        }

        public float Invoke(float pointInZone, List<string> log) {
            var result = Invoke(pointInZone);
            log.Add($"{ZoneType} ({CurveForm}): f({pointInZone}) -> {result}");
            return result;
        }

        public override string ToString() {
            return $"{ZoneType}, [Geo: {GeographicDistance}] x [Amp: {Amplitude}], {CurveForm}";
        }
    }
}
