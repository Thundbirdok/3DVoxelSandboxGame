using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public partial class VoronoiDiagram
{
	public class Point
	{
		public double x, y;

		public Point()
		{
		}

		public void setPoint(double x, double y)
		{
			this.x = x;
			this.y = y;
		}
	}

	// use for sites and vertecies
	public class Site
	{
		public Point coord;
		public int sitenbr;

		public Site()
		{
			coord = new Point();
		}
	}

	public class Edge
	{
		public double a = 0, b = 0, c = 0;
		public Site[] ep;
		public Site[] reg;
		public int edgenbr;

		public Edge()
		{
			ep = new Site[2];
			reg = new Site[2];
		}
	}


	public class Halfedge
	{
		public Halfedge ELleft, ELright;
		public Edge ELedge;
		public bool deleted;
		public int ELpm;
		public Site vertex;
		public double ystar;
		public Halfedge PQnext;

		public Halfedge()
		{
			PQnext = null;
		}
	}

	public class GraphEdge
	{
		public double x1, y1, x2, y2;
		public int site1, site2;
	}
	
	public class SiteSorterYX : IComparer<Site>
	{
		public int Compare(Site p1, Site p2)
		{
			Point s1 = p1.coord;
			Point s2 = p2.coord;
			if (s1.y < s2.y) return -1;
			if (s1.y > s2.y) return 1;
			if (s1.x < s2.x) return -1;
			if (s1.x > s2.x) return 1;
			return 0;
		}
	}
	
}
