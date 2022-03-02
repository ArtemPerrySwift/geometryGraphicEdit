using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicEditor.Geometry.API
{
    class Point
    {
        // Координаты точки
        public double x { get; set; }
        public double y { get; set; }
                
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Поворачивает точку на угол angle относительно точки center
        /// </summary>
        /// <param name="center">Точка, относительно которой поворачиваеится данная</param>
        /// <param name="angle">Угол, на который поварачивается данная точка</param>
        public void Rotate(Point center, double angle)
        {
            x = center.x + (x - center.x) * Math.Cos(angle) - (y - center.y) * Math.Sin(angle);
            y = center.y + (x - center.x) * Math.Sin(angle) + (y - center.y) * Math.Cos(angle);
            //return new Point(center.x + (x - center.x) * Math.Cos(angle) - (y - center.y) * Math.Sin(angle), center.y + (x - center.x) * Math.Sin(angle) + (y - center.y) * Math.Cos(angle));
        }

        /// <summary>
        /// Отражает точку относительно той, что указна в параметрах
        /// </summary>
        /// <param name="center">Точка, относительно которой производиться отражение</param>
        public void Reflection(Point center)
        {
            x = center.x + (center.x - x);
            y = center.y + (center.y - y);
            //return new Point(center.x + center.x - x, center.y  + center.y - y);
        }

        public void Reflection(LineParam line)
        {
            Point cross = LineParam.findCrossPoint(line, new LineParam(1 / line.a, y - x / line.a));
            x += 2 * (x - cross.x);
            y += 2 * (y - cross.y);
        }

        /// <summary>
        /// Перемещает точку, растягивая проекции вектора, проведённого из точки center в данную, в соответствии с коэффициентами lambdaX и lambdaY
        /// </summary>
        /// <param name="center">Точка, отночсительно которой производиться растяжение</param>
        /// <param name="lambdaX">Коэффициент растяжения проекции вектора на ось x, проведённого из точки center в данную</param>
        /// <param name="lambdaY">Коэффициент растяжения проекции вектора на ось y, проведённого из точки center в данную</param>
        public void MoveByScale(Point center, double lambdaX, double lambdaY)
        {
            x = center.x + (x - center.x) * lambdaX;
            y = center.y + (y - center.y) * lambdaY;
            //return new Point(center.x + (x - center.x)*lambdaX, y + (y - center.y)*lambdaY);
        }

        /// <summary>
        /// Перемещение точки на вдоль вектора (dx, dy)
        /// </summary>
        /// <param name="dx">Перемещение точки по оси абцисс</param>
        /// <param name="dy">Перемещение точки по оси ординат</param>
        public void Move(double dx, double dy)
        {
            x += dx;
            y += dy;
        }
    }
}
