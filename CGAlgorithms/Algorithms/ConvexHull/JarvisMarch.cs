using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class JarvisMarch : Algorithm
    {
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int n = points.Count;
            if (n == 1)
            {
                outPoints.Add(points[0]);
                return;
            }

            Point v = points[0];
            for (int i = 0; i < n; i++)
            {
                if (points[i].X < v.X || points[i].X == v.X && points[i].Y < v.Y) v = points[i];
            }
            outPoints.Add(v);
            while (true)
            {
                v = outPoints.Last();
                Point p = null;
                for (int i = 0; i < n; i++)
                {
                    if (points[i].Equals(v)) continue;
                    if (p == null)
                    {
                        p = points[i];
                    }
                    else
                    {
                        Enums.TurnType t = HelperMethods.CheckTurn(new Line(v, p), points[i]);
                        if (t == Enums.TurnType.Right) p = points[i];
                        else if (HelperMethods.PointOnSegment(p, v, points[i])) p = points[i];
                    }
                }
                if (p == null || p.Equals(v) || p.Equals(outPoints.First())) break;
                outPoints.Add(p);
            }
        }

        public override string ToString()
        {
            return "Convex Hull - Jarvis March";
        }
    }
}
