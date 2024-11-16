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
        public void TryToAdd(Point p, AVLTree<Point> T)
        {
            if (T.size < 2)
            {
                T.insert(p);
                return;
            }

            Node<Point> nxt1 = T.lower_bound(p);

            // exist
            if (nxt1 != null && nxt1.value.X == p.X && nxt1.value.Y == p.Y)
                return;

            Node<Point> prev1 = T.prev(p);

            if (nxt1 != null && prev1 != null && CheckTurn(prev1.value - p, nxt1.value - prev1.value) != Enums.TurnType.Left)
                return;

            Node<Point> prev2;
            while (prev1 != null && (prev2 = T.prev(prev1.value)) != null &&
                CheckTurn(prev1.value - p, prev2.value - prev1.value) != Enums.TurnType.Left)
            {
                T.erase(prev1.value);
                prev1 = prev2;
            }

            Node<Point> nxt2;
            while (nxt1 != null && (nxt2 = T.next(nxt1.value)) != null &&
                CheckTurn(nxt1.value - p, nxt2.value - nxt1.value) != Enums.TurnType.Right)
            {
                T.erase(nxt1.value);
                nxt1 = nxt2;
            }

            T.insert(p);
        }

        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            AVLTree<Point> upper_convex = new AVLTree<Point>();
            AVLTree<Point> lower_convex = new AVLTree<Point>();
            outPoints = new List<Point>();

            foreach (Point p in points)
            {
                TryToAdd(p, upper_convex);
                TryToAdd(new Point(-p.X, -p.Y), lower_convex);
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
