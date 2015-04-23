using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKLib
{
    /// <summary>
    /// Compare vertices based on their indices in the model
    /// </summary>
    public class VertexComparerIndexInModel : IComparer<Vertex>
    {
        public static readonly VertexComparerIndexInModel IndexInModel = new VertexComparerIndexInModel();

        public int Compare(Vertex x, Vertex y)
        {
            return x.IndexInModel.CompareTo(y.IndexInModel);
        }
    }
    
}
