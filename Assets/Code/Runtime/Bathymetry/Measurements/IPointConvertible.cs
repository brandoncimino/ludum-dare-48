using Code.Runtime.Bathymetry.Points;

using UnityEngine;

namespace Code.Runtime.Bathymetry.Measurements {
    public abstract class Spacey {
        public interface IWorldly {
            Vector3 ToWorldly();
        }

        public interface ITerrene : IWorldly {
            TerrainPoint ToTerrene();
        }

        public interface IGeographic : ITerrene {
            IDGeographicPoint ToGeographic();
        }

        public interface IBenthic : IGeographic {
            BenthicPoint ToBenthic();
        }


        public interface IZonal : IBenthic {
            ZonePoint ToZonal();
        }
    }
}
