using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGUtilities
{
    public class Node<T>
    {
        public Node(Node<T> left, Node<T> right, int height, T value)
        {
            this.left = left;
            this.right = right;
            this.height = height;
            this.value = value;
        }

        public Node<T> left, right;
        public int height, balance;
        public T value;
    }

    public class AVLTree<T>
    {
        private Node<T> root, begin, end;
        public int size = 0;
        private readonly IComparer<T> comparer;
        public AVLTree()
        {
            this.comparer = Comparer<T>.Default;
        }
        public AVLTree(IComparer<T> comparer)
        {
            this.comparer = comparer;
        }

        private int getHeight(Node<T> node)
        {
            if (node == null) return 0;
            return node.height;
        }

        private int getBalance(Node<T> node)
        {
            return node == null ? 0 : getHeight(node.left) - getHeight(node.right);
        }

        private Node<T> rotateRight(Node<T> N)
        {
            if (N == null || N.left == null) return N;

            //     N
            //   p    a
            // b   c
            Node<T> p = N.left;
            N.left = p.right;
            p.right = N;

            N.height = Math.Max(getHeight(N.left), getHeight(N.right)) + 1;
            p.height = Math.Max(getHeight(p.left), getHeight(p.right)) + 1;

            return p;
        }

        private Node<T> rotateLeft(Node<T> N)
        {
            if (N == null || N.right == null) return N;

            //       N
            //    a     p      
            //        b   c   
            Node<T> p = N.right;
            N.right = p.left;
            p.left = N;

            N.height = Math.Max(getHeight(N.left), getHeight(N.right)) + 1;
            p.height = Math.Max(getHeight(p.left), getHeight(p.right)) + 1;

            return p;
        }

        private Node<T> balance(Node<T> root)
        {
            if (root == null) return root;

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

        private int compare(T p1, T p2)
        {
            return comparer.Compare(p1, p2);
        }

        private Node<T> insert(T value, Node<T> root)
        {
            if (root == null)
            {
                root = new Node<T>(null, null, 0, value);
                if (begin == null || compare(value, begin.value) < 0)
                    begin = root;
                if (end == null || compare(value, end.value) > 0)
                    end = root;
                size++;
                return root;
            }

            int cmp = compare(value, root.value);
            // root = value
            if (cmp == 0) return root;

            // value < root  
            if (cmp < 0)
                root.left = insert(value, root.left);
            else
                root.right = insert(value, root.right);

            return balance(root);
        }

        private Node<T> GetSuccessor(Node<T> curr)
        {
            curr = curr.right;
            while (curr != null && curr.left != null)
                curr = curr.left;
            return curr;
        }

        private Node<T> erase(T value, Node<T> root)
        {
            if (root == null) return root;

            int cmp = compare(value, root.value);
            if (cmp == 0)
            {
                if (root.left != null && root.right != null)
                {
                    Node<T> successor = GetSuccessor(root);
                    root.value = successor.value;
                    root.right = erase(root.value, root.right);
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
            else if (cmp < 0)
                root.left = erase(value, root.left);
            else
                root.right = erase(value, root.right);

            return balance(root);
        }

        public void insert(T value)
        {
            root = insert(value, root);
        }

        public void erase(T value)
        {
            root = erase(value, root);
        }

        public Node<T> next(T value)
        {
            return next(root, value);
        }

        private Node<T> next(Node<T> root, T value, bool equal = false)
        {
            Node<T> it = this.root, ans = null;
            while (it != null)
            {
                int cmp = compare(value, it.value);
                if (cmp < 0)
                {
                    ans = it;
                    it = it.left;
                }
                else if (equal && cmp == 0)
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

        public Node<T> lower_bound(T value)
        {
            return next(root, value, true);
        }

        public Node<T> prev(T value)
        {
            return prev(root, value);
        }

        private Node<T> prev(Node<T> root, T value)
        {
            Node<T> it = this.root, ans = null;
            while (it != null)
            {
                int cmp = compare(value, it.value);
                // value <= curr 
                if (cmp <= 0)
                    it = it.left;
                // value > curr
                else
                {
                    ans = it;
                    it = it.right;
                }
            }
            return ans;
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
        public void traverse(ref List<T> outputs, Node<T> root)
        {
            if (root == null)
                return;
            traverse(ref outputs, root.left);
            outputs.Add(root.value);
            traverse(ref outputs, root.right);
        }
        public void traverse(ref List<T> outputs)
        {
            traverse(ref outputs, root);
        }
    }
}
