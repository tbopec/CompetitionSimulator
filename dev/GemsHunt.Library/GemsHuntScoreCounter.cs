using System;
using System.Linq;
using Eurosim.Core;

namespace GemsHunt.Library
{
    public class GemsHuntScoreCounter : IScoreCounter
    {
        public GemsHuntScoreCounter(ScoreCollection scoreCollection, Body root)
        {
            _scoreCollection = scoreCollection;
            _root = root;
        }

        private const int RubyUnit = 1;

        private static class ScoreRuby
        {             
            public const int Unit = 1;
            public const int Few = 3;
            public const int Maximum = 4;
        };

        private static class ScoreEmerald
        {
            public const int Unit = 1;
            public const int Few = 3;
            public const int Maximum = 4;
        };

        private static class ScoreDiamond
        {
            public const int Unit = 3;
            public const int Few = 5;
            public const int Maximum = 8;
        };

        private int ScoringTturret(int count, int unit, int few, int maximum)
        {
            int ScoreTemp = 0;

            if (count == 1)
            {
                ScoreTemp += unit;
            }
            else
            {
                if (count < 4)
                {
                    ScoreTemp += count * few;
                }
                else
                {
                    ScoreTemp += count * maximum;
                }
            }

            return ScoreTemp;
        }

        public void UpdateScores()
        {
            Body CurrentWorld = _root.FirstOrDefault();
            int ScoreLeft = 0;
            int ScoreRight = 0;
            foreach (Body CurrentBody in CurrentWorld)
            {
                try
                {
                    if (CurrentBody.Name.Length >= 9)
                    {
                        string type_name = CurrentBody.Name.Substring(0, 9);
                        if (type_name.Substring(0, 4) == "Romb")
                        {
                            foreach (Body CurrentBox in CurrentWorld)
                            {
                                if (Math.Pow((CurrentBody.Location.X - CurrentBox.Location.X), 2) + Math.Pow((CurrentBody.Location.Y - CurrentBox.Location.Y), 2) < 40)
                                {
                                    int ScoreTemp = 0;
                                    switch (CurrentBox.Name)
                                    {
                                        case "Ruby":
                                            int count = CurrentBox.GetSubtreeChildrenFirst().Count();
                                            ScoreTemp += ScoringTturret(count, ScoreRuby.Unit, ScoreRuby.Few, ScoreRuby.Maximum);
                                            break;
                                        case "Emerald":
                                            count = CurrentBox.GetSubtreeChildrenFirst().Count();
                                            ScoreTemp += ScoringTturret(count, ScoreEmerald.Unit, ScoreEmerald.Few, ScoreEmerald.Maximum);
                                            break;
                                        case "Diamond":
                                            count = CurrentBox.GetSubtreeChildrenFirst().Count();
                                            ScoreTemp += ScoringTturret(count, ScoreDiamond.Unit, ScoreDiamond.Few, ScoreDiamond.Maximum);
                                            break;
                                    }
                                    switch (type_name)
                                    {
                                        case "RombRight":
                                            ScoreRight += ScoreTemp;
                                            break;
                                        case "RombLeftt":
                                            ScoreLeft += ScoreTemp;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                }
                catch
                {
                    //   Console.WriteLine("ошибочка вышла");
                }
            }
            _scoreCollection.SetTemp(new[] { ScoreLeft, ScoreRight });
        }
                
        private readonly ScoreCollection _scoreCollection;
        private readonly Body _root;
    }
}