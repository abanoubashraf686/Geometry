using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGUtilities
{
    /// <summary>
    /// The primary Point structure to be used in the CG project.
    /// </summary>
    public class Point : ICloneable
    {
        /// <summary>
        /// Creates a point structure with the given coordinates.
        /// </summary>
        /// <param name="x">The X value/</param>
        /// <param name="y">The Y value.</param>
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        /// <summary>
        /// Gets or sets the X coordinate.
        /// </summary>
        public double X
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y coordinate.
        /// </summary>
        public double Y
        {
            get;
            set;
        }
        public double angle
        {
            get;
            set;
        }
        public double distance
        {
            get;
            set;
        }

        public static Point Identity { get { return new Point(0, 0); } }
        public override bool Equals(object obj)
        {
            if (obj is Point)
            {
                Point p = (Point)obj;

                return Math.Abs(this.X - p.X) < Constants.Epsilon && Math.Abs(this.Y - p.Y) < Constants.Epsilon;
            }
            return false;
        }
        public static Point operator /(Point p, double d)
        {
            return new Point(p.X / d, p.Y / d);
        }
        public static Point operator *(Point p, double d)
        {
            return new Point(p.X * d, p.Y * d);
        }
        public Point Vector(Point to)
        {
            return new Point(to.X - this.X, to.Y - this.Y);
        }
        public double Magnitude()
        {
            return Math.Sqrt(this.X * this.X + this.Y * this.Y);
        }
        public Point Normalize()
        {
            double mag = this.Magnitude();
            Point ans = this / mag;
            return ans;
        }
        public static Point operator -(Point p1, Point p2)
        {
            return new Point(p1.X - p2.X, p1.Y - p2.Y);
        }
        /// <summary>
        /// Make a new instance of Point
        /// </summary>
        /// <returns>The new instance of Point</returns>
        public object Clone()
        {
            return new Point(X, Y);
        }
        public override String ToString()
        {
            return X + " " + Y;
        }
        // Compare based on X, then Y if X values are equal
        public int CompareTo(Point other)
        {
            if (other == null) return 1;

            // First compare by X coordinate
            int xComparison = X.CompareTo(other.X);

            if (xComparison != 0)
                return xComparison;

            // If X is equal, compare by Y coordinate
            return Y.CompareTo(other.Y);
        }

    }
}
