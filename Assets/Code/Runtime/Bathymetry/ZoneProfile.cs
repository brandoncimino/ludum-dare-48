using System;
using System.Collections.Generic;

namespace Code.Runtime.Bathymetry {
    [Serializable]
    public class ZoneProfile : IComparable<ZoneProfile> {
        public  ZoneType           ZoneType;
        public  Curve.Form         CurveForm;
        private Func<float, float> Equation => Curve.Curves[CurveForm];
        public  float              GeographicDistance;
        public  float              Amplitude;

        public ZoneProfile(Curve.Form curveForm) {
            this.CurveForm = curveForm;
        }

        public int CompareTo(ZoneProfile other) {
            if (ReferenceEquals(this, other)) {
                return 0;
            }

            return ReferenceEquals(null, other) ? 1 : ZoneType.CompareTo(other.ZoneType);
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
