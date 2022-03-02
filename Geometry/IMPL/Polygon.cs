using GraphicEditor.Geometry.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicEditor.Geometry.IMPL
{

    internal class Polygon : IFigure
    {
        private Point center;   // Центр фигуры

        //Класс с типами фигур
        static class PolygType
        {
            public const byte Triangle = 1; // Треугольник
            public const byte Square = 2; // Квадрат
            public const byte Rectangle = 3; // Прямоугольник
            public const byte Polygon = 4; // Полигон
        }

        /* Тип отражения */
        static class ReflectType
        {
            public const byte Horizon = 1; // По горизонтали
            public const byte Vertical = 2; // По вертикали
        }

        private List<LineParam> lineParams;     // Параметры прямых

        /* Переменные для оптимизации вычислений*/
        private int indMaxXP;  //Индекс самой левой точки
        private int indMinXP;  //Индекс самой правой точки
        private int indMaxYP;  //Индекс самой верхней точки
        private int indMinYP;  //Индекс самой нижней точки
        public Point lup { get; private set; }      // Левая верхняя точка описывающего прямоугольника
        public Point rdp { get; private set; }        // Правая нижняя точка описывающего многоугольника

        public Point Сenter { get; set; }
        public double eps {get; set;}

        private List<Point> points {get; set;} // Точки на границе фигуры

        public List<Point> getFigurePoints() { return points; }

        public Point actionCenter { get; set ; }

        /// <summary>
        /// Конструктор, инициализирующий многоугольник в соответствии с передаваемыми точками и типом фигуры
        /// </summary>
        /// <param name="basePoints"> Массив точек по которым строиться многоугольник </param>
        /// <param name="polygType"> Код, указывающий какой тип многоугольника должен быть построен. Коды типов многоугольников указаны в PolygType</param>
        /// <exception cref="Exception"></exception>
        Polygon(List<Point> basePoints, byte polygType)
        {
            /* Проверяем соответсвие количества передаваемых точек с необходимым количеством точек для построения заданной фигуры */
            if (basePoints.Count() != getMinPoints(polygType))
                throw new Exception("Incorrect number of points to create figure");
            int i, j;
            int listLen = basePoints.Count();

            /* Проверяем, что среди передаваемых точек нет, учитывая заданную точность, неразличимо близких друг к другу */
            for (i = 0; i < listLen; i++)
                for (j = i + 1; j < listLen; j++)
                {
                    if (Math.Pow(basePoints[i].x - basePoints[j].x, 2) + Math.Pow(basePoints[i].y - basePoints[j].y, 2) < eps)
                        throw new Exception("There are two equal or too close to each other points");
                }

            switch (polygType)
            {
                /* Строим треугольник */
                case PolygType.Triangle:
                    points = new List<Point>(basePoints);
                    break;

                /* Строим квадрат */
                case PolygType.Square:

                    /* Проверяем, что стороны построенного по точкам прямоугольника будут равны, 
                     * т.е. что построенным прямоугольник будет квадратом*/
                    if (Math.Abs(basePoints[0].x - basePoints[1].x) - Math.Abs(basePoints[0].y - basePoints[1].y) > eps)
                        throw new Exception("Square can't be build using given points");

                    /* Заполняем массив базисных точек по которым будет строиться квадрат */
                    if (basePoints[0].y > basePoints[1].y && basePoints[0].x < basePoints[1].x || 
                       basePoints[0].y < basePoints[1].y && basePoints[0].x > basePoints[1].x)
                        points = new List<Point>() { new Point(basePoints[0].x, basePoints[0].y),
                                                     new Point(basePoints[0].x, basePoints[1].y),
                                                     new Point(basePoints[1].x, basePoints[1].y),
                                                     new Point(basePoints[1].x, basePoints[0].y)};
                    else
                        points = new List<Point>() { new Point(basePoints[0].x, basePoints[0].y),
                                                     new Point(basePoints[1].x, basePoints[0].y),
                                                     new Point(basePoints[1].x, basePoints[1].y),
                                                     new Point(basePoints[0].x, basePoints[1].y)};
                    
                    
                    break;

                /* Строим прямоугольник */
                case PolygType.Rectangle:

                    /* Проверяем, что передаваемые точки лежат на диагонали прямоугольника и по ним можно построить сам прямоугольник */
                    if(Math.Abs(basePoints[0].x - basePoints[1].x) < eps && Math.Abs(basePoints[0].y - basePoints[1].y) < eps)
                        throw new Exception("Rectangle can't be build using given points");
                    
                    /* Заполняем массив базисных точек по которым будет строиться прямоугольник */
                    if (basePoints[0].y > basePoints[1].y && basePoints[0].x < basePoints[1].x ||
                       basePoints[0].y < basePoints[1].y && basePoints[0].x > basePoints[1].x)
                        points = new List<Point>() { new Point(basePoints[0].x, basePoints[0].y),
                                                     new Point(basePoints[0].x, basePoints[1].y),
                                                     new Point(basePoints[1].x, basePoints[1].y),
                                                     new Point(basePoints[1].x, basePoints[0].y)};
                    else
                        points = new List<Point>() { new Point(basePoints[0].x, basePoints[0].y),
                                                     new Point(basePoints[1].x, basePoints[0].y),
                                                     new Point(basePoints[1].x, basePoints[1].y),
                                                     new Point(basePoints[0].x, basePoints[1].y)};
                    break;

                /* Строим многоугольник */
                case PolygType.Polygon:
                    points = new List<Point>(basePoints);
                    break;

                default: throw new Exception("Wrong code of figure type");
            }
        }

        public void move(Point prevP, Point CurP)
        {
            /* Перемещения точки prevP в точку CurP по оси x и y соответственно */
            double dx = CurP.x - prevP.x, dy = CurP.y - prevP.y; 
            
            /* Перемещаем все точки, по которым строиться многогранник в соответвствии с расчитанным вектором перемещения*/
            for (int i = 0; i < points.Count; i++)
                points[i].Move(dx, dy);

            countRDPandLUP();
            center.Move(dx, dy); // Перемещаем центр
        }

        public void scale(Point prevP, Point CurP)
        {
            /* Коэффициенты растяжения по осям x и y соответственно */
            double lx = (CurP.x - center.x)/(prevP.x - center.x), ly = (CurP.y - center.y) / (prevP.x - center.x);

            /* Перемещаем все точки, по которым строиться многогранник в соответвствии с растяжением*/
            for (int i = 0; i < points.Count; i++)
                points[i].MoveByScale(center, lx, ly);

            countRDPandLUP();
            updateLineParams();
        }


        public void rotate(Double angle)
        {
            /* Перемещаем все точки, по которым строиться многогранник 
             * в соответвствии с поворотом относительно установленного центра */
            for (int i = 0; i < points.Count; i++)
                points[i].Rotate(center, angle);

            countMinMaxP();
            updateLineParams();
        }

        public void reflection(Point p)
        {
            for (int i = 0; i < points.Count; i++)
                points[i].Reflection(center);

            countMinMaxP();
            updateLineParams();
        }

        public void reflection(Point p1, Point p2)
        {
            for (int i = 0; i < points.Count; i++)
                points[i].Reflection(new LineParam(p1, p2));

            countMinMaxP();
            updateLineParams();

        }

        public void reflection(Byte refType)
        {
            switch (refType)
            {
                /* Отражение по горизонтали */
                case ReflectType.Horizon:
                    for (int i = 0; i < points.Count; i++)
                        points[i].x = center.x + (center.x - points[i].x);
                    break;

                /* Отражение по вертикали */
                case ReflectType.Vertical:
                    for (int i = 0; i < points.Count; i++)
                        points[i].y = center.y + (center.y - points[i].y);
                    break;

                default: throw new Exception("Wrong code of reflection type");
            }
        }

        public Boolean isIn(Point p, Double eps)
        {
            /* Проверяем попала ли точка в описывающий прямоугольник*/
            if (lup.x - p.x > eps || p.y - lup.y > eps || p.x - rdp.x > eps || rdp.y - p.y > eps)
                return false;
            int indLastPoint = points.Count() - 1; // Последний индекс в массиве

            Point pGreaterMaxX = new Point(p.x + 1.2*(rdp.x - p.x), p.y);
            LineParam LineParalXFromP = new LineParam(p, pGreaterMaxX);
            int i;
            /*Ищем отрезок, область аргумента x которого захватыает координату x интересующей точки*/
            for (i = indMinXP; points[i].x < p.x; i = (i == indLastPoint ? 0 : i + 1));

            int crossNum = 0; //Количество пересечений луча, проведённого из указанной точки паралельно оси x в сторону возрастания аргумента
            for (; points[i].x < p.x; i = (i == indLastPoint ? 0 : i + 1))
            {
                //Проверяем принадлежность точки пересечения построенного луча с прямыми, описывающими многогранник, к отрезкам этот многогранник образующим
                // Посокльку фигура замкнутая, то при движении по ней мы должны не выходить за границу массива, т.е. просматривать его циклично
                if(i == 0)
                    if (points[i].y < p.y && points[indLastPoint].y > p.y || points[i].y > p.y && points[indLastPoint].y < p.y)
                        crossNum++;
                else
                    if (points[i].y < p.y && points[i - 1].y > p.y || points[i].y > p.y && points[i - 1].y < p.y)
                        crossNum++;
            }
            if (crossNum % 2 == 1)
                return true;
            else
                return false;
        }

        public Int32 getMinPoints(Byte figureType)
        {
            switch (figureType)
            {
                /*  треугольник */
                case PolygType.Triangle: return 3;

                /* Строим квадрат */
                case PolygType.Square: return 2;

                /* Строим прямоугольник */
                case PolygType.Rectangle: return 2;

                /* Строим многоугольник */
                case PolygType.Polygon: return 2;

                default: throw new Exception("Wrong code of figure type");
            }
        }

        private void updateLineParams()
        {
            int i;
            for (i = 0; i < points.Count - 1; i++)
                lineParams[i].calcParam(points[i], points[i + 1]);

            lineParams[i].calcParam(points[i], points[0]);
        }

        public bool addPoint(Point newP)
        {
            Point cross; // Точка пересечения линии многоугольника и достраиваемой линии
            int indLastP = points.Count() - 1;

            LineParam newLine = new LineParam(points[indLastP], newP); // Параметры достраиваемой линии
            for (int i = 0; i < indLastP; i++)
            {
                cross = LineParam.findCrossPoint(lineParams[i], newLine);
                if (cross.x > points[i].x && cross.x < points[i + 1].x || cross.x < points[i].x && cross.x > points[i + 1].x) 
                      if (cross.y > points[i].y && cross.y < points[i + 1].y || cross.y < points[i].y && cross.y > points[i + 1].y)
                        if (cross.x > points[indLastP].x && cross.x < newP.x || cross.x < points[indLastP].x && cross.x > newP.x)
                            if (cross.y > points[indLastP].y && cross.y < newP.y || cross.y < points[indLastP].y && cross.y > newP.y)
                                return false;
            }
            points.Add(newP);
            lineParams.Add(newLine);

            countMinMaxP(); // Потом надо бужет заменить функцию получше! 

            return true;
        }

        private void countMinMaxP()
        {
            double minX = points[0].x, 
                   maxX = points[0].x, 
                   minY = points[0].y, 
                   maxY = points[0].y;

            for (int i = 1; i < points.Count; i++)
            {
                if(points[i].x < minX)
                {
                    minX = points[i].x;
                    indMinXP = i;
                }
                
                if (points[i].y < minY)
                {
                    minY = points[i].y;
                    indMinYP = i;
                }
                    
                if (points[i].x > maxX)
                {
                    maxX = points[i].x;
                    indMaxXP = i;
                }
                    
                if (points[i].y > maxY)
                {
                    maxY = points[i].y;
                    indMaxYP = i;
                }              
            }

            lup.x = minX;
            lup.y = maxY;

            rdp.x = maxX;
            rdp.y = minY;
        }

        private void countRDPandLUP()
        {
            lup.x = points[indMinXP].x;
            lup.y = points[indMaxYP].y;

            rdp.y = points[indMinYP].y;
            rdp.x = points[indMaxXP].x;
        }

    }
}
