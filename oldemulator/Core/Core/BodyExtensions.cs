using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using AIRLab.Mathematics;

namespace Eurosim.Core
{
    public static class BodyExtensions
    {
        
        /// <summary>
        /// Изменить материальность тела (в тч PrimaryBody). Раковый метод. 
        /// Для башенок работает, в общем случае, скорее всего, нет.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="material"></param>
        public static void SetMaterial(this Body body, bool material)
        {
//            if (body is PhysicalPrimitiveBody)
//                (body as PhysicalPrimitiveBody).IsMaterial = material;
            if (body is PhysicalPrimitiveBody)
            {
                foreach (var part in body.Nested.OfType<PhysicalPrimitiveBody>())
                {
                    if (material == false || !Physics.PhysicalManager.Is2d)
                        part.IsMaterial = material;
                }
                if (material && Physics.PhysicalManager.Is2d)
					(body as PhysicalPrimitiveBody).PhysicalModel.IsMaterial = true;
            }
            else
                foreach (var part in body.Nested)
                    SetMaterial(part, material);
         
        }
       
        /// <summary>
        /// Телепортировать робота со всеми детьми. Нужен для ресета эмулятора.
        /// </summary>
        /// <param name="body"></param>
        /// <param name="newLocation"></param>
        public static void TeleportRobot(this Robot body, Frame3D newLocation)
        {
            body.Emulator.Robots.Remove(body);
            body.Location = newLocation;
            body.Emulator.Robots.Add(body);
        }

        /// <summary>
        /// Плавно поверуть тело изменяя pitch на Angle
        /// </summary>
        /// <param name="body"></param>
        /// <param name="center"></param>
        /// <param name="ang"></param>
        public static void PitchRotateBody(this Body body, Frame3D center, Angle ang)
        {
            new Thread(() =>
            {
                var dist = body.Location.X - center.X;
                for (var i = 0; i < 20; i++)
                {
                    var diff1 = new Frame3D(0, 0,
                        Math.Abs(dist) * Angem.Sin(ang / 20), ang / 20, Angle.Zero, Angle.Zero);
                    body.Location = body.Location + diff1;
                    Thread.Sleep(50);
                }
            }).Start();
        }
    }
}
