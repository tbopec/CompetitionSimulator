using System.Drawing;
using AIRLab.Mathematics;
using Eurosim.Core;

namespace Eurosim.ChessUp
{
	public class ChessUpFigure : PhysicalPrimitiveBody
	{
		private ChessUpFigure(ChessUpRules.Figures type)
			: base(OneShape, Color.Yellow)
		{
			string str = type.ToString().ToLower();
			if(!str.Contains("tower"))
				Shape = OneShape;
			else if(str.Contains("2"))
				Shape = TwoShape;
			else if(str.Contains("3"))
				Shape = ThreeShape;
			if(!str.Contains("pawn"))
				ModelFileName = str;
			IsMaterial = true;
		}

		/// <summary>
		/// Фабричный метод для создания башен и фигур в ChessUp
		/// </summary>
		/// <param name="type">тип (ChessUpRules.Figures)</param>
		/// <param name="location">координаты</param>
		/// <returns></returns>
		public static ChessUpFigure CreateFigure(ChessUpRules.Figures type, Frame3D location)
		{
			var b = new ChessUpFigure(type)
			        {
			        	Location = location,
			        	Mass = FigureMass,
			        	FrictionCoefficient = _FrictionCoefficient,
			        	Name = type.ToString()
			        };
			return b;
		}

		public const string King = "King";
		public const string Queen = "Queen";
		public const string Pawn = "Pawn";
		public const string KingTower2 = "KingTower2";
		public const string QueenTower2 = "QueenTower2";
		public const string PawnTower2 = "PawnTower2";
		public const string KingTower3 = "KingTower3";
		public const string QueenTower3 = "QueenTower3";
		public const string PawnTower3 = "PawnTower3";

		public const double FigureMass = 25;
		public const double _FrictionCoefficient = 7;

		private static readonly Shape OneShape = new CyllinderShape(10, 10, 5);
		private static readonly Shape TwoShape = new CyllinderShape(10, 10, 10);
		private static readonly Shape ThreeShape = new CyllinderShape(10, 10, 15);
	}
}