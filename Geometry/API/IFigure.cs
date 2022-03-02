using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicEditor.Geometry.API
{
    interface IFigure
    {
        
        void move(Point prevP, Point CurP);

        void scale(Point prevP, Point CurP);

        void rotate(double angle);

        void reflection(Point p);

        void reflection(Point p1, Point p2);

        void reflection(byte refType);

        bool isIn(Point p, double eps);

        List<Point> p { get; }

        double actionCenter { get; set; }

        int getMinPoints(byte figureType);
    }
}
