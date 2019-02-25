using System;
using System.Collections.Generic;
using System.Linq;

namespace DeterminantMethods
{
    public class Permutation
    {
        public int Dim { get; private set; }
        public int[] Array { get; private set; }
        public int Signature { get; private set; }

        public Permutation(List<int> array)
        {
            Dim = array.Count;
            Array = array.ToArray();
            Signature = ComputeSignature(array);
        }

        public override string ToString()
        {
            var s = string.Join(" ", Array);
            return $"{s} [{Signature,2}]";
        }

        // Get All Permutations of Dim N and transform them to All Permutations of Dim N+1
        static List<List<int>> AddDimension(List<List<int>> list)
        {
            if (list.Count == 0 || list.Any(l => l.Count != list.First().Count))
                throw new Exception();

            List<List<int>> list0 = new List<List<int>>();
            int nb = list.First().Count;

            for (int k = 0; k <= nb; ++k)
            {
                foreach (var l0 in list)
                {
                    var l1 = l0.ToList();
                    l1.Insert(nb - k, nb);
                    list0.Add(l1);
                }
            }

            return list0;
        }

        // Get All Orbits from permutation
        static List<List<int>> Orbits(List<int> list)
        {
            var all = new List<List<int>>();
            var list0 = list.ToList();

            while (list0.Count != 0)
            {
                List<int> l0 = new List<int>();
                var f = list0.First();

                while (list0.Contains(f))
                {
                    l0.Add(f);
                    list0.Remove(f);
                    f = list[f];
                }

                all.Add(l0);
            }

            return all;
        }

        // Compute the signature of a permutation
        static int ComputeSignature(List<int> list)
        {
            var orbits = Orbits(list);

            int n = list.Count;
            int p = orbits.Count;
            int q = orbits.Count(l => l.Count % 2 == 0);
            int s0 = 1, s1 = 1;
            for (int k = 0; k < q; ++k) s0 *= -1;
            for (int k = 0; k < n - p; ++k) s1 *= -1; // Produce the same result of s0

            return s0;
        }

        // All Permutations grouped by Dimension
        public static Dictionary<int, List<Permutation>> AllPermutations = new Dictionary<int, List<Permutation>>();

        public static void CreatePermutations(int maxDim = 10)
        {
            AllPermutations.Clear();
            maxDim = Math.Min(10, maxDim); // Maximum dimension is 10, and 10!=3,628,800 for complexity

            var permutations = new Dictionary<int, List<List<int>>>();
            permutations[1] = new List<List<int>>() { new List<int>() { 0 } };
            for (int k = 1; k < maxDim; ++k)
                permutations[k + 1] = AddDimension(permutations[k]);

            AllPermutations = permutations.SelectMany(e => e.Value)
                .Select(l => new Permutation(l))
                .GroupBy(e => e.Dim)
                .ToDictionary(a => a.Key, b => b.ToList());
        }

        public static double ComputeDeterminant(double[][] mat)
        {
            int dim = mat.Length;

            if (mat.Any(m => m.Length != dim)) throw new Exception("Not a Square matrix");
            if (dim > 10) throw new Exception("Too big matrix");
            if (AllPermutations.Count == 0) CreatePermutations();

            return AllPermutations[dim].Sum(p => p.Signature * p.Array.Select((i, k) => mat[i][k]).Aggregate((a, b) => a * b));
        }
    }

    public static class CoFactorMethod
    {
        const int MaxDim = 10; // Limited to 10 for complexity
        static double[][][] allSubMatrix = null;
        public static void createAllSubMatrix()
        {
            allSubMatrix = new double[MaxDim][][];
            for (int k = 1; k < MaxDim; ++k)
                allSubMatrix[k] = Enumerable.Range(0, k).Select(i0 => new double[k]).ToArray();
        }

        public static double[][] rowCofactor(double[][] mat, int row)
        {
            int nb = mat.Length - 1;
            double[][] mat0 = allSubMatrix[nb];

            for (int i = 0; i <= nb; ++i)
            {
                if (i == row) continue;
                int i0 = i < row ? i : i - 1;
                for (int j = 1; j <= nb; ++j)
                {
                    int j0 = j - 1;
                    mat0[i0][j0] = mat[i][j];
                }
            }

            return mat0;
        }

        public static double ComputeDeterminant(double[][] mat)
        {
            int nb = mat.Length;
            if (nb == 1) return mat[0][0];

            double det = 0;
            int s = 1;
            for (int k = 0; k < nb; ++k)
            {
                var mat0 = rowCofactor(mat, k);
                det += s * mat[k][0] * ComputeDeterminant(mat0);
                s *= -1;
            }

            return det;
        }
    }

    public static class PivotMethod
    {
        const int MaxDim = 20; // Unlimited
        static double[][][] allSubMatrix = null;
        public static void createAllSubMatrix()
        {
            allSubMatrix = new double[MaxDim][][];
            for (int k = 1; k < MaxDim; ++k)
                allSubMatrix[k] = Enumerable.Range(0, k).Select(i0 => new double[k]).ToArray();
        }

        static void swapNullRow(double[][] mat)
        {
            if (mat[0][0] != 0.0) return;
            int dim = mat.Length;

            int row = 0;
            for (row = 1; row < dim; ++row)
                if (mat[row][0] != 0.0) break;

            for (int k = 0; k < dim; ++k)
            {
                var a = mat[0][k];
                mat[0][k] = -mat[row][k];
                mat[row][k] = a;
            }
        }

        static void rowsOperations(double[][] mat)
        {
            swapNullRow(mat);

            int dim = mat.Length;
            double a00 = mat[0][0];
            double[] a0 = mat[0];

            for (int i = 1; i < dim; ++i)
            {
                double[] ai = mat[i];
                double ai0 = ai[0];
                if (ai0 == 0.0) continue;

                for (int j = 0; j < dim; ++j)
                {
                    double aij = ai[j];
                    ai[j] = aij - ai0 * a0[j] / a00;
                }
            }
        }

        static double[][] subMatrix(double[][] mat)
        {
            int dim = mat.Length;
            var mat0 = allSubMatrix[dim - 1];

            for (int i = 1; i < dim; ++i)
                for (int j = 1; j < dim; ++j)
                    mat0[i - 1][j - 1] = mat[i][j];

            return mat0;
        }

        // Recursive method
        public static double ComputeDeterminant(double[][] mat)
        {
            if (mat.Length == 1) return mat[0][0];

            rowsOperations(mat);
            double a00 = mat[0][0];
            if (a00 == 0.0) return 0.0;

            var mat0 = subMatrix(mat);
            return a00 * ComputeDeterminant(mat0);
        }
    }
}
