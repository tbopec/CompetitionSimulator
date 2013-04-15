using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AIRLab.Mathematics;
using Eurosim.Core;

namespace Eurosim.ChessUp
{
    partial class ChessUpRules
    {
        private void BonusScore(int robotNumber)
        {
            //Emulator.Scores.Add(new Score {Message = "Bonus Square", Value = 30, RobotNumber = robotNumber});
            Emulator.Scores.TempSum[robotNumber] += 30;
        }

        private void FigureScore(string name, int robotNumber)
        {
            var val = 0;
            if (name.Contains(ChessUpFigure.King)) val = 30;
            if (name.Contains(ChessUpFigure.Queen)) val = 20;
            if (name.Contains(ChessUpFigure.Pawn)) val = 10;
            if (name.Contains("Tower") && !name.Contains(ChessUpFigure.Pawn))
            {
                var str = name.Substring(name.Length - 1);
                val *= (int.Parse(str));
            }
            Emulator.Scores.TempSum[robotNumber] += val;
            //Emulator.Scores.Add(new Score {Message = name, Value = val, RobotNumber = robotNumber});
        }
        private void ApplyPenaltyScore(Robot robot, PenaltyType type)
        {
            Emulator.Scores.Penalties.Add(
                new Score {
                    Message = type + " Penalty", 
                    Permanent = true, 
                    Value = -10, 
                    Time =Emulator.LocalTime,
                    RobotNumber = robot.RobotNumber}
                );
        }

        private const double Timeout = 2;
        private readonly double[] LastRobotCollisionTime = new[] {-Timeout, -Timeout};
        private readonly double[] LastPawnCollisionTime = new[] {-Timeout, -Timeout};

        private enum PenaltyType{Collision,ProtectedArea,TowerOnGreen}

        private readonly List<Body> TowerList = new List<Body>();
        private readonly Dictionary<Body, Frame3D> ProtectedFigures = new Dictionary<Body, Frame3D>();

        public override void InitializeScores()
        {
            //Emulator.Scores=new ScoreCollection(2);
           /* for (var i = 0; i < 2; i++)
            {
                var x = new Score {Message = "Figures", Permanent = false, RobotNumber = i, Value = 0};
                TempFigureScore[i] = x;
                Emulator.Scores.Add(x);
            }*/
            foreach (var r in Emulator.Robots)
            {
                r.Collision += HandleFigureCollision;
                r.Collision += HandleTwoRobotCollision;
            }
        }

        private void HandleFigureCollision(IPhysicalBody r1, PhysicalPrimitiveBody body)
        {
            //Если фигура лежит в Protected Area, переместилась относительно прошлого положения и истек таймаут
            //то назначаем пенальи
            var robot = r1 as Robot;
            if (ProtectedFigures.ContainsKey(body) &&
                Angem.Hypot(body.Location - ProtectedFigures[body]) > 0.1 &&
                Emulator.LocalTime >= LastPawnCollisionTime[robot.RobotNumber] + Timeout
                && IsInProtectedArea(body.Location))
            {
                ApplyPenaltyScore(robot, PenaltyType.ProtectedArea);
                LastPawnCollisionTime[robot.RobotNumber] = Emulator.LocalTime;
            }
        }

        private void HandleTwoRobotCollision(IPhysicalBody r1, PhysicalPrimitiveBody r2)
        {
            /***
             * При столкновении двух роботов назначаем виновным того, у кого угол между скоростью и направлением
             * на противника меньше 90 градусов. Даем ему пенальти
             */
            var thisrobot = r1 as Robot;
            var other = r2.GetParents().FirstOrDefault(x => x is Robot) as Robot;
            if (other == null || thisrobot.RobotNumber == other.RobotNumber)
                return;
            var diffvec = (other.Location - thisrobot.Location).ToPoint3D();
            if (Angem.Hypot(thisrobot.Velocity) == 0)
                return;
            var ang = Angem.AngleBetweenVectors(thisrobot.Velocity.ToPoint3D(), diffvec);
            if (Emulator.LocalTime >= LastRobotCollisionTime[thisrobot.RobotNumber] + Timeout)
            {
                if (Math.Abs(ang.Radian) < Angle.HalfPi.Radian)
                    ApplyPenaltyScore(thisrobot, PenaltyType.Collision);
                LastRobotCollisionTime[thisrobot.RobotNumber] = Emulator.LocalTime;
            }
        }

        /// <summary>
        /// Подсчитать набранные за такт временные очки
        /// </summary>
        public override void AccountScores()
        {
            Emulator.Scores.ResetTemp();
            //Emulator.Scores.RemoveAll(x => !x.Permanent);
            /***
             * Проверяем если робот содержит башню, собранную на зеленом поле
             *(робот находится на зеленом и содержит башню, которой раньше не было в списке)
             **/
            foreach (var robot in Emulator.Robots)
                foreach (var body in robot.GetSubtreeChildrenFirst().OfType<ChessUpFigure>())
                {
                    if (IsInGreenArea(body) && body.Name.Contains("Tower") && !TowerList.Contains(body))
                    {
                        ApplyPenaltyScore(robot, PenaltyType.TowerOnGreen);
                        TowerList.Add(body);
                    }
                }
            //Проверяем если робот взял фигуру из Protected Area.
            foreach (var pfig in ProtectedFigures.Keys.ToList())
            {
                var rob = pfig.GetParents().FirstOrDefault(x => x is Robot) as Robot;
                if (rob != null)
                {
                    ApplyPenaltyScore(rob, PenaltyType.ProtectedArea);
                    ProtectedFigures.Remove(pfig);
                    break;
                }
                ProtectedFigures[pfig] = pfig.Location;
            }
            foreach (var body in Emulator.Objects)
            {
                var square = GetSquareNumber(body);
                if (square.X==InvalidSquare.X && square.Y==InvalidSquare.Y)
                    continue;
                var choice = GetRobotBySquare(square, body); //определяем номер робота
                if (IsProtectedSquare(square)) //в защищенных клетках тела считаются как пешки
                {
                    FigureScore(ChessUpFigure.Pawn, choice);
                    //содержим список тел в защищенной зоне
                    ProtectedFigures[body]=body.Location;
                    continue;
                }
                FigureScore(body.Name, choice); //добавляем очки
                if (IsBonusSquare(square)) //бонус если в бонусной клетке
                    BonusScore(choice);
                if (body.Name.Contains("Tower"))
                    TowerList.Add(body);
            }
        }

        /// <summary>
        /// Метод должен вызываться по завершению матча.
        /// Пенальти составляют по 20% от суммы очков, но не меньше 10.
        /// Исправляем их значения
        /// </summary>
        public void OnMatchEnd()
        {
            foreach (var robot in Emulator.Robots)
            {
                int n = robot.RobotNumber;
                double sum = Emulator.Scores.TempSum[n];
                if (sum*0.2 > 10)//по правилам пенальти 20% от суммы
                    foreach (var score in Emulator.Scores.Penalties)
                        score.Value = (int) (-sum/5);
            }
        }

        /// <summary>
        /// Возвращает номер квадрата, в котором лежит тела. Квадраты нумеруются от центра поля.
        /// Ближайшие к центру квадраты имеют номер 1. Самые дальние - 3
        /// </summary>
        /// <param name="body"></param>
        /// <returns>Если тело принадлежит квадрату, его номер.
        /// Иначе -InvalidSquare</returns>
        private static Point2D GetSquareNumber(Body body)
        {
            var loc = new Point2D(body.Location.X, body.Location.Y);
            if (Math.Abs(loc.X) > 105 || Math.Abs(loc.Y) > 105)
                return InvalidSquare;
            var square = new Point2D((int) (loc.X)/35 + Math.Sign(loc.X), (int) (loc.Y)/35 + Math.Sign(loc.Y));
            var posInSquare = new Point2D(Math.Abs(loc.X%35.0), Math.Abs(loc.Y%35.0));
            if (IsInSquare(posInSquare))
                return square;
            return InvalidSquare;
        }

        private static readonly Point2D InvalidSquare = new Point2D(-1, -1);

        /// <summary>
        /// По номеру квадрата возвращает номер робота, которому он принадлежит
        /// </summary>
        /// <param name="square"></param>
        /// <param name="body"></param>
        /// <returns>0 - красный (квадрат и робот)
        /// 1- синий</returns>
        private static int GetRobotBySquare(Point2D square, Body body)
        {
            var sign = Math.Sign(body.Location.X*body.Location.Y);
            var sum = square.X + square.Y;
            if (sum%2 == 0 && sign > 0 || (sum%2 != 0) && sign < 0) //red
                return 0;
            return 1;
        }

        private static bool IsInGreenArea(Body body)
        {
            return Math.Abs(body.GetAbsoluteLocation().X) > 120;
        }

        private static bool IsInProtectedArea(Frame3D loc)
        {
            return loc.Y < -82 && Math.Abs(loc.X) < 95 && Math.Abs(loc.X) > 45;
        }

        private static bool IsProtectedSquare(Point2D sq)
        {
            return sq.Y == -3 && Math.Abs(sq.X) > 1;
        }

        private static bool IsBonusSquare(Point2D sq)
        {
            return Math.Abs(sq.X) == 2 && (sq.Y == -1 || sq.Y == 2) || Math.Abs(sq.X) == 1 && sq.Y == -3;
        }

        private static bool IsInSquare(Point2D point)
        {
            const double eps = 0.5; //погрешность. максимальный выход за пределы квадрата
            return (Math.Abs(point.X - 17.5) < 7.5 + eps && Math.Abs(point.Y - 17.5) < 7.5 + eps);
        }
    }
}

