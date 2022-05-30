using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace AutoVDesktop.DesktopRestorer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DesktopPoint
    {
        public int X;
        public int Y;

        public DesktopPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public struct NamedDesktopPoint
    {
        public string Name;
        public int X;
        public int Y;

        public NamedDesktopPoint(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;
            NamedDesktopPoint other = (NamedDesktopPoint)obj;
            return Name.Equals(other.Name) && X == other.X && Y == other.Y;
        }

        public override int GetHashCode()
        {
            return (Name + X.ToString() + Y.ToString()).GetHashCode();
        }

        public static bool operator ==(NamedDesktopPoint left, NamedDesktopPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(NamedDesktopPoint left, NamedDesktopPoint right)
        {
            return !(left == right);
        }
    }
}
