using CGUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class GrahamScan : Algorithm
    {
        public double getAngle(Point vec1, Point vec2)
        {
            double angle = Math.Atan2(HelperMethods.CrossProduct(vec1, vec2), HelperMethods.DotProduct(vec1, vec2));
            angle = angle * 180 / Math.PI;
            if (angle < 0) angle += 360;
            return angle;
        }
        public bool check(Stack<Point> l)
        {
            // check the last 3 points
            if (l.Count < 3) return true;

            Point pi = l.Pop();
            Point p = l.Pop();
            Point p_ = l.Pop();

            Enums.TurnType t = HelperMethods.CheckTurn(new Line(p_, p), pi);
            if (t == Enums.TurnType.Left)
            {
                // push the 3 points
                l.Push(p_);
                l.Push(p);
                l.Push(pi);
                return true;
            }

            // push the first and the last and remove the middle
            if (t == Enums.TurnType.Right || HelperMethods.PointOnSegment(p, p_, pi))
            {
                l.Push(p_);
                l.Push(pi);
            }
            else if (HelperMethods.PointOnSegment(pi, p_, p))
            {
                l.Push(p_);
                l.Push(p);
            }
            else if (HelperMethods.PointOnSegment(p_, p, pi))
            {
                l.Push(p);
                l.Push(pi);
            }
            else Debug.Assert(false);
            return false;   
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            int n = points.Count;
            if (n <= 1)
            {
                outPoints = points;
                return;
            }
            Point p1 = points[0];
            for (int i = 1; i < n; i++)
            {
                if (points[i].Y < p1.Y || points[i].Y == p1.Y && points[i].X < p1.X) p1 = points[i];                
            }
            points.Remove(p1);
            Stack<Point> l = new Stack<Point>();
            Point p2 = new Point(p1.X + 10, p1.Y);
            Point vec = p2 - p1;
            points.Sort((p, q) =>
            {
                Point vec1 = p - p1;
                Point vec2 = q - p1;
                return getAngle(vec, vec1).CompareTo(getAngle(vec, vec2));
            });
            points.Add(p1); // Add it to close the polygon
            l.Push(p1);
            for (int i = 0; i < n; i++)
            {
                Point pi = points[i];
                if (pi.Equals(l.Peek())) continue;
                l.Push(pi);
                while (!check(l)); // Continue while removing points
            }
            while (l.Count > 0) outPoints.Add(l.Pop());
        
            if (outPoints.Count > 0 && outPoints[0].Equals(outPoints.Last())) // remove one if first = last
                outPoints.RemoveAt(outPoints.Count - 1);
        }

        public override string ToString()
        {
            return "Convex Hull - Graham Scan";
        }
    }
}
