using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicEditor.Geometry.API
{
    internal class LineParam
    {
        public double a { get; private set; } // Коэффициент при x в уравнении прямой
        public double b { get; private set; } // Свободный коэффициент в уравнении прямой

        /// <summary>
        /// Вычисляет параметры уравнения, задающего прямую линию, по 2 точкам 
        /// </summary>
        /// <param name="p1">Точка прямой</param>
        /// <param name="p2">Точка прямой</param>
        public void calcParam(Point p1, Point p2)
        {
            a = (p2.y - p1.y) / (p2.x - p1.x);
            b = (p1.y * p2.x - p2.y * p1.x) / (p2.x - p1.x);
        }

        /// <summary>
        /// Конструктор вычисляющий параметры уравнения прямой, по двум её точкам 
        /// </summary>
        /// <param name="p1">Точка прямой</param>
        /// <param name="p2">Точка прямой</param>
        public LineParam(Point p1, Point p2)
        {
            calcParam(p1, p2);
        }

        /// <summary>
        /// Конструктор, явно задающий параметры уравнения
        /// </summary>
        /// <param name="a"> Коэффициент при x</param>
        /// <param name="b"> Свободный коэффициент</param>
        public LineParam(double a, double b)
        {
            this.a = a; 
            this.b = b;
        }

        /// <summary>
        /// Находит точку пересечения двух линий
        /// </summary>
        /// <param name="line1">Параметры первой линии</param>
        /// <param name="line2">Параметры второй линии</param>
        /// <returns></returns>
        public static Point findCrossPoint(LineParam line1, LineParam line2)
        {
            return new Point((line2.b - line1.b) / (line1.a - line2.a),
                             (line1.a * line2.b - line2.a * line1.b) / (line1.a - line2.a));
        }
    }
}
