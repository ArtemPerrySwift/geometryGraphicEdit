using Sloths.source.model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;

namespace Sloths.source.math
{
    //[DataContract]
    //[KnownType(typeof(Rectangle))]
    // На формирование прямоугольника приходит от пользователя две точки
    // Первая точка верхняя левая
    // Вторая точка нижняя правая
    class Rectangle : IFigure
    {
        public List<NormPoint> points {
            get
            { 
                points.Add(points[1]);
                NormPoint point = new NormPoint();
                point.x = points[1].x;
                point.y = point[0].y;
                point[1] = point;
                point = new NormPoint();
                point.x = points[0].x;
                point.y = point[2].y;
                points.Add(point);
            }; set;
             }

        public Rectangle()
        {
            points = new List<NormPoint>();
        }

        public void AddPoint(NormPoint point)
        {
            if (points.Count < 4)
            {
                points.Add(point);
            }
            else
            {
                "Ошибку"
            }
        }

        public int CountOfPointsForCreateFigure()
        {
            return 2;
        }

        
        
    }
}
