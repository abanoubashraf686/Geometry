using CGUtilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CGUtilities.HelperMethods;

namespace CGAlgorithms.Algorithms.ConvexHull
{
    public class Incremental : Algorithm
    {
        public void TryToAdd(Point p, AVLTree T)
        {
            if (T.size < 2)
            {
                T.insert(p);
                return;
            }
            Node nxt1 = T.lower_bound(p);
            // exist
            if (nxt1 != null && nxt1.p.X == p.X && nxt1.p.Y == p.Y)
                return;
            Node prev1 = T.prev(nxt1);
            if (nxt1 != null && prev1 != null && CheckTurn(nxt1.p - prev1.p, p - nxt1.p) != Enums.TurnType.Left)
                return;
            Node prev2;
            while (prev1 != null && (prev2 = T.prev(prev1)) != null && CheckTurn(prev1.p - p, prev2.p - prev1.p) != Enums.TurnType.Left)
            {
                T.erase(prev1.p);
                prev1 = prev2;
            }
            Node nxt2;
            while (nxt1 != null && (nxt2 = T.next(nxt1)) != null && CheckTurn(nxt1.p - p, nxt2.p - nxt1.p) != Enums.TurnType.Right)
            {
                T.erase(nxt1.p);
                nxt1 = nxt2;
            }
            T.insert(p);
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            AVLTree upper_convex = new AVLTree() , lower_convex = new AVLTree();
            outPoints = new List<Point>();
            foreach (Point p in points)
            {
                TryToAdd(p, upper_convex);
                TryToAdd(p*-1, lower_convex);
            }
            lower_convex.traverse(ref outPoints);
            foreach (Point p in outPoints)
            {
                p.X *= -1;
                p.Y *= -1;
            }
            outPoints.RemoveAt(outPoints.Count - 1);
            upper_convex.traverse(ref outPoints);
            if (outPoints.Count > 1)
                outPoints.RemoveAt(outPoints.Count - 1);
        }
        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
