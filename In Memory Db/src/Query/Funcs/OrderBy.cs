using System;
using System.Collections.Generic;
using System.Linq;
//todo Debug using RowsWrapper.Print
namespace InMemoryDb
{
    public partial class Funcs
    {
        /*You work recursively rightards. If you're given 3 columns to eb ordered by,
         * then you sort the leftmost column of the 3, and wherever there
         * is a section of rows of this column that are ties (ie are all the same)
         * then you sort this section based on the cells of the column to its right
         * (ie the next column in the paramter's array), and if there are sections of ties
         * in this second column then you sort these subsections based on the cells of the column
         * to the right, and so on.*/
        /// <summary>
        /// Uses a stable sorting algorithm.
        /// </summary>
        public Funcs OrderBy(string[] columnNames, string nameOfResultTable = null)
        {
            _currResultRows = _lastResult;
            _resultTable = new Table(_currResultRows);

            _OrderBy(columnNames, 0, 0, _resultTable.GetNumOfRows());


            _EndOfFunc(nameOfResultTable);
            return this;
        }


        private void _OrderBy(string[] columnNames, int columnStart, int rowStart, int rowEnd)
        {
            string columnName = columnNames[columnStart];
            _SortSection(columnName, rowStart, rowEnd);

            /*Find in this column a section of duplicates, sort it based on the row to the right,
             * and then repeat for that section.*/
            columnStart++;
            if (columnStart < columnNames.Length)
            {
                dynamic prevCell;
                _resultTable.GetCell(rowStart, columnName, out prevCell);
                dynamic currCell = prevCell;
                int start;
                int end;
                for (int rowIndex = rowStart; rowIndex < rowEnd;)
                {
                    start = rowIndex;
                    for (; prevCell.Equals(currCell) && rowIndex < rowEnd; rowIndex++)
                    {
                        prevCell = currCell;
                        _resultTable.GetCell(rowIndex, columnName, out currCell);
                    }
                    end = rowIndex;
                    bool theresMoreThanOneRowToSort = start + 1 != end;
                    if (theresMoreThanOneRowToSort)
                    {
                        _OrderBy(columnNames, columnStart, start, end);
                    }

                    //So you can reenter the inner loop to start finding the range of the next section (if another one exists).
                    prevCell = currCell;
                }
            }
        }


        /// <param name="start">Inclusive</param>
        /// <param name="end">Exclusive</param>
        private void _SortSection(string columnName, int start, int end)
        {
            //<originalIndex, indexOfWhereToBeMoved>
            Dictionary<int, int> dict = new Dictionary<int, int>();
            int size = end - start;
            ElemData[] originalArr = new ElemData[size];
            for (int i = 0; i < size; i++)
            {
                ElemData ed = new ElemData() { originalIndex = i };
                _resultTable.GetCell(i, columnName, out ed.elem);
                originalArr[i] = ed;
            }
            ElemData[] sortedArr = MergeSort(originalArr);
            for (int i = 0; i < sortedArr.Length; i++)
            {
                dict.Add(sortedArr[i].originalIndex, i);
            }


            /*rowA is at index3 and wants to be moved to index7. so rowA is moved to
             * index7, and rowB which was originally at index7 gets put in index3 for
             * now. now in the next iteration, we look at rowB, which is at index3, and
             * see where it wants to go, and repeat the process of swapping places with
             * where it wants to go and in the next iteration doing the proper placing of
             * the row that was
             * bumped.
             * This will cause a chain of moving all the rows to where they belong,
             * but if somewhere in the chain rowU bumps rowV, and rowV actually wanted to
             * be placed in the original index of rowU, then the chain would end because
             * in the next iteration rowV won't have to bump anyone in order to be palced
             * where it wants to be placed. So this is why there's the outer loop, ie to
             * make sure that even if the chain breaks and the inner loop is ended,
             * the rest of the rows will be taken care of, with a new chain being created.*/
            int currIndex;
            int destinationIndex;
            int originalIndexOfBumpedRow;
            bool loop;
            /*This and the Where extension method is needed instead of just removing the entry from the dict,
             * because you can't alter a collection while enumerating thruouhg it.*/
            HashSet<int> removed = new HashSet<int>();
            foreach (KeyValuePair<int, int> entry in dict.Where(e => !removed.Contains(e.Key)))
            {
                currIndex = entry.Key;
                destinationIndex = entry.Value;
                originalIndexOfBumpedRow = currIndex;
                loop = true;
                while (loop)
                {
                    _resultTable.Swap(currIndex, destinationIndex);
                    removed.Add(originalIndexOfBumpedRow);

                    //Setting up for the next iteration, ie for putting the bumped row where it always wanted to go.
                    //Note, the currIndex doesn't change for the entire chain.
                    originalIndexOfBumpedRow = destinationIndex;
                    destinationIndex = dict[originalIndexOfBumpedRow];
                    if (currIndex == destinationIndex)
                    {
                        removed.Add(originalIndexOfBumpedRow);
                        loop = false;
                    }
                }
            }
        }


        public class ElemData : IComparable<ElemData>
        {
            public int originalIndex;
            public dynamic elem;

            public int CompareTo(ElemData other)
            {
                return elem.CompareTo(other.elem);
            }
        }


        public static T[] MergeSort<T>(T[] elems) where T : IComparable<T>
        {
            return _MergeSort(elems, 0, elems.Length);
        }


        /// <param name="start">Inclusive></param>
        /// <param name="end">Exclusive</param>
        private static T[] _MergeSort<T>(T[] elems, int start, int end) where T : IComparable<T>
        {
            if (start + 1 == end)
            {
                T[] arr = new T[1];
                arr[0] = elems[start];
                return arr;
            }


            int middle = start + ((end - start) / 2);
            T[] sortedLeft = _MergeSort(elems, start, middle);
            T[] sortedRight = _MergeSort(elems, middle, end);


            bool loop = true;
            int leftI = 0;
            int rightI = 0;
            int newI = 0;
            T[] newElems = new T[end - start];
            while (loop)
            {
                T left = sortedLeft[leftI];
                T right = sortedRight[rightI];

                // = 0 ensures stable sort.
                if (left.CompareTo(right) <= 0)
                {
                    newElems[newI] = left;
                    leftI++;
                }
                else
                {
                    newElems[newI] = right;
                    rightI++;
                }
                newI++;

                if (leftI == sortedLeft.Length)
                {
                    //Fill in the rest with the rest of sortedRight.
                    for (int i = rightI; i < sortedRight.Length; i++)
                    {
                        newElems[newI++] = sortedRight[i];
                    }
                    loop = false;
                }
                else if (rightI == sortedRight.Length)
                {
                    //Fill in the rest with the rest of sortedLeft.
                    for (int i = leftI; i < sortedLeft.Length; i++)
                    {
                        newElems[newI++] = sortedLeft[i];
                    }
                    loop = false;
                }
            }
            return newElems;
        }
    }
}