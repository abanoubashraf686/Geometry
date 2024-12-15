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
                points.Add(Lines[i].Start);
                if (points[i].Y > points[maxYLineIdx].Y || (points[i].Y == points[maxYLineIdx].Y && points[i].X < points[maxYLineIdx].X))
                    maxYLineIdx = i;
            }
            //outPoints = points;
            // start[-1] start[i], end[i] -> check right ? right cycle
            int R = -1;
            if (HelperMethods.CheckTurn(points[(maxYLineIdx - 1 + n) % n], points[maxYLineIdx], points[(maxYLineIdx + 1 + n) % n]) == Enums.TurnType.Right)
                R = +1;
            int dir = R;
            //Debug.Assert(false, "dir: "+dir.ToString());
            List<Point> st = new List<Point>();
            int curr = maxYLineIdx, op = (curr - dir + n) % n;
            int cnt = 0;
            while(cnt<1e6)
            {
                cnt++;
                Enums.TurnType turn = (dir == R ? Enums.TurnType.Right : Enums.TurnType.Left);
                while (points[op].Y < points[curr].Y || (op!=curr && points[op].Y == points[curr].Y))
                {
                    Point last = points[curr];
                    while (st.Count >= 2)
                    {
                        Enums.TurnType t = (HelperMethods.CheckTurn(st[st.Count - 2], st[st.Count - 1], last));
                        if (t != turn)
                        {
                            if(t==Enums.TurnType.Colinear)
                                st.RemoveAt(st.Count - 1);
                            break;
                        }
                        outLines.Add(new Line(last, st[st.Count - 2]));
                        st.RemoveAt(st.Count - 1);
                    }
                    st.Add(last);
                    curr = (curr + dir + n) % n;
                }
                if (op==curr) break;
                if (st.Count > 1)
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
