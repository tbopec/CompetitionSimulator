using System;
using AIRLab.Thornado;

namespace AIRLab.Mathematics {
    [Serializable]
    public class Matrix : ICloneable {
        double[,] matrix;
        public readonly int RowCount;
        public readonly int ColumnCount;
        public double this[int row, int column] {
            get {
                return matrix[row, column];
            }
            set {
                matrix[row, column] = value;
            }
        }

        public void Print() {
            Console.WriteLine();
            for(int i = 0; i < RowCount; i++) {
                for(int j = 0; j < ColumnCount; j++)
                    Console.Write(Formats.Double.Write(matrix[i, j]) + "\t");
                Console.WriteLine();
            }
        }

        public override String ToString() {
            String str = "";
            for(int i = 0; i < RowCount; i++) {
                for(int j = 0; j < ColumnCount; j++)
                    str += (Formats.Double.Write(matrix[i, j]) + "\t");
                str += "\n";
            }
            return str;
        }

        #region constructors
        public Matrix(int rowCount, int columnCount) {
            RowCount = rowCount;
            ColumnCount = columnCount;
            matrix = new double[rowCount, columnCount];
        }

        public static Matrix IdentityMatrix(int width) {
            Matrix matrix = new Matrix(width, width);
            for(int i = 0; i < width; i++)
                for(int j = 0; j < width; j++)
                    if(i == j)
                        matrix[i, j] = 1;
                    else
                        matrix[i, j] = 0;
            return matrix;
        }
        #endregion

        #region operations
        public static Matrix operator +(Matrix a, Matrix b) {
            if(a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Sizes of matrices are not equal");
            var c = new Matrix(a.RowCount, a.ColumnCount);
            for(int row = 0; row < c.RowCount; row++)
                for(int column = 0; column < c.ColumnCount; column++)
                    c[row, column] = a[row, column] + b[row, column];
            return c;
        }

        public static Matrix operator -(Matrix a, Matrix b) {
            if(a.RowCount != b.RowCount || a.ColumnCount != b.ColumnCount)
                throw new ArgumentException("Sizes of matrices are not equal");
            var c = new Matrix(a.RowCount, a.ColumnCount);
            for(int row = 0; row < c.RowCount; row++)
                for(int column = 0; column < c.ColumnCount; column++)
                    c[row, column] = a[row, column] - b[row, column];
            return c;
        }

        public static Matrix operator *(Matrix a, Matrix b) {
            if(a.ColumnCount != b.RowCount)
                throw new ArgumentException("Matrices are not fit to multiply");
            var c = new Matrix(a.RowCount, b.ColumnCount);
            for(int row = 0; row < c.RowCount; row++)
                for(int column = 0; column < c.ColumnCount; column++)
                    for(int k = 0; k < a.ColumnCount; k++)
                        c[row, column] += a[row, k] * b[k, column];
            return c;
        }

        public static Matrix operator /(Matrix matrix, double value) {
            var new_matrix = new Matrix(matrix.RowCount, matrix.ColumnCount);
            for(int row = 0; row < matrix.RowCount; row++)
                for(int column = 0; column < matrix.ColumnCount; column++)
                    new_matrix[row, column] = matrix[row, column] / value;
            return new_matrix;
        }

        public static Matrix operator *(Matrix matrix, double value) {
            var new_matrix = new Matrix(matrix.RowCount, matrix.ColumnCount);
            for(int row = 0; row < matrix.RowCount; row++)
                for(int column = 0; column < matrix.ColumnCount; column++)
                    new_matrix[row, column] = matrix[row, column] * value;
            return new_matrix;
        }

        public static Matrix operator *(double value, Matrix matrix) {
            var new_matrix = new Matrix(matrix.RowCount, matrix.ColumnCount);
            for(int row = 0; row < matrix.RowCount; row++)
                for(int column = 0; column < matrix.ColumnCount; column++)
                    new_matrix[row, column] = matrix[row, column] * value;
            return new_matrix;
        }

        #endregion

        /// <summary>
        /// translate matrix to row echelon(переводит матрицу в ступенчатый вид)
        /// </summary>
        /// <returns>returns matrix and amount of swappings</returns>
        public Tuple<Matrix, int> ToRowEchelon() {
            int swapings = 0;
            double[][] matrix = new double[this.RowCount][];
            for(int i = 0; i < this.RowCount; i++) {
                matrix[i] = new double[this.ColumnCount];
                for(int j = 0; j < this.ColumnCount; j++) {
                    matrix[i][j] = this[i, j];
                }
            }

            int activeColumn = 0;

            for(int buildedRows = 0; buildedRows < this.RowCount && activeColumn < this.ColumnCount; buildedRows++) {
                int lastElement = -1;
                for(; activeColumn < this.ColumnCount && lastElement < 0; activeColumn++) {
                    for(int row = buildedRows; row < this.RowCount && lastElement < 0; row++) {
                        if(System.Math.Abs(matrix[row][activeColumn]) > double.Epsilon) {
                            lastElement = row;
                        }
                    }
                    if(lastElement >= 0) {
                        if(buildedRows != lastElement) {
                            swapings++;
                            double[] swap_element = matrix[buildedRows];
                            matrix[buildedRows] = matrix[lastElement];
                            matrix[lastElement] = swap_element;
                        }

                        for(int i = buildedRows + 1; i < this.RowCount; i++) {
                            if(System.Math.Abs(matrix[i][activeColumn]) > double.Epsilon) {
                                double koef = matrix[i][activeColumn] / matrix[buildedRows][activeColumn];
                                for(int j = activeColumn; j < this.ColumnCount; j++) {
                                    matrix[i][j] -= matrix[buildedRows][j] * koef;
                                }
                            }
                        }
                    }
                }
            }

            Matrix new_matrix = new Matrix(this.RowCount, this.ColumnCount);
            for(int i = 0; i < this.RowCount; i++) {
                for(int j = 0; j < this.ColumnCount; j++) {
                    new_matrix[i, j] = matrix[i][j];
                }
            }
            return new Tuple<Matrix, int>(new_matrix, swapings);
        }

        public double GetDeterminant() {
            if(this.ColumnCount != this.RowCount)
                throw new Exception("Матрица должна быть квадратной.");
            Tuple<Matrix, int> triangle_matrix = this.ToRowEchelon();

            double determinant = ((triangle_matrix.Item2 & 1) == 1 ? -1 : 1);
            for(int i = 0; i < this.RowCount; i++) {
                determinant *= triangle_matrix.Item1[i, i];
            }
            return determinant;
        }

        public double Minor(int x, int y, int size) {
            if(x + size > this.ColumnCount || y + size > this.RowCount || y < 0 || x < 0) {
                throw new ArgumentOutOfRangeException();
            }

            Matrix matrix = new Matrix(size, size);
            for(int i = 0; i < size; i++) {
                for(int j = 0; j < size; j++) {
                    matrix[i, j] = this[x + i, y + j];
                }
            }
            return matrix.GetDeterminant();
        }

        #region ICloneable Members

        public object Clone() {
            Matrix matrix = new Matrix(this.RowCount, this.ColumnCount);
            for(int i = 0; i < this.RowCount; i++) {
                for(int j = 0; j < this.ColumnCount; j++) {
                    matrix[i, j] = this.matrix[i, j];
                }
            }
            return matrix;
        }

        #endregion
    }
}