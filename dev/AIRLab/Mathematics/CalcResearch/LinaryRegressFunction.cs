namespace AIRLab.Mathematics.CalcResearch
{
    public class LinaryRegressFunction : IRegressFunction
    {
        private readonly double _minX;
        private readonly double _maxX;
        private readonly double _minY;
        private readonly double _maxY;
        public LinaryRegressFunction(double minX, double maxX, double minY, double maxY)
        {
            _minX = minX;
            _maxX = maxX;
            _minY = minY;
            _maxY = maxY;
        }

        public double GetValue(double frecuency)
        {
            if (_minX > frecuency)
                frecuency = _minX;
            if (_maxX < frecuency)
                frecuency = _maxX;
            if (frecuency < 0)
            {
                return (frecuency / _minX * _minY);
            }
            return (frecuency / _maxX * _maxY);
        }
    }
}
