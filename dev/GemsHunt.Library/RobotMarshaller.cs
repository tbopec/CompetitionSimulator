using Eurosim.Core;

namespace GemsHunt.Library
{
   class RobotMarshaller
    {
        public RobotMarshaller(Body robot)
        {

        }


        public static void Main(string[] args)
        {
            

         }

        //1. взять команды на перемещение и изменить скорость robot-a
        //2. нужно уметь собирать данные с "сенсоров" и отправлять их в сеть в XML
        //2.а. Где я нахожусь?
        //2.б. Сколько до вражеского робота?
        //2.в. Камера: еще одна сцена, ставите параметры камеры в соответствие с положением робота, и делаете "сколку" с экрана
        //2.г. Кинект
        //3. 
    }

  
    class Robot2013Marshaller : RobotMarshaller
    {
        public Robot2013Marshaller(Robot2013 robot) : base(robot) { }
    }
}

