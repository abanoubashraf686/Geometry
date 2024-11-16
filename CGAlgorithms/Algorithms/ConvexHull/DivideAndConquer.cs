using CGUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class DivideAndConquer : Algorithm
    {
        static public Point center;
        static public int cmp_CW(Point p1, Point p2)
        {
            return HelperMethods.getAngle(p1, center).CompareTo(HelperMethods.getAngle(p2, center));
        }
        static public void sortInCW(ref List<Point> res)
        {
            int n = res.Count;
            center = new Point(0, 0);
            for (int i = 0; i < n; i++)
                center += res[i];
            center /= n;
            res.Sort(cmp_CW);
        }
        static public void getTangent(List<Point> l, List<Point> r, ref int i, ref int j, int up)
        {

            int n1 = l.Count, n2 = r.Count;
            // upper tangent 
            Enums.TurnType dir1, dir2;
            if (up == 1)
            {
                dir1 = Enums.TurnType.Right;
                dir2 = Enums.TurnType.Left;
            }
            else
            {
                dir1 = Enums.TurnType.Left;
                dir2 = Enums.TurnType.Right;
            }
            bool change = true;
            int it = 0;
            while (change)
            {
                int ii = i, jj = j, nxt;
                // l[up],r[rup],r[rup-1] turn right 
                if (n2 == 2 && HelperMethods.CheckTurn(r[j] - l[i], r[nxt = ((j - up + n2) % n2)] - r[j]) == Enums.TurnType.Colinear)
                {
                    j = 1;
                }
                else
                {
                    while (HelperMethods.CheckTurn(r[j] - l[i], r[nxt = ((j - up + n2) % n2)] - r[j]) != dir1)
                    {
                        j = nxt;
                        it++;
                        Debug.Assert(it < 100000000);
                    }
                }
                // r[rup],l[lup],l[lup+1] turn 
                if(n1==2 && HelperMethods.CheckTurn(l[i] - r[j], l[nxt = ((i + up + n1) % n1)] - l[i])== Enums.TurnType.Colinear)
                {
                    i = 0;
                }
                else
                {
                    while (HelperMethods.CheckTurn(l[i] - r[j], l[nxt = ((i + up + n1) % n1)] - l[i]) != dir2)
                    {
                        i = nxt;
                        it++;
                        Debug.Assert(it < 100000000);
                    }
                }
                change = (ii != i ||  j!=jj);
            }
        }
        static public void AddRange(List<Point> list, int l, int r, ref List<Point> res)
        {
            int n = list.Count;
            while (l != r)
            {
                res.Add(list[l]);
                l = (l + 1) % n;
            }
            res.Add(list[r]);
        }
        static public void merge(List<Point> l, List<Point> r, ref List<Point> res)
        {
            int idx1 = 0, idx2 = 0;
            int n1 = l.Count, n2 = r.Count;
            for (int i = 0; i < n1; i++)
                if (l[i].X > l[idx1].X)
                    idx1 = i;
            for (int i = 0; i < n2; i++)
                if (r[i].X < r[idx2].X)
                    idx2 = i;
            int l_up = idx1, r_up = idx2;
            int l_low = idx1, r_low = idx2;
            getTangent(l, r, ref l_up, ref r_up, 1);
            getTangent(l, r, ref l_low, ref r_low, -1);
            AddRange(l, l_up, l_low, ref res);
            AddRange(r, r_low, r_up, ref res);
        }
        static public List<Point> solve(List<Point> points)
        {
            List<Point> res = new List<Point>();
            int n = points.Count;
            if (n < 6)
            {
                ExtremeSegments.solve(points, ref res);
                sortInCW(ref res);
                return res;
            }
            List<Point> l = new List<Point>(), r = new List<Point>();
            for (int i = 0; i < n / 2; i++)
                l.Add(points[i]);
            for (int i = n / 2; i < n; i++)
                r.Add(points[i]);
            l = solve(l);
            r = solve(r);
            merge(l, r, ref res);
            return res;
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            SortedSet<Point> sortedPoints = new SortedSet<Point>();
            foreach (Point p in points)
                sortedPoints.Add(p);
            points = sortedPoints.ToList();
            outPoints = solve(points);
        }

        public override string ToString()
        {
            return "Convex Hull - Divide & Conquer";
        }

    }
}
