using System;
using System.Collections.Generic;
using System.Linq;
using DeterminantMethods;

namespace MainTest
{
    class Program
    {
        static double[][] list2matrix(List<int> list)
        {
            int nb2 = list.Count;
            int nb = (int)Math.Sqrt(nb2);

            var mat = Enumerable.Range(0, nb).Select(i => new double[nb]).ToArray();

            for (int i = 0; i < nb; ++i)
                for (int j = 0; j < nb; ++j)
                    mat[i][j] = list[i * nb + j];

            return mat;
        }

        static bool testDefinitionDeterminant(string s, bool printAll = true)
        {
            var l0 = s.Split(' ').Select(int.Parse).ToList();
            var det0 = l0[0];
            var l1 = l0.Skip(1).ToList();
            var mat = list2matrix(l1);

            var det1 = Permutation.ComputeDeterminant(mat);

            if (printAll || det0 != det1)
            {
                foreach (var r in mat)
                    Console.WriteLine(string.Join(" ", r));

                var info = det0 != det1 ? $"Bad;  {det0,10} != {det1}" : $"Good; {det0,10} == {det1}";
                Console.WriteLine($"=> {info} definitionDeterminant");
                Console.WriteLine();
            }

            return det0 == det1;
        }

        static bool testCofactorDeterminant(string s, bool printAll = true)
        {
            var l0 = s.Split(' ').Select(int.Parse).ToList();
            var det0 = l0[0];
            var l1 = l0.Skip(1).ToList();
            var mat = list2matrix(l1);

            var det1 = CoFactorMethod.ComputeDeterminant(mat);

            if (printAll || det0 != det1)
            {
                foreach (var r in mat)
                    Console.WriteLine(string.Join(" ", r));

                var info = det0 != det1 ? $"Bad;  {det0,10} != {det1}" : $"Good; {det0,10} == {det1}";
                Console.WriteLine($"=> {info} cofactorDeterminant");
                Console.WriteLine();
            }

            return det0 == det1;
        }

        static bool testPivotDeterminant(string s, bool printAll = true)
        {
            var l0 = s.Split(' ').Select(int.Parse).ToList();
            var det0 = l0[0];
            var l1 = l0.Skip(1).ToList();
            var mat0 = list2matrix(l1);
            var mat1 = list2matrix(l1);

            var det1 = Math.Round(PivotMethod.ComputeDeterminant(mat1), 6);

            if (printAll || det0 != det1)
            {
                foreach (var r in mat0)
                    Console.WriteLine(string.Join(" ", r));

                var info = det0 != det1 ? $"Bad;  {det0,10} != {det1}" : $"Good; {det0,10} == {det1}";
                Console.WriteLine($"=> {info} pivotDeterminant");
                Console.WriteLine();
            }

            return det0 == det1;
        }

        static void test0()
        {
            Permutation.CreatePermutations(3);
            var perm = Permutation.AllPermutations;

            foreach (var e in perm)
            {
                Console.WriteLine($"Dim={e.Key,2} Count={e.Value.Count}");
                foreach (var p in e.Value)
                    Console.WriteLine(p);
            }

            Permutation.AllPermutations.Clear();
        }

        static void test1()
        {
            Permutation.CreatePermutations(5);

            testDefinitionDeterminant("-76 2 10 8 2");
            testDefinitionDeterminant("0 5 9 10 18");
            testDefinitionDeterminant("-358 6 3 8 2 4 10 8 9 3");
            testDefinitionDeterminant("0 6 3 8 12 6 16 8 9 3");

            //testDefinitionDeterminant("358 6 3 8 2 4 10 8 9 3"); // Will Fail
            //testDefinitionDeterminant("10 6 3 8 12 6 16 8 9 3"); // Will Fail

            testDefinitionDeterminant("-648 10 2 6 1 5 10 3 6 7 2 9 7 4 6 8 9");
            testDefinitionDeterminant("1718 6 4 10 1 10 5 1 9 3 2 9 10 6 2 6 2");

            testDefinitionDeterminant("52 4 6 10 7 1 4 9 8 10 2 10 7 10 7 6 10 3 6 1 10 10 7 1 8 3");
        }

        static void test2()
        {
            CoFactorMethod.createAllSubMatrix();

            testCofactorDeterminant("-76 2 10 8 2");
            testCofactorDeterminant("0 5 9 10 18");
            testCofactorDeterminant("-358 6 3 8 2 4 10 8 9 3");
            testCofactorDeterminant("0 6 3 8 12 6 16 8 9 3");

            //testDefinitionDeterminant("358 6 3 8 2 4 10 8 9 3"); // Will Fail
            //testDefinitionDeterminant("10 6 3 8 12 6 16 8 9 3"); // Will Fail

            testCofactorDeterminant("-648 10 2 6 1 5 10 3 6 7 2 9 7 4 6 8 9");
            testCofactorDeterminant("1718 6 4 10 1 10 5 1 9 3 2 9 10 6 2 6 2");

            testCofactorDeterminant("52 4 6 10 7 1 4 9 8 10 2 10 7 10 7 6 10 3 6 1 10 10 7 1 8 3");
        }

        static void test3()
        {
            PivotMethod.createAllSubMatrix();

            testPivotDeterminant("-76 2 10 8 2");
            testPivotDeterminant("0 5 9 10 18");
            testPivotDeterminant("-358 6 3 8 2 4 10 8 9 3");
            testPivotDeterminant("0 6 3 8 12 6 16 8 9 3");

            //testDefinitionDeterminant("358 6 3 8 2 4 10 8 9 3"); // Will Fail
            //testDefinitionDeterminant("10 6 3 8 12 6 16 8 9 3"); // Will Fail

            testPivotDeterminant("-648 10 2 6 1 5 10 3 6 7 2 9 7 4 6 8 9");
            testPivotDeterminant("1718 6 4 10 1 10 5 1 9 3 2 9 10 6 2 6 2");

            testPivotDeterminant("52 4 6 10 7 1 4 9 8 10 2 10 7 10 7 6 10 3 6 1 10 10 7 1 8 3");
        }

        static void Main(string[] args)
        {
            //test0();
            //test1();
            //test2();
            test3();

            Console.ReadKey();
        }
    }
}
