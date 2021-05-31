using Code.Runtime.Bathymetry.Points;

using UnityEngine;

namespace Code.Runtime.Bathymetry.Measurements {
    public interface IPoint {
        float          Distance       { get; }
        float          Breadth        { get; }
        DimensionSpace DimensionSpace { get; }
        Vector3 ToWorldAxes();
    }

    public abstract class Spacey {
        public interface IWorldly {
            Vector3 Worldly { get; }
        }

        public interface ITerrene : IWorldly, IPoint {
            TerrainPoint Terrene { get; }
        }

        public interface IGeographic : ITerrene {
            GeographicPoint Geographic { get; }
        }

        public interface IBenthic : IGeographic {
            BenthicPoint Benthic { get; }
        }


        public interface IZonal : IBenthic {
            ZonePoint Zonal { get; }
        }
    }
}