using GraphicEditor.Geometry.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicEditor.Geometry.IMPL
{
    internal class Ellipse : IFigure
    {
        public Point actionCenter { get; set ; }

        private double A_x { get; set; }
        private double A_y { get; set; }

        private double B_x { get; set; }
        private double B_y { get; set; }

        private double X_c { get; set; }
        private double Y_c { get; set; }

        /* Тип эллипса */
        static class EllipseType
        {
            public const byte Circle = 1; // Круг
            public const byte Ellipse = 2; // Эллипс
        }

        /* Тип отражения */
        static class ReflectType
        {
            public const byte Horizon = 1; // По горизонтали
            public const byte Vertical = 2; // По вертикали
        }

        public Ellipse(Point[] BasePoints, byte EllipseType)
        {
            //точки А(х1,у1) и В(х2,у2) - точки описанного вокруг эллипса прямоугольника
            if (EllipseType == 1) // если фигура круг, то меняем координаты прямоугольника таким образом, чтоб получался описанный квадрат
            {
                double dx = BasePoints[0].x - BasePoints[1].x; // x1 - x2
                double dy = BasePoints[0].y - BasePoints[1].y; // y1 - y2
                double length = 0; //длина меньшей стороны
                if (Math.Abs(dx) > Math.Abs(dy))//определение меньшей стороны описанного прямогульника
                {
                    length = Math.Abs(dy);//если наименьшая сторона вертикальная, то меняем координату x2
                    if (dx < 0) BasePoints[1].x = BasePoints[0].x + length; //если x1 слева, то x2 справа(получается сложением x1 и длины)
                    else BasePoints[1].x = BasePoints[0].x - length; //если x1 справа, то x2 слева(получается вычитанием длины от х1) 
                }
                else
                {
                    length = Math.Abs(dx);//если наименьшая сторона горизонтальная, то меняем координату y2
                    if (dy < 0) BasePoints[1].y = BasePoints[0].y + length; //если y1 снизу, то у2 сверху(получается сложением у1 и длины)
                    else BasePoints[1].y = BasePoints[0].y - length;//если y1 сверху, то у2 снизу(получается вычитанием длины от у2)
                }
            }

            X_c = (BasePoints[0].x + BasePoints[1].x) / 2;
            Y_c = (BasePoints[0].y + BasePoints[1].y) / 2;
            A_x = BasePoints[0].x - X_c;
            B_x = 0;
            A_y = 0;
            A_x = Y_c - BasePoints[0].y;
            actionCenter = new Point(X_c, Y_c);
            eps = 1e-5;
            acc = 1e-1;
        }

        public double eps { get; set; } // Точность определения эллипса (определения принадлежности точки к фигуре)
        public double acc { get; set; } // Точность представления эллипса

        public List<Point> getFigurePoints()
        {
            int nPoints = ((int)Math.Ceiling(2 * Math.PI/eps));
            List<Point> points = new List<Point>(nPoints);
            for(int i = 0; i < nPoints; i++)
                points[i] = new Point(countX(i * acc), countY(i * acc));
            
            return points;
        }

        public Int32 getMinPoints(Byte figureType)
        {
            switch (figureType)
            {
                /*  треугольник */
                case EllipseType.Circle: return 2;

                /* Строим квадрат */
                case EllipseType.Ellipse: return 2;

                default: throw new Exception("Wrong code of figure type");
            }
        }

        public Boolean isIn(Point p, Double eps)
        {
            double sum1 = Math.Pow(((p.x - X_c)/A_x - (p.y - Y_c)/A_y), 2)/Math.Pow((B_x/A_x - B_y/A_y), 2);
            double sum2 = Math.Pow(((p.x - X_c) / B_x - (p.y - Y_c) / B_y), 2) / Math.Pow((A_x / B_x - A_y / B_y), 2);
            if (1 - sum1 - sum2 <= eps)
                return true;
            return false;
        }

        public void move(Point prevP, Point CurP)
        {
            /* Перемещения точки prevP в точку CurP по оси x и y соответственно */
            double dx = CurP.x - prevP.x, dy = CurP.y - prevP.y;

            X_c += dx;
            Y_c += dy;
        }

        public void reflection(Point p)
        {
            X_c = 2 * p.x - X_c;
            A_x = -A_x;
            B_x = -B_x;

            Y_c = 2 * p.y - Y_c;
            A_y = -A_y;
            B_y = -B_y;

        }

        public void reflection(Point p1, Point p2)
        {
            if(Math.Abs(p1.x - p2.x) < eps)
            {
                X_c = 2 * p1.x - X_c;
                A_x = -A_x;
                B_x = -B_x;
                return;
            }

            double X_c_new, Y_c_new;
            double A_x_new, A_y_new;
            double B_x_new, B_y_new;

            double kX_c_new, kY_c_new;
            double kA_x_new, kA_y_new;
            double kB_x_new, kB_y_new;

            LineParam reflecLine = new LineParam(p1, p2);

            double a = reflecLine.a;
            double b = reflecLine.b;

            double del = 1 + a * a;
            kY_c_new = 2 * b / del;
            kA_y_new = 2 * a / del;
            kB_y_new = (a*a - 1) / del;

            kX_c_new = -a* kY_c_new;
            kA_x_new = -kB_y_new;
            kB_x_new = -kA_y_new * a * a;

            X_c_new = kX_c_new + kA_x_new * X_c + kB_x_new * Y_c;
            Y_c_new = kY_c_new + kA_y_new * X_c + kB_y_new * Y_c;

            A_x_new = kA_x_new * A_x + kB_x_new * A_y;
            A_y_new = kA_y_new * A_x + kB_y_new * A_y;

            B_x_new = kA_x_new * B_x + kB_x_new * B_y;
            B_y_new = kA_y_new * B_x + kB_y_new * B_y;

            X_c = X_c_new;
            Y_c = Y_c_new;
            A_x = A_x_new;
            B_x = B_x_new;
            A_y = A_y_new;
            B_y = B_y_new;

            throw new NotImplementedException();
        }

        public void reflection(Byte refType)
        {
            switch (refType)
            {
                /* Отражение по горизонтали */
                case ReflectType.Horizon:
                    Y_c = 2 * actionCenter.y - Y_c;
                    A_y = -A_y;
                    B_y = -B_y;
                    break;

                /* Отражение по вертикали */
                case ReflectType.Vertical:
                    X_c = 2 * actionCenter.x - X_c;
                    A_x = -A_x;
                    B_x = -B_x; 
                    break;

                default: throw new Exception("Wrong code of reflection type");
            }
        }

        public void rotate(Double angle)
        {
            double X_c_new, Y_c_new;
            double A_x_new, A_y_new;
            double B_x_new, B_y_new;

            X_c_new = actionCenter.x + (X_c - actionCenter.x) * Math.Cos(angle) + (Y_c - actionCenter.y) * Math.Sin(angle);
            Y_c_new = actionCenter.y + (X_c - actionCenter.x) * Math.Sin(angle) + (Y_c - actionCenter.y) * Math.Cos(angle);
            A_x_new = A_x * Math.Cos(angle) + A_y * Math.Sin(angle);
            A_y_new = A_x * Math.Sin(angle) + A_y * Math.Cos(angle);
            B_x_new = B_x * Math.Cos(angle) + B_y * Math.Sin(angle);
            B_y_new = B_x * Math.Sin(angle) + B_y * Math.Cos(angle);

            X_c = X_c_new;
            Y_c = Y_c_new;
            A_x = A_x_new;
            B_x = B_x_new;
            A_y = A_y_new;
            B_y = B_y_new;
        }

        public void scale(Point prevP, Point CurP)
        {
            /* Коэффициенты растяжения по осям x и y соответственно */
            double lx = (CurP.x - actionCenter.x) / (prevP.x - actionCenter.x), ly = (CurP.y - actionCenter.y) / (prevP.x - actionCenter.x);
            

            X_c = actionCenter.x + (X_c - actionCenter.x) * lx;
            Y_c = actionCenter.y + (Y_c - actionCenter.y) * ly;
            A_x *= lx;
            A_y *= ly;
            B_x *= lx;
            B_y *= ly;

            throw new NotImplementedException();
        }

        private double countX(double t)
        {
            return X_c + A_x * Math.Cos(t) + B_x * Math.Sin(t);
        }

        private double countY(double t)
        {
            return Y_c + A_y * Math.Cos(t) + B_y * Math.Sin(t);
        }

    }
}
