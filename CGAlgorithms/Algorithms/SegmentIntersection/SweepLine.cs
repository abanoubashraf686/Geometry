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

        public const double EPS = 1e-6;
        public class EventComparer : IComparer<KeyValuePair<Line, bool>>
        {
            public int Compare(KeyValuePair<Line, bool> l1, KeyValuePair<Line, bool> l2) {
                Point p1 = l1.Value == false ? l1.Key.Start : l1.Key.End;
                Point p2 = l2.Value == false ? l2.Key.Start : l2.Key.End;
                if (p1.X != p2.X) return p1.X.CompareTo(p2.X);
                if (p1.Y != p2.Y) return p1.Y.CompareTo(p2.Y);
                return l1.Value.CompareTo(l2.Value);
            }
        }
        public class YListComparer : IComparer<Line> {
            public double currentX;
            public int Compare(Line l1, Line l2)
            {
                double y1 = HelperMethods.GetY(l1, currentX);
                double y2 = HelperMethods.GetY(l2, currentX);
                return y1.CompareTo(y2);
            }
        }
        public static YListComparer Ycomparer = new YListComparer();
        public void checkIntersection(Line l1, Line l2, ref SortedSet<Intersection> I, ref List<Point> outPoints, ref Dictionary<KeyValuePair<int, int>, bool> intersectionID)
        {
            int id1 = Math.Min(l1.ID, l2.ID);
            int id2 = Math.Max(l1.ID, l2.ID);
            KeyValuePair<int, int> ids = new KeyValuePair<int, int>(id1, id2);
            if (id1 == id2 || intersectionID.ContainsKey(ids))
                return;
            
            Intersection i = new Intersection(l1, l2);
            if (i.hasIntersection && i.intersectionPoint.X > Ycomparer.currentX)
            {
                I.Add(i);
                outPoints.Add(i.intersectionPoint);
                intersectionID[ids] = true;
            }
        }
        public void handleIntersection(Line l, bool add, ref SortedSet<Intersection> I, ref AVLTree<Line> L, ref List<Point> outPoints, ref Dictionary<KeyValuePair<int, int>, bool> intersectionID) {
            if (add)
            {
                L.insert(l);
            }
            Node<Line> l1 = L.prev(l);
            Node<Line> l2 = L.next(l);
            if (!add)
            {
                L.erase(l);
            }
            if (add && l1 != null)            
                checkIntersection(l1.value, l, ref I, ref outPoints, ref intersectionID);
            if (add && l2 != null)
                checkIntersection(l2.value, l, ref I, ref outPoints, ref intersectionID);
            if (!add && l1 != null && l2 != null)
                checkIntersection(l1.value, l2.value, ref I, ref outPoints, ref intersectionID);
        }
        public void print(string s)
        {
            Debug.Assert(false, s);
        }
        public override void Run(List<CGUtilities.Point> points, List<CGUtilities.Line> lines, List<CGUtilities.Polygon> polygons, ref List<CGUtilities.Point> outPoints, ref List<CGUtilities.Line> outLines, ref List<CGUtilities.Polygon> outPolygons)
        {
            if (lines.Count == 0) return;

            SortedSet<KeyValuePair<Line, bool>> E = new SortedSet<KeyValuePair<Line, bool>>(new EventComparer());
            AVLTree<Line> L = new AVLTree<Line>(Ycomparer);
            SortedSet<Intersection> I = new SortedSet<Intersection>();
            Dictionary<KeyValuePair<int, int>, bool> intersectionID = new Dictionary<KeyValuePair<int, int>, bool>();
            string s = "";
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
            
            Ycomparer.currentX = E.Min.Key.Start.X;
            int id = 1;
            foreach (var e in E)
            {
                if (e.Value == false) e.Key.ID = id++;
            }

            int cnt = 0;
            foreach (var e in E)
            {
                Point p;
                p = e.Value == false ? e.Key.Start : e.Key.End;
                Intersection curIntersection;
                while (I.Count > 0 && (curIntersection = I.Min).intersectionPoint.X < p.X)
                {
                    cnt++;
                    Node<Line> l1 = L.lower_bound(curIntersection.l1);
                    Node<Line> l2 = L.lower_bound(curIntersection.l2);

                     /*if (!(l1 != null && l1.value.Equals(curIntersection.l1))||
                          !(l2 != null && l2.value.Equals(curIntersection.l2)))
                      {
                            s = "";
                            s = "intersection #" + cnt.ToString();
                            s += " Between " + curIntersection.l1.ToString() + " " + curIntersection.l2.ToString() + "\n";
                            s += L.ToString();
                            s += "l1 = " + curIntersection.l1.ToString() + "\n";
                            s += "l1 lower = " + l1.value.ToString() + "\n";
                            s += "l2 = " + curIntersection.l2.ToString() + "\n";
                            s += "l2 lower = " + l2.value.ToString() + "\n";
                            print(s);
                     }*/
                    /*Ycomparer.currentX = I.Min.intersectionPoint.X - EPS;
                    L.erase(I.Min.l1);
                    L.erase(I.Min.l2);

                    Ycomparer.currentX = I.Min.intersectionPoint.X + EPS;
                    handleIntersection(I.Min.l1, true, ref I, ref L, ref res);
                    handleIntersection(I.Min.l2, true, ref I, ref L, ref res);
                    */
                    Node<Line> prvl1 = L.prev(l1.value);
                    Node<Line> nxtl1 = L.next(l1.value);

                    Node<Line> prvl2 = L.prev(l2.value);
                    Node<Line> nxtl2 = L.next(l2.value);

                    Line tmp = l1.value;
                    l1.value = l2.value;
                    l2.value = tmp;
                    Ycomparer.currentX = curIntersection.intersectionPoint.X;
                    
                    if (prvl1 != null) checkIntersection(l1.value, prvl1.value, ref I, ref outPoints, ref intersectionID);
                    if (nxtl1 != null) checkIntersection(l1.value, nxtl1.value, ref I, ref outPoints, ref intersectionID);
                    if (prvl2 != null) checkIntersection(l2.value, prvl2.value, ref I, ref outPoints, ref intersectionID);
                    if (nxtl2 != null) checkIntersection(l2.value, nxtl2.value, ref I, ref outPoints, ref intersectionID);
                    
                    I.Remove(curIntersection);
                }
                // Debug.Assert(Ycomparer.currentX < p.X);
                Ycomparer.currentX = p.X;
                handleIntersection(e.Key, !e.Value, ref I, ref L, ref outPoints, ref intersectionID);
            }
            
         //   print(outPoints.Count.ToString());
/*            s = "";
            foreach(Point p in outPoints)
            {
                s += p.ToString() + "\n";
            }
            print(s);*/
        }

        public override string ToString()
        {
            return "Sweep Line";
        }
    }
}
