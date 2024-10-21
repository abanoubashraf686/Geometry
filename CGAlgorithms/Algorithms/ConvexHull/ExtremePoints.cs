using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGUtilities.Enums;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremePoints : Algorithm
    {
        public static bool checkPoint(int i, List<Point> points)
        {
            for (int j = 0; j < points.Count; j++)
                for (int k = j + 1; k < points.Count; k++)
                    for (int l = k + 1; l < points.Count; l++)
                        if (j!=i && k!=i && l!=i && HelperMethods.PointInTriangle(points[i], points[j], points[k], points[l]) != PointInPolygon.Outside)
                            return false;
            return true;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // For each i 
            //   For each j,k,l != i
            //      if pi lies in triangle pj,pk,pl, eliminate pi
            outPoints = new List<Point>();
            int i = 0;
            for (i=0; i<points.Count; i++)
                outPoints.Add(points[i]);
            i = 0;
            while(i < outPoints.Count)
            {
                if (checkPoint(i,outPoints))
                    i++;
                else  
                    outPoints.RemoveAt(i);
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Points";
        }
    }
}
