using System;
using System.Collections.Generic;
using System.Linq;

internal static class ExtensionMethods
{
    internal static T[][] ToJaggedArray<T>(this T[,] twoDimensionalArray)
    {
        int rowsFirstIndex = twoDimensionalArray.GetLowerBound(0);
        int rowsLastIndex = twoDimensionalArray.GetUpperBound(0);
        int numberOfRows = rowsLastIndex - rowsFirstIndex + 1;

        int columnsFirstIndex = twoDimensionalArray.GetLowerBound(1);
        int columnsLastIndex = twoDimensionalArray.GetUpperBound(1);
        int numberOfColumns = columnsLastIndex - columnsFirstIndex + 1;

        T[][] jaggedArray = new T[numberOfRows][];
        for (int i = 0; i < numberOfRows; i++)
        {
            jaggedArray[i] = new T[numberOfColumns];

            for (int j = 0; j < numberOfColumns; j++)
            {
                jaggedArray[i][j] = twoDimensionalArray[i + rowsFirstIndex, j + columnsFirstIndex];
            }
        }
        return jaggedArray;
    }

    public static int FirstIndexMatch<TItem>(this IEnumerable<TItem> items, Func<TItem,bool> matchCondition)
        {
            var index = 0;
            foreach (var item in items)
            {
                if(matchCondition.Invoke(item))
                {
                    return index;
                }
                index++;
            }
            return -1;
        }

    public static T[] GetColumn<T>(this T[,] matrix, int columnNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(0))
                .Select(x => matrix[x, columnNumber])
                .ToArray();
    }

    public static T[] GetRow<T>(this T[,] matrix, int rowNumber)
    {
        return Enumerable.Range(0, matrix.GetLength(1))
                .Select(x => matrix[rowNumber, x])
                .ToArray();
    }
}