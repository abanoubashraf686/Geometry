using CGUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Net;

namespace CGAlgorithms.Algorithms.SegmentIntersection
{
    class SweepLine : Algorithm
    {
        public class Intersection : IComparable<Intersection>
        {
            public Line l1 { get; private set; }
            public Line l2 { get; private set; }
            public Point intersectionPoint { get; private set; }
            public bool hasIntersection { get; private set; }
            public Intersection(Line l1, Line l2)
            {
                this.l1 = l1;
                this.l2 = l2;
                hasIntersection = false;
                TryCalculateIntersection();
            }
            private void TryCalculateIntersection()
            {
                Point p1 = l1.Start, p2 = l1.End, p3 = l2.Start, p4 = l2.End;
                double d = (p2.X - p1.X) * (p4.Y - p3.Y) - (p2.Y - p1.Y) * (p4.X - p3.X);
                if (d == 0) return;

                double t = ((p3.X - p1.X) * (p4.Y - p3.Y) - (p3.Y - p1.Y) * (p4.X - p3.X)) / d;
                double u = ((p3.X - p1.X) * (p2.Y - p1.Y) - (p3.Y - p1.Y) * (p2.X - p1.X)) / d;

                if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    double x = (p1.X + t * (p2.X - p1.X));
                    double y = (p1.Y + t * (p2.Y - p1.Y));
                    intersectionPoint = new Point(x, y);
                    hasIntersection = true;
                }
            }
            public int CompareTo(Intersection other)
            {
                // compare the intersection point by X
                Debug.Assert(hasIntersection);
                Debug.Assert(other.hasIntersection);

                if (intersectionPoint.X != other.intersectionPoint.X)
                    return intersectionPoint.X.CompareTo(other.intersectionPoint.X);
                return intersectionPoint.Y.CompareTo(other.intersectionPoint.Y);
            }
        }

        public static double currentX;
        public const double ESP = 1e-4;

        public class YListComparer : IComparer<Line> {
            public int Compare(Line l1, Line l2)
            {
                double y1 = HelperMethods.GetY(l1, currentX);
                double y2 = HelperMethods.GetY(l2, currentX);
                return y1.CompareTo(y2);
            }
        }
        public class PointComparer : IComparer<Point>
        {
            public int Compare(Point p1, Point p2)
            {
                if (p1.X != p2.X) return p1.X.CompareTo(p2.X);
                return p1.Y.CompareTo(p2.Y);
            }
        }
        public void checkIntersection(Line l1, Line l2, ref SortedSet<Intersection> I, ref SortedSet<Point> res)
        {
            Intersection i = new Intersection(l1, l2);
            if (i.hasIntersection)
            {
                if (i.intersectionPoint.X > currentX)
                    I.Add(i);
                res.Add(i.intersectionPoint);
            }
        }
        public void handleIntersection(Line l, bool add, ref SortedSet<Intersection> I, ref SortedSet<Line> L, ref SortedSet<Point> res) {
            if (!add) L.Remove(l);
            
            bool ok1 = false, ok2 = false;
            Line l1 = new Line(new Point(0, 0), new Point(0, 0));
            Line l2 = new Line(new Point(0, 0), new Point(0, 0));

            foreach (var line in L)
            {
                if (HelperMethods.GetY(line, currentX) < HelperMethods.GetY(l, currentX))
                {
                    l1 = line;
                    ok1 = true;
                } else if (!ok2)
                {
                    l2 = line;
                    ok2 = true;
                }
            }
            if (ok1)
                checkIntersection(l1, l, ref I, ref res);
            if (ok2)
                checkIntersection(l2, l, ref I, ref res);
            if (ok1 && ok2)
                checkIntersection(l1, l2, ref I, ref res);

            if (add) L.Add(l);
        }
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            List<KeyValuePair<Line, bool>> E = new List<KeyValuePair<Line, bool>>();
            SortedSet<Line> L = new SortedSet<Line>(new YListComparer());
            SortedSet<Intersection> I = new SortedSet<Intersection>();
            SortedSet<Point> res = new SortedSet<Point>(new PointComparer());

            foreach (var l in lines)
            {
                Line l_;
                if (l.Start.X == l.End.X) continue; // need to handled

                if (l.Start.X < l.End.X)
                    l_ = new Line(l.Start, l.End);
                else
                    l_ = new Line(l.End, l.Start);
                E.Add(new KeyValuePair<Line, bool>(l_, false));
                E.Add(new KeyValuePair<Line, bool>(l_, true));
            }
            E.Sort((l1, l2) =>
            {
                Point p1 = l1.Value == false ? l1.Key.Start : l1.Key.End;
                Point p2 = l2.Value == false ? l2.Key.Start : l2.Key.End;
                if (p1.X != p2.X) return p1.X.CompareTo(p2.X);
                if (p1.Y != p2.Y) return p1.Y.CompareTo(p2.Y);
                return l1.Value.CompareTo(l2.Value);
            });

            foreach(var e in E)
            {
                Point p;
                p = e.Value == false ? e.Key.Start : e.Key.End;
                while (I.Count > 0 && I.Min.intersectionPoint.X < p.X)
                {
                    
                    currentX = I.Min.intersectionPoint.X - ESP;
                    L.Remove(I.Min.l1);
                    L.Remove(I.Min.l2);

                    currentX = I.Min.intersectionPoint.X + ESP; 
                    handleIntersection(I.Min.l1, true, ref I, ref L, ref res);
                    handleIntersection(I.Min.l2, true, ref I, ref L, ref res);
                    I.Remove(I.Min);
                }
                currentX = p.X;
                handleIntersection(e.Key, !e.Value, ref I, ref L, ref res);
            }
            outPoints = new List<Point>(res);
        }

        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
