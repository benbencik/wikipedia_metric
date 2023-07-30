using System;

namespace wikipedia_metric
{
    public class AsciiTablePrinter
    {
        public static void PrintTable(string[] columnNames, string[,] table)
        {
            int rowCount = table.GetLength(0);
            int columnCount = table.GetLength(1);

            int[] columnWidths = CalculateColumnWidths(columnNames, table);

            // Print the top border line
            PrintLine(columnWidths);

            // Print the column names row
            PrintRow(columnWidths, columnNames);

            // Print the line separating the column names row and the table contents
            PrintLine(columnWidths);

            // Print the table rows
            for (int i = 0; i < rowCount; i++)
            {
                PrintRow(columnWidths, table, i);
            }

            // Print the bottom border line
            PrintLine(columnWidths);
        }

        private static void PrintLine(int[] columnWidths)
        {
            // Print a line with '+' and '-' characters for each column
            for (int i = 0; i < columnWidths.Length; i++)
            {
                Console.Write("+");
                Console.Write(new string('-', columnWidths[i] + 2));
            }
            Console.WriteLine("+");
        }

        private static void PrintRow(int[] columnWidths, string[] values)
        {
            int columnCount = columnWidths.Length;
            for (int j = 0; j < columnCount; j++)
            {
                string cellValue = values[j];
                Console.Write("| ");
                Console.Write(cellValue.PadRight(columnWidths[j]));
                Console.Write(" ");
            }
            Console.WriteLine("|");
        }

        private static void PrintRow(int[] columnWidths, string[,] table, int rowIndex)
        {
            int columnCount = columnWidths.Length;
            for (int j = 0; j < columnCount; j++)
            {
                string cellValue = table[rowIndex, j];
                Console.Write("| ");
                Console.Write(cellValue.PadRight(columnWidths[j]));
                Console.Write(" ");
            }
            Console.WriteLine("|");
        }

        private static int[] CalculateColumnWidths(string[] columnNames, string[,] table)
        {
            int rowCount = table.GetLength(0);
            int columnCount = table.GetLength(1);

            int[] columnWidths = new int[columnCount];

            // Calculate the maximum width for each column, including column names
            for (int j = 0; j < columnCount; j++)
            {
                int maxWidth = columnNames[j].Length;
                for (int i = 0; i < rowCount; i++)
                {
                    string cellValue = table[i, j];
                    int cellWidth = cellValue.Length;
                    if (cellWidth > maxWidth)
                    {
                        maxWidth = cellWidth;
                    }
                }
                columnWidths[j] = maxWidth;
            }

            return columnWidths;
        }
    }
}

// string[] columnNames = { "Name", "Age", "City" };

// string[,] table = new string[,]
// {
//     { "John", "25", "New York" },
//     { "Alice", "30", "London" },
//     { "Bob", "35", "Paris" }
// };

// AsciiTablePrinter.PrintTable(columnNames, table);
