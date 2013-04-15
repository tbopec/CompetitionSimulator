using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Thornado;
using AIRLab.Mathematics;
using System.Windows.Forms;

namespace Eurosim.Core
{
    [Serializable]
    public class DoubleWheelMovement : IPlaneMovement
    {
        //скорости в начальный момент, конечный момент и время
        [Thornado]
        public double VLeft0 { get; set; }
        [Thornado]
        public double VLeft1 { get; set; }
        [Thornado]
        public double VRight0 { get; set; }
        [Thornado]
        public double VRight1 { get; set; }
        [Thornado]
        public double TotalTime { get; set; }
        [Thornado]
        public double DistanceWheels = 26.5;
        [Thornado]
        public string Comment;

        //этот метод работает непрвильно
        public DoubleWheelMovement Revert()
        {
            return new DoubleWheelMovement() {
                Comment = Comment, 
                DistanceWheels = DistanceWheels, 
                TotalTime = TotalTime, 
                VLeft0 = -VLeft0, 
                VLeft1 = -VLeft1, 
                VRight0 = -VRight0, 
                VRight1 = -VRight1 
            };
        }
        public Frame2D GetOffset(double startTime, double dtime)
        {
            if (dtime < 0.000000001) //Если время ну очень маленькое, то  изменение не произошло
            {
                return new Frame2D(0, 0, Angle.Zero);
            }
            
            #region Пересчитываем скорости для нашего интервала времени
            //ULeft,=/=1,uRight,=/=1 пересчитали скорости в моменты времени start time and starttime+dtime
            double ULeft = VLeft0 + startTime * (VLeft1 - VLeft0) / TotalTime;
            double URight0 = VRight0 + startTime * (VRight1 - VRight0) / TotalTime;
            double ULeft1 = VLeft0 + (startTime + dtime) * (VLeft1 - VLeft0) / TotalTime;
            double URight1 = VRight0 + (startTime + dtime) * (VRight1 - VRight0) / TotalTime;
            #endregion
            //ALeft and ARight линейное ускорение на левом и правом колесе
            double ALeft = (ULeft1 - ULeft) / dtime;
            double ARight = (URight1 - URight0) / dtime;

            //Суммарная скорость и ускорение центра робота
            double AForvard = (ARight + ALeft) / 2; // Поступательное ускорение, обозначим A
            double UForvard0 = (ULeft + URight0) / 2; // Начальная скорость поступательного движения, обозначим B

            if (URight0 == ULeft && ULeft1 == URight1)
                //Движение по прямой  <=> когда скорости на левом и правом колесе в начальный и конечный момент времени равны          
                return new Frame2D(AForvard * dtime * dtime / 2 + dtime * UForvard0, 0, Angle.Zero);
            else
            {
                //Движение по окружности <=> начальные и конечные скорости  равны между собой
                if (URight0 == URight1 && ULeft == ULeft1) //движение по окружности угловово ускорениея нет
                    return moveOnCircle(ULeft, URight0, DistanceWheels, dtime);
                else //Движение по спирали
                {
                    //Иначе будет движение по спирали
                    return moveOnSpiral(startTime, startTime + dtime, 100, ULeft, URight0, ALeft, ARight, DistanceWheels);
                }
            }
            
            
        }
        public override string ToString()
        {
            return String.Format("mov{0}.{1}.{2}.{3}.{4};",TotalTime,VLeft0,VRight0,VLeft1,VRight1);
        }
        private static Point2D getDoteRote(double vl, double vr, double d,double dt)
        {
            //Функция вычисляющая точку поворота
            // В точке (о,0) находится центр робота. Его колеса находятся (-d/2,0) and (d/2,0). 
            // Еще начертим точки (-d/2,vl) and (d/2,vr) Через них проведем прямую, где пересечение с осью ох и будет центром вращения робота
            if (vl == vr)
                return new Point2D();
            Line2D oX = new Line2D(new Point2D(0, 0), new Point2D(1, 0));//Ось ох
            Line2D vlVr = new Line2D(new Point2D(-d / 2, vl), new Point2D(d / 2, vr));// Прямая "противодействия" скоростей
            return AIRLab.Mathematics.Angem.GetCrossing(oX, vlVr);
           
        }
        public static Frame2D moveOnCircle(double ULeft, double URight0, double DistanceWheels, double dtime)
        {
            //Получаем центр вращения робота DoteCrossing
            Point2D DoteCrossing = getDoteRote(ULeft, URight0, DistanceWheels, dtime);
            //Радиус вращения робота относительно центра вращения и двигателя с наибольшей скоростью(Который и совершает вращение)
            double DugaRote = DistanceWheels / 2 + Math.Abs(DoteCrossing.X);
            // R*a=vl*dt
            double X, Y;
            Angle angleRote;
            if (ULeft!= URight0) 
            {
                //Если скорости на левом и правом колесе не равны, то робот будет двигаться по окружности

                //Угол поворота робота, высчитываем из формулы 2*pi*R*angleRote/2pi= R*angleRote=speed*time => angleRote= speed*time/R
                angleRote = (Math.Abs(ULeft) > Math.Abs(URight0)) ? Angle.FromRad(-ULeft * dtime / DugaRote) : Angle.FromRad(URight0 * dtime / DugaRote);
                //Тк мы знаем что робот повернул по окружности на угол angleRote, то воспользуемся Полярные координатами и переведем их в декартову систему исчисления
                X = -DoteCrossing.X * Math.Cos(angleRote.Radian - Math.PI / 2);
                Y = DoteCrossing.X * Math.Sin(angleRote.Radian - Math.PI / 2) + DoteCrossing.X;
            }
            else 
            {
                // Вдруг все таки скорости равны то робот поедет по окружности
                angleRote = Angle.Zero;
                X = dtime * ULeft;
                Y = 0;
            }
            return new Frame2D(X, Y,angleRote);
        }
        public static Frame2D moveOnSpiral(double t1, double t2, int n, double vl, double vr, double al, double ar, double d)
        {
            //Движение по спирали разобьем на маленькие части и применим n раз метод движения по окружности

            //Текущее положение
            var currentPlace = new Frame2D();
            //Шаг
            double dt = (t2 - t1) / n;
            //Скорости на n-ом шаге
            double vcurL = vl, vcurR = vr;
            for (int i = 0; i < n; i++)
            {
                //Пересчитываем скорости на n-ом шаге
                vcurL=vl+dt*i*al;
                vcurR=vr+dt*i*ar;
                //Изменения положения центра робота на n-ом шаге
                var changeFrame = moveOnCircle(vcurL, vcurR, d, dt);
                //Нахождение его фактической позиции относительно (0.0) в 
                currentPlace= currentPlace.Apply(changeFrame);
            }
            return currentPlace;
        }
    }
}
