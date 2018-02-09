using System;

namespace Clothoid.Geometry
{
    class GeometryException: ApplicationException
    {
        public GeometryException(string message): base (message)
        {
        }
    }
}
