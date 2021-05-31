using Code.Runtime.Bathymetry.Points;

using UnityEngine;

namespace Code.Runtime.Bathymetry.Measurements {
    public interface IPoint {
        float          Distance       { get; set; }
        float          Breadth        { get; set; }
        DimensionSpace DimensionSpace { get; }
        Vector3 ToWorldAxes();
    }

    public abstract class Spacey {
        public interface IWorldly {
            Vector3 ToWorldly();
        }

        public interface ITerrene : IWorldly, IPoint {
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
