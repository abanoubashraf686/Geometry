using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGUtilities.Enums;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class QuickHull : Algorithm
    {
        public static void solve(Point p1, Point p2, List<Point> points, ref List<Point> outPoints)
        {
            if (points.Count == 0)
                return;
            Point Line = p2 - p1;
            double mag = Line.Magnitude();
            Point p3 = points.First();
            double currDist = -1;
            foreach (Point pi in points)
            {
                double dist = Math.Abs(HelperMethods.CrossProduct(Line, pi - p1));
                if (dist > currDist)
                {
                    p3 = pi;
                    currDist = dist;
                }
            }
            List<Point> L = new List<Point>(), R = new List<Point>();
            Point p1p3 = p3 - p1, p2p3 = p3 - p2;
            foreach (Point pi in points)
            {
                // Left p1p3
                if (HelperMethods.CheckTurn(p1p3, pi - p3) == TurnType.Left)
                {
                    L.Add(pi);
                }
                // right p2p3
                else if (HelperMethods.CheckTurn(p2p3, pi - p3) == TurnType.Right)
                {
                    R.Add(pi);
                }
            }
            outPoints.Add(p3);
            solve(p1, p3, L, ref outPoints);
            solve(p3, p2, R, ref outPoints);
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            // min and max of x and y
            // four lines minx-maxy,
            // remove any point inside polygon 
            // left minx-maxy
            // left minx-miny
            // right maxx-miny
            // right maxx-maxy
            Point p1 = points[0], p2 = p1, p3 = p1, p4 = p1;
            //     p3

            //p1------p2

            //    p4
            foreach (Point point in points)
            {
                Console.WriteLine(point);
                if (point.X < p1.X)
                    p1 = point;
                if (point.X > p2.X)
                    p2 = point;
                if (point.Y > p3.Y)
                    p3 = point;
                if (point.Y < p4.Y)
                    p4 = point;
            }
            outPoints = new List<Point>();
            if (p1.X == p2.X)
            {
                if (p3.Y == p4.Y)
                {
                    outPoints.Add(p3);
                }
                else
                {
                    outPoints.Add(p3);
                    outPoints.Add(p4);
                }
                return;
            }
            outPoints.Add(p1);
            outPoints.Add(p2);
            List<Point> L1 = new List<Point>(), L2 = new List<Point>();
            foreach (Point p in points)
            {
                Enums.TurnType x = HelperMethods.CheckTurn(p2 - p1, p - p2);
                if (x == TurnType.Left)
                { L1.Add(p); }
                else if (x == TurnType.Right)
                { L2.Add(p); }
            }
            solve(p1, p2, L1, ref outPoints);
            solve(p2, p1, L2, ref outPoints);
        }

        public override string ToString()
        {
            return "Convex Hull - Quick Hull";
        }
    }
}
