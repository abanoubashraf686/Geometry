using CGUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            //double mag = Line.Magnitude();
            Point pl =null, pr=null;
            double currDist = 0;
            foreach (Point pi in points)
            {
                double dist = Math.Abs(HelperMethods.CrossProduct(Line, pi - p1));
                if (dist > currDist)
                {
                    pl = pr = pi;
                    currDist = dist;
                }
                else if (dist == currDist)
                {
                    if (HelperMethods.CheckTurn(pl - p1, pi - pl) == TurnType.Left)
                        pl = pi;
                    else if (HelperMethods.CheckTurn(pr - p2, pi - pr) == TurnType.Right)
                        pr = pi;
                }
            }
            if (currDist == 0)
            {
                foreach (Point pi in points)
                    outPoints.Add(pi);
                return;
            }
            List<Point> L = new List<Point>(), R = new List<Point>();
            Point p1pl = pl - p1, p2pr = pr - p2;
            foreach (Point pi in points)
            {

                // Left p1p3
                if (HelperMethods.CheckTurn(p1pl, pi - pl) == TurnType.Left)
                {
                    L.Add(pi);
                }
                // right p2p3
                else if (HelperMethods.CheckTurn(p2pr, pi - pr) == TurnType.Right)
                {
                    R.Add(pi);
                }
            }
            outPoints.Add(pl);
            if(pr!=pl)
                outPoints.Add(pr);
            solve(p1, pl, L, ref outPoints);
            solve(pr, p2, R, ref outPoints);
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            Point p1 = points[0], p2 = p1 , p3 = p1;
            foreach (Point point in points)
            {
                //Console.WriteLine(point);
                if (point.X < p1.X || (point.X==p1.X && point.Y<p1.Y))
                    p1 = point;
                if (point.X > p2.X || (point.X == p2.X && point.Y < p2.Y))
                    p2 = point;
                if (p3.Y < point.Y)
                    p3 = point;
            }
            outPoints = new List<Point>();
            outPoints.Add(p1);
            if (p1.X == p2.X)
            {
                if(p3.Y!=p1.Y)
                    outPoints.Add(p3);
                return;
            }
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
