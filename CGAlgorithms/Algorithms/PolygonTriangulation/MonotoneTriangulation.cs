using CGUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace CGAlgorithms.Algorithms.PolygonTriangulation
{
    class MonotoneTriangulation  :Algorithm
    {
        public override void Run(System.Collections.Generic.List<CGUtilities.Point> Ps, System.Collections.Generic.List<CGUtilities.Line> lines, 
            System.Collections.Generic.List<CGUtilities.Polygon> polygons, ref System.Collections.Generic.List<CGUtilities.Point> outPoints, ref System.Collections.Generic.List<CGUtilities.Line> outLines, ref System.Collections.Generic.List<CGUtilities.Polygon> outPolygons)
        {
            Polygon polygon = polygons[0];
            List<Line> Lines = polygon.lines;
            int n = Lines.Count;
            int maxYLineIdx = 0;
            List<Point> points = new List<Point>();
            for (int i = 0; i < n; i++)
            {
                Point oldPoint = Lines[maxYLineIdx].Start;
                Point newPoint = Lines[i].Start;
                points.Add(newPoint);
                if (newPoint.Y > oldPoint.Y || newPoint.Y == oldPoint.Y && newPoint.X < oldPoint.X)
                {
                    maxYLineIdx = i;
                }
            }
            // start[-1] start[i], end[i] -> check right ? right cycle
            int dir = -1;
            if (HelperMethods.CheckTurn(points[(maxYLineIdx - 1 + n) % n], points[maxYLineIdx], points[(maxYLineIdx + 1 + n) % n]) == Enums.TurnType.Right)
                dir = +1;
            //Debug.Assert(false, "dir: "+dir.ToString());
            List<Point> st = new List<Point>();
            int curr = maxYLineIdx, op = (curr - dir + n) % n;
            //outLines.Add(new Line(points[op], points[curr]));
            while(true)
            {
                Enums.TurnType turn = (dir == 1 ? Enums.TurnType.Right : Enums.TurnType.Left);
                while (points[op].Y < points[curr].Y)
                {
                    Point last = points[curr];
                    while (st.Count >= 2 && (HelperMethods.CheckTurn(st[st.Count - 2], st[st.Count - 1], last)) == turn)
                    {
                        // Add triangle 
                        outLines.Add(new Line(last, st[st.Count - 2]));
                        st.RemoveAt(st.Count - 1);
                    }
                    st.Add(last);
                    curr = (curr + dir + n) % n;
                }
                if (points[curr].Equals(points[op])) break;

                if (st.Count > 0)
                {
                    Point last = st.Last();
                    while (st.Count > 1)
                    {
                        // Add triangle 
                        outLines.Add(new Line(points[op], st[st.Count - 1]));
                        st.RemoveAt(st.Count - 1);
                    }
                    st.RemoveAt(st.Count - 1);
                    st.Add(last);
                }
                int tmp = curr;
                curr = op;
                op = tmp;
                dir *= -1;
            };
        }
        public override string ToString()
        {
            return "Monotone Triangulation";
        }
    }
}
