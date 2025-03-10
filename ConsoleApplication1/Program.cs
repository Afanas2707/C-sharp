using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string filePath =
                "C:\\Users\\afony\\RiderProjects\\ConsoleApplication1\\ConsoleApplication1\\Dzhordan_The-Wheel-of-Time_1_The-Eye-of-the-World_RuLit_Me.txt";

            string[] lines;

            lines = File.ReadAllLines(filePath);


            int screenWidth = Console.WindowWidth;
            int screenHeight = Console.WindowHeight;
            int textPaneWidth = (int)(screenWidth * 0.7); // 70% экрана для текста
            int searchPaneWidth = screenWidth - textPaneWidth - 1;
            int startLine = 0;
            int startColumn = 0;
            string searchString = "";
            List<SearchResult> searchResults = new List<SearchResult>();
            int selectedSearchResultIndex = -1;

            while (true)
            {
                Console.Clear();

                // **Левая часть: Отображение текста**
                for (int i = 0; i < screenHeight; i++)
                {
                    int lineIndex = startLine + i;
                    Console.SetCursorPosition(0, i);
                    if (lineIndex < lines.Length)
                    {
                        string line = lines[lineIndex];
                        if (startColumn < line.Length)
                        {
                            string displayedLine = line.Substring(startColumn);
                            if (displayedLine.Length > textPaneWidth)
                            {
                                displayedLine = displayedLine.Substring(0, textPaneWidth);
                            }

                            Console.Write(displayedLine);
                            Console.Write(new string(' ', searchPaneWidth + 1));
                        }
                        else
                        {
                            Console.Write(new string(' ', textPaneWidth + searchPaneWidth + 1));
                        }
                    }
                }

                // **Разделитель между частями**
                for (int i = 0; i < screenHeight; i++)
                {
                    Console.SetCursorPosition(textPaneWidth, i);
                    Console.Write("|");
                }

                // **Правая часть: Отображение результатов поиска**
                Console.SetCursorPosition(textPaneWidth + 1, 0);
                if (!string.IsNullOrEmpty(searchString) && searchResults.Any())
                {
                    for (int i = 0; i < Math.Min(searchResults.Count, screenHeight - 2); i++)
                    {
                        SearchResult result = searchResults[i];
                        string context = GetContext(lines, result.LineNumber, result.CharPosition, searchPaneWidth - 5);
                        string displayResult = $"[{result.LineNumber + 1}:{result.CharPosition + 1}] {context}";
                        if (displayResult.Length > searchPaneWidth)
                        {
                            displayResult = displayResult.Substring(0, searchPaneWidth);
                        }

                        Console.SetCursorPosition(textPaneWidth + 1, i);
                        if (i == selectedSearchResultIndex)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                        }

                        Console.Write(displayResult);
                        Console.ResetColor();
                    }
                }
                else if (!string.IsNullOrEmpty(searchString))
                {
                    Console.WriteLine("Совпадений не найдено");
                }


                // **Строка статуса**
                Console.SetCursorPosition(0, screenHeight - 2);
                Console.Write($"Строка: {startLine + 1}, Символ: {startColumn + 1} ");
                Console.Write(new string(' ', screenWidth - Console.CursorLeft));

                // **Строка ввода поиска**
                Console.SetCursorPosition(0, screenHeight - 1);
                Console.Write("Поиск: " + searchString);
                Console.Write(new string(' ', screenWidth - Console.CursorLeft));

                Console.SetCursorPosition("Поиск: ".Length + searchString.Length, screenHeight - 1);

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
                        {
                            if (searchResults.Any())
                            {
                                selectedSearchResultIndex--;
                                if (selectedSearchResultIndex < 0)
                                {
                                    selectedSearchResultIndex = searchResults.Count - 1;
                                }

                                UpdateViewFromSearchResult(lines, searchResults[selectedSearchResultIndex],
                                    screenHeight, textPaneWidth, ref startLine, ref startColumn);
                            }
                        }
                        else if (startLine > 0)
                        {
                            startLine--;
                        }

                        break;
                    case ConsoleKey.DownArrow:
                        if ((keyInfo.Modifiers & ConsoleModifiers.Shift) != 0)
                        {
                            if (searchResults.Any())
                            {
                                selectedSearchResultIndex++;
                                if (selectedSearchResultIndex >= searchResults.Count)
                                {
                                    selectedSearchResultIndex = 0;
                                }

                                UpdateViewFromSearchResult(lines, searchResults[selectedSearchResultIndex],
                                    screenHeight, textPaneWidth, ref startLine, ref startColumn);
                            }
                        }
                        else if (startLine < lines.Length - screenHeight + 2 && startLine < lines.Length - 1)
                        {
                            startLine++;
                        }

                        break;
                    case ConsoleKey.LeftArrow:
                        if (startColumn > 0)
                        {
                            startColumn--;
                        }

                        break;
                    case ConsoleKey.RightArrow:
                        startColumn++;
                        break;
                    case ConsoleKey.Enter:
                        searchResults = SearchText(lines, searchString);
                        selectedSearchResultIndex = searchResults.Any() ? 0 : -1;
                        if (selectedSearchResultIndex != -1)
                        {
                            UpdateViewFromSearchResult(lines, searchResults[selectedSearchResultIndex], screenHeight,
                                textPaneWidth, ref startLine, ref startColumn);
                        }

                        break;
                    case ConsoleKey.Backspace:
                        if (searchString.Length > 0)
                        {
                            searchString = searchString.Substring(0, searchString.Length - 1);
                            searchResults.Clear();
                            selectedSearchResultIndex = -1;
                        }

                        break;
                    case ConsoleKey.Escape:
                        return;
                    default:
                        searchString += keyInfo.KeyChar;
                        searchResults.Clear();
                        selectedSearchResultIndex = -1;
                        break;
                }
            }
        }

        // Вспомогательный класс для хранения результатов поиска
        class SearchResult
        {
            public int LineNumber { get; set; }
            public int CharPosition { get; set; }
        }

        // Функция поиска текста
        static List<SearchResult> SearchText(string[] lines, string searchText)
        {
            List<SearchResult> results = new List<SearchResult>();
            if (string.IsNullOrEmpty(searchText))
            {
                return results; // Не ищем, если строка поиска пуста
            }

            for (int i = 0; i < lines.Length; i++)
            {
                int index = lines[i].IndexOf(searchText, StringComparison.OrdinalIgnoreCase);
                while (index != -1)
                {
                    results.Add(new SearchResult { LineNumber = i, CharPosition = index });
                    index = lines[i].IndexOf(searchText, index + 1,
                        StringComparison.OrdinalIgnoreCase); // Ищем следующее вхождение
                }
            }

            return results;
        }

        // Функция для получения контекста вокруг найденного слова
        static string GetContext(string[] lines, int lineNumber, int charPosition, int contextLength)
        {
            string line = lines[lineNumber];
            int start = Math.Max(0, charPosition - contextLength / 2);
            int end = Math.Min(line.Length, charPosition + contextLength / 2);
            return line.Substring(start, end - start);
        }

        // Функция для обновления позиции просмотра текста на основе результата поиска
        static void UpdateViewFromSearchResult(string[] lines, SearchResult result, int screenHeight, int textPaneWidth,
            ref int startLine, ref int startColumn)
        {
            startLine = Math.Max(0, result.LineNumber - screenHeight / 2);
            if (startLine + screenHeight > lines.Length)
            {
                startLine = Math.Max(0, lines.Length - screenHeight + 2);
            }

            startColumn = Math.Max(0, result.CharPosition - textPaneWidth / 4);
        }
    }
}