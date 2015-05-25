using System;

namespace ClothoidAndTheOthers.Geometry
{
    class GeometryException: ApplicationException
    {
        public GeometryException(string message): base (message)
        {
        }
    }

    internal class StraightsAreParallelException: GeometryException
    {
        public StraightsAreParallelException()
            : base("")
        {
        }
    }
}
