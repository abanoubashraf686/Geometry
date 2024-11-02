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
        public class Node
        {
            public Node(Node left, Node right, int height, Point p)
            {
                this.left = left;
                this.right = right;
                this.height = height;
                this.p = p;
            }
            public Node left, right;
            public int height, balance;
            public Point p;
        }
        public class Incremental_convex
        {
            Node root, begin, end;
            public int size = 0;
            private int getHeight(Node node)
            {
                if (node == null) return 0;
                return node.height;
            }
            private int getBalance(Node node)
            {
                if (node == null) return 0;
                return getHeight(node.left) - getHeight(node.right);
            }
            private Node rotateRight(Node N)
            {
                if (N == null || N.left == null) return N;
                //     N
                //   p    a
                // b   c
                Node p = N.left;
                N.left = p.right;
                p.right = N;
                N.height = Math.Max(getHeight(N.left), getHeight(N.right)) + 1;
                p.height = Math.Max(getHeight(p.left), getHeight(p.right)) + 1;
                return p;
            }
            private Node rotateLeft(Node N)
            {
                if (N == null || N.right == null) return N;
                //       N
                //    a     p      
                //        b   c   
                Node p = N.right;
                N.right = p.left;
                p.left = N;
                N.height = Math.Max(getHeight(N.left), getHeight(N.right)) + 1;
                p.height = Math.Max(getHeight(p.left), getHeight(p.right)) + 1;
                return p;
            }
            private Node balance(Node root)
            {
                if (root == null)
                    return root;
                // update height 
                root.height = Math.Max(getHeight(root.left), getHeight(root.right)) + 1;
                // update balance 
                int balance = getBalance(root);
                // R 
                if (balance < -1)
                {
                    //RL
                    if (getBalance(root.right) > 0)
                        root.right = rotateRight(root.right);
                    //RR
                    return rotateLeft(root);
                }
                // L
                else if (balance > 1)
                {
                    //LR
                    if (getBalance(root.left) < 0)
                        root.left = rotateLeft(root.left);
                    //LL
                    return rotateRight(root);
                }
                return root;
            }
            private int compare(Point p1, Point p2)
            {
                int cmp = p1.angle.CompareTo(p2.angle);
                if (cmp == 0)
                    cmp = p1.distance.CompareTo(p2.distance);
                return cmp;
            }
            private Node insert(Point p, Node root)
            {
                if (root == null)
                {
                    root = new Node(null, null, 0, p);
                    if (begin == null || compare(p, begin.p) == -1)
                        begin = root;
                    if (end == null || compare(p, end.p) == 1)
                        end = root;
                    size++;
                    return root;
                }
                int cmp = compare(p, root.p);
                // root = p 
                if (cmp == 0)
                    return root;
                // p < root  
                if (cmp == -1)
                    root.left = insert(p, root.left);
                else
                    root.right = insert(p, root.right);
                return balance(root);
            }
            private Node GetSuccessor(Node curr)
            {
                curr = curr.right;
                while (curr != null && curr.left != null)
                {
                    curr = curr.left;
                }
                return curr;
            }
            private Node balance(Node root, Point p)
            {
                // Update height
                root.height = Math.Max(getHeight(root.left), getHeight(root.right)) + 1;

                // Calculate balance factor
                int balance = getBalance(root);

                // Left Left Case
                if (balance > 1 && compare(p, root.left.p) < 0)
                    return rotateRight(root);

                // Right Right Case
                if (balance < -1 && compare(p, root.right.p) > 0)
                    return rotateLeft(root);

                // Left Right Case
                if (balance > 1 && compare(p, root.left.p) > 0)
                {
                    root.left = rotateLeft(root.left);
                    return rotateRight(root);
                }

                // Right Left Case
                if (balance < -1 && compare(p, root.right.p) < 0)
                {
                    root.right = rotateRight(root.right);
                    return rotateLeft(root);
                }

                // Return unchanged root if no rotations were performed
                return root;
            }
            private Node erase(Point p, Node root)
            {
                if (root == null)
                    return root;
                int cmp = compare(p, root.p);
                if (cmp == 0)
                {
                    if (root.left != null && root.right != null)
                    {
                        Node successor = GetSuccessor(root);
                        root.p = successor.p;
                        root.right = erase(root.p, root.right);
                    }
                    else
                    {
                        if (root.left == null)
                            root = root.right;
                        else
                            root = root.left;
                        size--;
                    }
                }
                else if (cmp == -1)
                    root.left = erase(p, root.left);
                else
                    root.right = erase(p, root.right);
                return balance(root); ;
            }
            public void insert(Point p)
            {
                this.root = insert(p, this.root);
            }
            public void erase(Point p)
            {
                this.root = erase(p, this.root);
            }
            public Node next(Node root, Point p,bool equal=false)
            {
                Node it = this.root, ans = null;
                int cmp = 0;
                while (it != null)
                {
                    cmp = compare(p, it.p);
                    if (cmp == -1)
                    {
                        ans = it;
                        it = it.left;
                    }
                    else if(equal && cmp == 0)
                    {
                        return it;
                    }
                    else
                    {
                        it = it.right;
                    }
                }
                return ans;
            }
            public Node prev(Node root, Point p)
            {
                Node it = this.root, ans = null;
                int cmp = 0;
                while (it != null)
                {
                    cmp = compare(p, it.p);
                    // p <= curr 
                    if (cmp != 1)
                        it = it.left;
                    // p > curr ;
                    else
                    {
                        ans = it;
                        it = it.right;
                    }
                }
                return ans;
            }

            public Node next(Node node)
            {
                return next(this.root, node.p);
            }
            public Node prev(Node node)
            {
                if (node == null)
                    return end;
                return prev(this.root, node.p);
            }
            public Node next(Point p)
            {
                return next(this.root, p);
            }
            public Node lower_bound(Point p)
            {
                return next(this.root, p,true);
            }
            public void Add(Point p)
            {
                if (size < 2)
                {
                    insert(p);
                    return;
                }
                Node nxt1 = lower_bound(p);
                // exist
                if (nxt1 != null && nxt1.p.X == p.X && nxt1.p.Y == p.Y)
                    return;
                Node prev1 = prev(nxt1);
                Debug.Assert(prev1 != null);
                if (nxt1 != null && CheckTurn(nxt1.p - prev1.p, p - nxt1.p) != Enums.TurnType.Left)
                    return;
                Node prev2;
                while (prev1 != null && (prev2 = prev(prev1)) != null && CheckTurn(prev1.p - p, prev2.p - prev1.p) != Enums.TurnType.Left)
                {
                    erase(prev1.p);
                    prev1 = prev2;
                }
                Node nxt2;
                while (nxt1 != null && (nxt2 = next(nxt1)) != null && CheckTurn(nxt1.p - p, nxt2.p - nxt1.p) != Enums.TurnType.Right)
                {
                    erase(nxt1.p);
                    nxt1 = nxt2;
                }
                insert(p);
            }
            // debug add
            //public void Add(Point p)
            //{
            //    Console.WriteLine($"Tro to Add Point: ({p.X}, {p.Y}) angle:({p.angle}) , distance: {p.distance}");

            //    if (size < 2)
            //    {
            //        Console.WriteLine($"Size < 2, inserting without checks.");
            //        insert(p);
            //        return;
            //    }

            //    Node nxt1 = lower_bound(p);
            //    Console.WriteLine($"Next Node after lower_bound: {nxt1?.p}");

            //    // exist
            //    if (nxt1 != null && nxt1.p.X == p.X && nxt1.p.Y == p.Y)
            //    {
            //        Console.WriteLine($"Point ({p.X}, {p.Y}) already exists.");
            //        return;
            //    }

            //    Node prev1 = prev(nxt1);
            //    Debug.Assert(prev1 != null, "prev1 should not be null here.");
            //    Console.WriteLine($"Previous Node: {prev1.p}");

            //    if (nxt1 != null && CheckTurn(nxt1.p - prev1.p, p - nxt1.p) != Enums.TurnType.Left)
            //    {
            //        Console.WriteLine($"Turn check failed between ({nxt1.p.X}, {nxt1.p.Y}) and ({prev1.p.X}, {prev1.p.Y})");
            //        return;
            //    }
            //    // Most right 
            //    Node prev2;
            //    while (prev1 != null && (prev2 = prev(prev1)) != null && CheckTurn(prev1.p - p, prev2.p - prev1.p) != Enums.TurnType.Left)
            //    {
            //        Console.WriteLine($"Removing point due to Most right turn check: ({prev1.p.X}, {prev1.p.Y})");
            //        erase(prev1.p);
            //        prev1 = prev2;
            //    }
            //    // Most Left  
            //    Node nxt2;
            //    while (nxt1 != null && (nxt2 = next(nxt1)) != null && CheckTurn(nxt1.p - p, nxt2.p - nxt1.p) != Enums.TurnType.Right)
            //    {
            //        Console.WriteLine($"Removing point due to Most Left turn check: ({p.X}, {p.Y})->({nxt1.p.X}, {nxt1.p.Y}) -> ({nxt2.p.X}, {nxt2.p.Y})");
            //        erase(nxt1.p);
            //        nxt1 = nxt2;
            //    }

            //    Console.WriteLine($"Inserting Point: ({p.X}, {p.Y})");
            //    insert(p);
            //}

            public void traverse(ref List<Point> outputs, Node root)
            {
                if (root == null)
                    return;
                traverse(ref outputs, root.left);
                outputs.Add(root.p);
                traverse(ref outputs, root.right);
            }
            public void traverse(ref List<Point> outputs)
            {
                traverse(ref outputs, root);
            }
        }
        public override void Run(List<Point> points, List<Line> lines, List<Polygon> polygons, ref List<Point> outPoints, ref List<Line> outLines, ref List<Polygon> outPolygons)
        {
            //Incremental_convex convexHull = new Incremental_convex();
            //List<Point> traversalOutput = new List<Point>();

            //// Sample queries (1 means insert, 0 means delete)
            //var queries = new List<Tuple<int, int, int>>
            //{
            //    Tuple.Create(1, 0, 0),   
            //    Tuple.Create(1, 0, 3),   
            //    Tuple.Create(1, 2, 0),
            //    Tuple.Create(1, 3, 2),
            //    Tuple.Create(1, 0, 1),   
            //    Tuple.Create(1, 2, 0),    
            //    Tuple.Create(1, 3, 1),
            //    Tuple.Create(0, 0, 0),
            //    Tuple.Create(0, 0, 3),
            //    Tuple.Create(0, 2, 0),
            //    Tuple.Create(0, 3, 2),
            //    Tuple.Create(0, 0, 1),
            //    Tuple.Create(0, 2, 0),
            //    Tuple.Create(0, 3, 1)
            //};
            //Point refP = new Point(0, 0);
            //foreach (var query in queries)
            //{
            //    int type = query.Item1;
            //    int x = query.Item2;
            //    int y = query.Item3;
            //    {
            //        Point p = new Point(x, y);
            //        p.angle = GetAngle(p, refP);
            //        p.distance = GetDistance(p, refP);
            //        if (type == 1)
            //        {
            //            Console.WriteLine($"Inserting ({x}, {y})");
            //            convexHull.insert(p);
            //        }
            //        else if (type == 0)
            //        {
            //            Console.WriteLine($"Deleting ({x}, {y})");
            //            convexHull.erase(p);
            //        }

            //        traversalOutput.Clear();
            //        convexHull.traverse(ref traversalOutput);
            //        Console.WriteLine("Current traversal:");
            //        foreach (var point in traversalOutput)
            //        {
            //            Console.WriteLine($"({point.X}, {point.Y}, {point.angle})");
            //        }
            //        Console.WriteLine();
            //    }
            //}
            //points = new List<Point>
            //{
            //    new Point(0, 0),  // A
            //    new Point(3, 0),  // B
            //    new Point(2, 0),  // C
            //    new Point(1, 0),  // D
            //    new Point(0, 1),  // E
            //    new Point(0, 2),  // F
            //    new Point(1, 2),  // G
            //    new Point(2, 2),  // H
            //    new Point(3, 2),  // I
            //    new Point(3, 1)   // J
            //};
            Point MostBottomLeft = points[0];
            foreach (Point point in points)
            {
                if (point.Y < MostBottomLeft.Y || point.Y == MostBottomLeft.Y && point.X < MostBottomLeft.X)
                    MostBottomLeft = point;
            }
            foreach (Point point in points)
            {
                point.angle = HelperMethods.GetAngle(point, MostBottomLeft);
                point.distance = HelperMethods.GetDistance(point, MostBottomLeft);
            }
            Incremental_convex convex = new Incremental_convex();
            outPoints = new List<Point>();
            convex.Add(MostBottomLeft);
            foreach (Point p in points)
            {
                convex.Add(p);
                //outPoints.Clear();
                //Console.WriteLine("After: ");
                //convex.traverse(ref outPoints);
                //foreach (Point pi in outPoints)
                //    Console.WriteLine($"({pi.X}, {pi.Y}) angle:({pi.angle}) , distance: {pi.distance}");
                //Console.WriteLine();
            }
            convex.traverse(ref outPoints);
        }

        public override string ToString()
        {
            return "Convex Hull - Incremental";
        }
    }
}
