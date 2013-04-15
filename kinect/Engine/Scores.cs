﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Eurosim.Core
{
	[Serializable]
	public class Penalty
	{
		public override string ToString()
		{
			return String.Format("{0} Points: {1} at time = {2}", Value, Message, (int)Time);
		}
		/// <summary>
		/// Время начисления пенальти
		/// </summary>
		public double Time;
		/// <summary>
		/// Количество очков
		/// </summary>
		public int Value;
		/// <summary>
		/// Комментарий (напр., за что начислено пенальти)
		/// </summary>
		public string Message;
		/// <summary>
		/// Номер робота, которому пенальти выставлено.
		/// </summary>
		public int RobotNumber;
	}

	public class ScoreCollection
	{
		public ScoreCollection(int robotCount)
		{
			_tempSums = new int[robotCount];
			RobotCount = robotCount;
			ResetTemp();
		}

		public event Action ScoresChanged;

		public void ResetAll()
		{
			ResetTemp();
			if(_penalties.Count <= 0) return;
			_penalties.Clear();
			OnScoresChanged();
		}

		public void ResetTemp()
		{
			for(int i = 0; i < _tempSums.Length; i++)
			{
				if(_tempSums[i] == 0) continue;
				_tempSums[i] = 0;
				OnScoresChanged();
			}
		}

		public int GetFullSumForRobot(int robotNumber)
		{
			return GetPenalties(robotNumber).Sum(x => x.Value) + _tempSums[robotNumber];
		}

		public void SetTemp(int robotNumber, int value)
		{
			if(value == _tempSums[robotNumber])
				return;
			_tempSums[robotNumber] = value;
			OnScoresChanged();
		}

		public void SetTemp(int[] newTempScores)
		{
			if(_tempSums.SequenceEqual(newTempScores))
				return;
			_tempSums = newTempScores;
			OnScoresChanged();
		}

		public void AddPenalty(Penalty penalty)
		{
			_penalties.Add(penalty);
			OnScoresChanged();
		}

		/// <summary>
		/// Возвращает врЕменные очки (т.е. те, которые пересчитываются каждый такт)
		///	для робота с номером <paramref name="robotNumber"/>
		/// </summary>
		/// <param name="robotNumber">Номер робота</param>
		/// <returns></returns>
		public int GetTemp(int robotNumber)
		{
			return _tempSums[robotNumber];
		}

		/// <summary>
		/// Возращает список пенальти, начисленных роботу с номером <paramref name="robotNumber"/>
		/// </summary>
		/// <param name="robotNumber">Номер робота</param>
		/// <returns></returns>
		public IEnumerable<Penalty> GetPenalties(int robotNumber)
		{
			return _penalties.Where(x => x.RobotNumber == robotNumber);
		}

		public IEnumerable<Penalty> Penalties { get { return _penalties; } }
		public int RobotCount { get; private set; }

		private void OnScoresChanged()
		{
			if(ScoresChanged != null)
				ScoresChanged();
		}

		private int[] _tempSums;
		private readonly List<Penalty> _penalties = new List<Penalty>();
	}
}
