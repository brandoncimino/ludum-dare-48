namespace Code.Runtime {
    public static class TrackerUtils {
        public static int Delta(this Tracker<int> tracker) {
            return tracker.Current - tracker.Previous;
        }

        public static double Delta(this Tracker<double> tracker) {
            return tracker.Current - tracker.Previous;
        }

        public static float Delta(this Tracker<float> tracker) {
            return tracker.Current - tracker.Previous;
        }

        public static float Delta(this Tracker<long> tracker) {
            return tracker.Current - tracker.Previous;
        }
    }
}