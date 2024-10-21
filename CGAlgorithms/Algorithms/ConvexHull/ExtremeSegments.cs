using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class ExtremeSegments : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int n = points.Count;
            if (n == 1)
            {
                outPoints.Add(points[0]);
                return;
            }
            List<int> take = new List<int>(new int[n]); // 0 = don't know, 1 = leave, 2 = take
            for (int i = 0; i < n; i++)
            {
                if (take[i] != 1) take[i] = 0;
                for (int j = i+1; j < n; j++)
                {
                    if (points[i].Equals(points[j])) take[j] = 1;
                }
            }
            for (int i = 0; i < n; i++)
            {
                if (take[i] == 1) continue;
                for (int j = i+1; j < n; j++)
                {
                    if (take[j] == 1) continue;
                    bool takeEdge = true, l = false, r = false;
                    Line ij = new Line(points[i], points[j]);
                    for (int k = 0; k < n; k++)
                    {
                        if (k == i || k == j || take[k] == 1) continue;
                       
                        Enums.TurnType t = HelperMethods.CheckTurn(ij, points[k]);

                        if (t == Enums.TurnType.Left) l = true;
                        else if (t == Enums.TurnType.Right) r = true;
                        else if (HelperMethods.PointOnSegment(points[k], points[i], points[j])) take[k] = 1;
                                                
                        if (l && r) takeEdge = false;
                        
                    }
                    if (takeEdge)
                    {
                        if (take[i] != 1) take[i] = 2;
                        if (take[j] != 1) take[j] = 2;
                    }                    
                }
            }
            for (int i = 0; i < n; i++)
            {
                if (take[i] == 2)
                {
                    outPoints.Add(points[i]);
                }
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Extreme Segments";
        }
    }
}
