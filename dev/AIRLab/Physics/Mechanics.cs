namespace AIRLab.Physics
{
    public class Mechanics
    {
        /// <summary>
        /// �������� ���� ������������ �����
        /// </summary>
        /// <param name="startVelocity">��������� ��������</param>
        /// <param name="acceleration">���������</param>
        /// <param name="time">�����</param>
        /// <returns>����</returns>
        public static double GetDiffPathBySTandA(double startVelocity, double acceleration, double time)
        {
            return startVelocity * time + acceleration * time * time / 2;
        }
        /// <summary>
        /// �������� ���� ������������ �����
        /// </summary>
        /// <param name="startVelocity">��������� ��������</param>
        /// <param name="finishVelocity">�������� ��������</param>
        /// <param name="time">�����</param>
        /// <returns>����</returns>
        public static double GetDiffPathBySTandSF(double startVelocity, double finishVelocity, double time)
        {
            return (startVelocity + finishVelocity) * time / 2;
        }
        /// <summary>
        /// �������� ��������� ��������
        /// </summary>
        /// <param name="path">���������� ����������</param>
        /// <param name="finishVelocity">�������� ��������</param>
        /// <param name="time">�����</param>
        /// <returns>��������� ��������</returns>
        public static double GetStartSpeedByPathAndSF(double path, double finishVelocity, double time)
        {
            return 2 * path / time - finishVelocity;
        }
        /// <summary>
        /// �������� �������� ��������
        /// </summary>
        /// <param name="path">���������� ����������</param>
        /// <param name="startVelocity">��������� ��������</param>
        /// <param name="time">�����</param>
        /// <returns>�������� ��������</returns>
        public static double GetFinishSpeedByPathAndST(double path, double startVelocity, double time)
        {
            return 2 * path / time - startVelocity;
        }

        /// <summary>
        /// �������� �������� ��������
        /// </summary>
        /// <param name="startVelocity">��������� ��������</param>
        /// <param name="acceleration">���������</param>
        /// <param name="time">�����</param>
        /// <returns>�������� ��������</returns>
        public static double GetFinishSpeedBySTAndAc(double startVelocity, double acceleration, double time)
        {
            return startVelocity + acceleration * time;
        }
        /// <summary>
        /// �������� ��������� ������������ �����
        /// </summary>
        /// <param name="startVelocity">��������� �������� �����</param>
        /// <param name="finishVelocity">�������� �������� �����</param>
        /// <param name="time">�����</param>
        /// <returns>���������</returns>
        public static double GetAcceleration(double startVelocity, double finishVelocity, double time)
        {
            return (finishVelocity - startVelocity) / time;
        }

        public static double LinerySpeedToAngleSpeed(double speed, double r)
        {
            return speed/r;
        }
        public static double AngleSpeedToLinerySpeed(double w, double r)
        {
            return w* r;
        }
    }
}
