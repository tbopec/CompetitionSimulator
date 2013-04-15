using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace AIRLab.Thornado
{
    public class TextTable
    {
        #region Всякая лабуда

        public readonly string[] Captions;
        public readonly int ColumnCount;
        public readonly double[] PercentWidthes;

        public readonly int[] Widthes;
        private readonly List<string[]> rows;

        public TextTable(int ColumnCount)
        {
            this.ColumnCount = ColumnCount;
            rows = new List<string[]>();
            Widthes = new int[ColumnCount];
            PercentWidthes = new double[ColumnCount];
            Captions = new string[ColumnCount];
        }

        public ReadOnlyCollection<string[]> Rows
        {
            get { return rows.AsReadOnly(); }
        }

        public void AddRow(params string[] entry)
        {
            if(entry.Length != ColumnCount)
                throw new Exception("Неверное количество аргументов - должно быть " + ColumnCount);
            rows.Add(entry);
        }

        public void SetCaptions(params string[] captions)
        {
            if(captions.Length != ColumnCount)
                throw new Exception("Неверное количество аргументов - должно быть " + ColumnCount);
            for(int i = 0; i < ColumnCount; i++)
                Captions[i] = captions[i];
        }

        public void SetPercentWidthes(params double[] quots)
        {
            if(quots.Length != ColumnCount)
                throw new Exception("Неверное количество аргументов - должно быть " + ColumnCount);
            for(int i = 0; i < PercentWidthes.Length; i++)
                PercentWidthes[i] = quots[i];
        }

        public void SetWidthesFromPercents(int total)
        {
            int total1 = total - 1 - ColumnCount;
            int mytotal = 0;
            for(int i = 0; i < PercentWidthes.Length; i++) {
                Widthes[i] = (int) (total1 * PercentWidthes[i]);
                mytotal += Widthes[i];
            }
            Widthes[0] += (total - mytotal - ColumnCount - 1);
        }

        #endregion

        private List<char[]> glyph;
        private int totalWidth;

        private void WriteAt(int x, int y, char c)
        {
            while(glyph.Count <= y)
                glyph.Add(new char[totalWidth]);
            glyph[y][x] = c;
        }

        private int WriteText(int x0, int y0, int width, string text)
        {
            int ptr = 0;
            bool br = false;
            int dy = 0;
            for(;; dy++) {
                for(int dx = 0; dx < width; dx++) {
                    WriteAt(x0 + dx, y0 + dy, text[ptr]);
                    ptr++;
                    if(ptr >= text.Length) {
                        br = true;
                        break;
                    }
                }
                if(br)
                    break;
                if(text[ptr] != ' ' && width > 6)
                    for(int i = 1; i < 5 && i < ptr; i++)
                        if(text[ptr - i] == ' ') {
                            for(int j = 0; j < i; j++)
                                WriteAt(x0 + width - j, y0 + dy, ' ');
                            ptr = ptr - i + 1;
                        }
            }
            return dy + 1;
        }

        private int WriteRow(int y, string[] texts)
        {
            int x = 1;
            int hei = 0;
            for(int i = 0; i < Widthes.Length; i++) {
                hei = Math.Max(WriteText(x, y, Widthes[i], texts[i]), hei);
                x += Widthes[i] + 1;
            }
            return hei;
        }

        private int DrawWholeRow(int y, string[] texts)
        {
            int hei = WriteRow(y, texts);
            for(int i = 0; i < hei; i++)
                DrawVertical(y + i, Widthes);
            return hei;
        }


        private void DrawVertical(int y, int[] widthes)
        {
            int x = 0;
            for(int i = -1; i < widthes.Length; i++) {
                if(i != -1)
                    x += widthes[i] + 1;
                char c = (i == -1 || i == widthes.Length - 1) ? '║' : '│';
                WriteAt(x, y, c);
            }
        }

        private void DrawHorizontal(int y, char begin, char middle, char separator, char end)
        {
            int x = 0;
            for(int i = -1; i < Widthes.Length; i++) {
                if(i != -1)
                    for(int j = 0; j <= Widthes[i]; j++)
                        WriteAt(++x, y, middle);
                char c = separator;
                if(i == -1)
                    c = begin;
                if(i == Widthes.Length - 1)
                    c = end;
                WriteAt(x, y, c);
            }
        }

        public string PrintTableToConsole()
        {
            glyph = new List<char[]>();
            totalWidth = 1;
            foreach(int t in Widthes)
                totalWidth += t + 1;
            int y = 0;

            DrawHorizontal(y, '╔', '═', '╤', '╗');
            y++;

            y += DrawWholeRow(y, Captions);

            DrawHorizontal(y, '╠', '═', '╪', '╣');
            y++;

            for(int i = 0; i < Rows.Count; i++) {
                y += DrawWholeRow(y, Rows[i]);

                if(i != Rows.Count - 1)
                    DrawHorizontal(y, '╟', '─', '┼', '╢');
                else
                    DrawHorizontal(y, '╚', '═', '╧', '╝');
                y++;
            }

            var bld = new StringBuilder();
            foreach(var t in glyph) {
                foreach(char t1 in t)
                    bld.Append(t1);
                bld.Append("\n");
            }
            return bld.ToString();
        }


        public string PrintTableToRTF()
        {
            string str = PrintTableToConsole();
            return str.Replace("\n", "\\par")
                .Replace('\0', ' ')
                .Replace("╔", "\\u9556\\'e3")
                .Replace("═", "\\u9552\\'e3")
                .Replace("╤", "\\u9572\\'e3")
                .Replace("╗", "\\u9559\\'e3")
                .Replace("╠", "\\u9568\\'e3")
                .Replace("╪", "\\u9578\\'e3")
                .Replace("╣", "\\u9571\\'e3")
                .Replace("╟", "\\u9567\\'e3")
                .Replace("─", "\\u9472\\'e3")
                .Replace("┼", "\\u9532\\'e3")
                .Replace("╢", "\\u9570\\'e3")
                .Replace("╚", "\\u9562\\'e3")
                .Replace("╧", "\\u9575\\'e3")
                .Replace("╝", "\\u9565\\'e3")
                .Replace("║", "\\u9553\\'e3")
                .Replace("│", "\\u9474\\'e3");
        }


        /*
         * B0 	  	░ 	▒ 	▓ 	│ 	┤ 	╡ 	╢ 	╖ 	╕ 	╣ 	║ 	╗ 	╝ 	╜ 	╛ 	┐
C0 	  	└ 	┴ 	┬ 	├ 	─ 	┼ 	╞ 	╟ 	╚ 	╔ 	╩ 	╦ 	╠ 	═ 	╬ 	╧
D0 	  	╨ 	╤ 	╥ 	╙ 	╘ 	╒ 	╓ 	╫ 	╪ 	┘ 	┌ 	█ 	▄ 	▌ 	▐ 	▀
         * */
    }
}