
using System.Windows;
using MIConvexHull;
using System.Windows.Media;

namespace OpenTKLib
{

    /// <summary>
    /// 
    /// </summary>
    public class Vertex2D : IVertexPosition
    {
        public double[] Position { get; set; }
        public int IndexInModel;
        public Vertex2D(double x, double y)
        {
            Position = new double[] { x, y };
        }
        public Vertex2D(double x, double y, int indexInModel)
        {
            Position = new double[] { x, y };
            IndexInModel = indexInModel;
        }
    
        
    }
}
