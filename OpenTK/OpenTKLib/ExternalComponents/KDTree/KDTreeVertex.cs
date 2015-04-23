using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Globalization;
using System.Windows.Media;
using System.Diagnostics;
using OpenTK;


namespace OpenTKLib
{
  
   
    public partial class KDTreeVertex 
    {
        public static KDTreeMode KDTreeMode = KDTreeMode.Rednaxala;
        public KDTreeRednaxela.KDTree_Rednaxela<EllipseWrapper> KdTree_Rednaxela;
        public KDTree_Stark KdTree_Stark;
        
        

        public int NumberOfNeighboursToSearch = 5;
        private static KDTreeVertex instance;

        public KDTreeVertex()
        {
            NumberOfNeighboursToSearch = 5;
            KDTreeMode = KDTreeMode.Rednaxala;
            instance = this;

        }
        public static KDTreeVertex Instance
        {
            get
            {
                if (instance == null)
                    instance = new KDTreeVertex();
                return instance;

            }

        }
        public bool BuildKDTree_Rednaxela(List<Vertex> vTarget)
        {
            TimeCalc.ResetTime();

            try
            {
                KdTree_Rednaxela = new KDTreeRednaxela.KDTree_Rednaxela<EllipseWrapper>(3);

                for (int i = 0; i < vTarget.Count; ++i)
                {

                    Vertex p = vTarget[i];
                    KdTree_Rednaxela.AddPoint(new double[] { p.Vector.X, p.Vector.Y, p.Vector.Z }, new EllipseWrapper(p));

                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error building kd-tree " + err.Message);
                return false;

            }

            TimeCalc.ShowLastTimeSpan("Build Tree Rednaxala");

            return true;

        }
        public bool BuildKDTree_Stark(List<Vertex> target)
        {
            TimeCalc.ResetTime();
            KdTree_Stark = KDTree_Stark.Build(target);
            TimeCalc.ShowLastTimeSpan("Build Tree Stark");

            return true;

        }
        public List<Vertex> FindNearest_Rednaxela(List<Vertex> pointsSource, List<Vertex> pointsTarget, int keepOnlyNearestPoints)
        {
            List<Vertex> nearestNeighbours = new List<Vertex>();
            int iMax = 1;
            List<double> listDistances = new List<double>();
            //float fThreshold = kdTreeNeighbourThreshold;
            for (int i = 0; i < pointsSource.Count; i++)
            {
                Vertex p = pointsSource[i];
                // Perform a nearest neighbour search around that point.
                KDTreeRednaxela.NearestNeighbour<EllipseWrapper> pIter = null;
                pIter = KdTree_Rednaxela.FindNearest_EuclidDistance(new double[] { p.Vector.X, p.Vector.Y, p.Vector.Z }, iMax, -1);
                while (pIter.MoveNext())
                {
                    EllipseWrapper wr = pIter.Current;
                    listDistances.Add(pIter.CurrentDistance);
                    nearestNeighbours.Add(wr.Vertex);
                    break;
                }

            }

            if (keepOnlyNearestPoints > 0)
                RemovePointsWithDistanceGreaterThanAverage(listDistances, pointsSource, nearestNeighbours);

            

            return nearestNeighbours;
        }
        private void RemovePointsWithDistanceGreaterThanAverage(List<double> listDistances, List<Vertex> pointsSource, List<Vertex> pointsTarget)
        {
            double median = GetAverage(listDistances);

            for (int i = listDistances.Count - 1; i >= 0; i--)
            {
                if (listDistances[i] > median)
                {
                    pointsSource.RemoveAt(i);
                    pointsTarget.RemoveAt(i);

                }

            }
        }
        private void RemovePointsWithDistanceGreaterThanMedian(List<double> listDistances, List<Vertex> pointsSource, List<Vertex> pointsTarget)
        {
            double median = GetMedian(listDistances);

            for (int i = listDistances.Count -1 ; i >=0 ; i--)
            {
                if (listDistances[i] > median)
                {
                    pointsSource.RemoveAt(i);
                    pointsTarget.RemoveAt(i);

                }

            }
        }
        public double GetAverage(List<double> source)
        {
            double[] temp1 = source.ToArray();
            Array.Sort(temp1);

            return temp1[temp1.Length / 2];

            //double average = 0;
            //for(int i = 0; i < source.Count/2; i++)
            //{
            //    average += source[i];
            //}
            //average /= source.Count;
            //return average;

        }
        public double GetMedian(List<double> source)
        {
            // Create a copy of the input, and sort the copy
            double[] temp1 = source.ToArray();
            double[] temp  = new double[temp1.Length];
            temp1.CopyTo(temp, 0);

            Array.Sort(temp);


            int count = temp.Length;
            if (count == 0)
            {
                throw new InvalidOperationException("Empty collection");
            }
            else if (count % 2 == 0)
            {
                // count is even, average two middle elements
                double a = temp[count / 2 - 1];
                double b = temp[count / 2];
                return (a + b) / 2;
            }
            else
            {
                // count is odd, return the middle element
                return temp[Convert.ToInt32(count * 0.8)] ;
            }
        }
        public List<Vertex> FindNearest_Stark(List<Vertex> source, List<Vertex> target)
        {
            KdTree_Stark.ResetSearch();
            List<Vertex> result = new List<Vertex>();
            
            List<int> indicesTargetFound = new List<int>();
            for (int i = source.Count - 1; i >= 0; i--)
            //for (int i = 0; i < source.Count ; i ++)
            {
                
                int indexNearest = KdTree_Stark.FindNearest(source[i]);
                //result.Add(target[indexNearest]);

                if (!indicesTargetFound.Contains(indexNearest))
                {
                    indicesTargetFound.Add(indexNearest);
                    result.Add(target[indexNearest]);
                }
                else
                {
                    bool bfound = false;
                    for (int j = 0; j < KDTree_Stark.LatestSearchResults.Count; j++)
                    {
                        int newIndex = KDTree_Stark.LatestSearchResults[j].Key;
                        if (!indicesTargetFound.Contains(newIndex))
                        {
                            bfound = true;
                            indicesTargetFound.Add(newIndex);
                            result.Add(target[newIndex]);
                            break;

                        }

                    }
                    if(!bfound)
                        source.RemoveAt(i);
                }


            }

            return result;
        }
        public List<Vertex> FindNearest_Points_Stark(List<Vertex> source)
        {
            KdTree_Stark.ResetSearch();
            List<Vertex> result = new List<Vertex>();
            List<Vertex> tempList = source;
            List<int> indicesTargetFound = new List<int>();

            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                Vertex v = tempList[i];
                //tempList.RemoveAt(i);

                int neighboursFound = 0;
                v.IndexNeighbours = new List<int>();
                for (int j = 0; j < 5; j++)
                {
                    int indexNearest = KdTree_Stark.FindNearest(tempList[i]);
                    if (!v.IndexNeighbours.Contains(indexNearest))
                    {
                        KDTree_Stark.LatestSearchResults.Sort(new KeyValueComparer());
                        neighboursFound++;
                        v.IndexNeighbours.Add(indexNearest);
                        KeyValuePair<int, double> res = KDTree_Stark.LatestSearchResults[0];
                        v.DistanceNeighbours.Add(res.Value);
                    }
                    if ((neighboursFound + 1) > NumberOfNeighboursToSearch)
                        break;
                   
                   
                }


            }

            return result;
        }
     
        public List<Vertex> FindNearest_BruteForce(List<Vertex> source, List<Vertex> target)
        {



            List<Vertex> result = new List<Vertex>();
            List<int> indicesTargetFound = new List<int>();

            List<Vertex> tempTarget = Vertices.CopyVertices(target);

            for (int i = source.Count - 1; i >= 0; i--)
            {
                BuildKDTree_Stark(tempTarget);

                int indexNearest = KdTree_Stark.FindNearest(source[i]);
                result.Add(target[indexNearest]);
                tempTarget.RemoveAt(indexNearest);

            }

            return result;


           
        }
        public List<Vertex> FindNearest_BruteForceOld(List<Vertex> vSource, List<Vertex> vTarget)
        {
            List<Vertex> nearestNeighbours = new List<Vertex>();
            int iMax = 10;
            List<Vertex> tempTarget = Vertices.CopyVertices(vTarget);

            for (int i = 0; i < vSource.Count; i++)
            {
                //BuildKDTree_Standard(tempTarget);

                Vertex p = vSource[i];
                // Perform a nearest neighbour search around that point.
                KDTreeRednaxela.NearestNeighbour<EllipseWrapper> pIter = null;
                pIter = KdTree_Rednaxela.FindNearest_EuclidDistance(new double[] { p.Vector.X, p.Vector.Y, p.Vector.Z }, iMax, -1);
                while (pIter.MoveNext())
                {
                    // Get the ellipse.
                    //var pEllipse = pIter.Current;
                    EllipseWrapper wr = pIter.Current;
                    nearestNeighbours.Add(wr.Vertex);
                    tempTarget.RemoveAt(pIter.CurrentIndex);
                    break;
                }

            }
            return nearestNeighbours;
        }
       
      
        public void InitVertices(List<Vertex> vertices)
        {
             //List<Vertex> tempList = new List<Vertex>();
            for (int i = vertices.Count -1; i >=0 ; i--)
            {
                Vertex v = vertices[i];
                v.IndexNeighbours = new List<int>();
                v.DistanceNeighbours = new List<double>();
                vertices[i] = v;

            }
        }
        public void FindNearest_Points_Rednaxela(List<Vertex> vertices)
        {

            
            //float fThreshold = kdTreeNeighbourThreshold;
            List<Vertex> nearestNeighbours = new List<Vertex>();

            for (int i = vertices.Count - 1; i >= 0; i--)
            {
                Vertex vToCheck = vertices[i];

                // Perform a nearest neighbour search around that point.
                KDTreeRednaxela.NearestNeighbour<EllipseWrapper> pIter = null;
                KDTreeVertex kv = KDTreeVertex.Instance;
                
                pIter = kv.KdTree_Rednaxela.FindNearest_EuclidDistance(new double[] { vToCheck.Vector.X, vToCheck.Vector.Y, vToCheck.Vector.Z }, NumberOfNeighboursToSearch , -1);
                int neighboursFound = 0;

                while (pIter.MoveNext())
                {
                    EllipseWrapper wr = pIter.Current;
                    Vertex vNeighbour = wr.Vertex;
                    if (vToCheck != vNeighbour)
                    {
                        
                        if (!vToCheck.IndexNeighbours.Contains(vNeighbour.IndexInModel))
                        {
                            vToCheck.IndexNeighbours.Add(vNeighbour.IndexInModel);
                            vToCheck.DistanceNeighbours.Add(pIter.CurrentDistance);
                        }
                        for (int j = vNeighbour.IndexNeighbours.Count -1; j >=0 ; j--)
                        {
                            if (vNeighbour.IndexNeighbours[j] == vToCheck.IndexInModel)
                            {
                                vNeighbour.IndexNeighbours.RemoveAt(j);
                                vNeighbour.DistanceNeighbours.RemoveAt(j);
                            }
                        }
                        neighboursFound = vToCheck.DistanceNeighbours.Count;
                            //if (!vNeighbour.IndexNeighbours.Contains(vToCheck.IndexInModel))
                            //{
                            //    vNeighbour.IndexNeighbours.Add(vToCheck.IndexInModel);
                            //    vNeighbour.DistanceNeighbours.Add(pIter.CurrentDistance);
                            //}
                            if ((neighboursFound) > NumberOfNeighboursToSearch)
                                break;
                    }
                }

            }

            TimeCalc.ShowLastTimeSpan("Find neighbours");

            //RemoveAllVerticesBasedOnRadius(vertices);


        }
        private void RemoveAllVerticesBasedOnRadius(List<Vertex> vertices)
        {
            //remove all vertices beyound minimal radius
            //(distance list is automatically sorted due to KDTree)
            for (int i = vertices.Count - 1; i >= 0; i--)
            {
                Vertex v = vertices[i];
                //if(v.DistanceNeighbours.Count < NumberOfNeighboursToSearch)
                double distanceMax = v.DistanceNeighbours[NumberOfNeighboursToSearch - 1];
                for (int j = v.DistanceNeighbours.Count - 1; j >= 2; j--)
                {

                    if (v.DistanceNeighbours[j] > distanceMax)
                    {
                        v.DistanceNeighbours.RemoveAt(j);
                        v.IndexNeighbours.RemoveAt(j);
                    }

                }


            }
        }
    }
  
}