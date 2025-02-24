using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    internal class Program
    {
        public static void Main(string[] args)
    {
        Console.WriteLine("Введите ряд чисел через пробел или запятую:");
        string input = Console.ReadLine();

        List<int> numbers = ParseInput(input);

        if (numbers.Count == 0)
        {
            Console.WriteLine("Не введено ни одного числа.");
            return;
        }

        QuickSort(numbers, 0, numbers.Count - 1);

        Console.WriteLine("Отсортированный список:");
        string output = "";
        foreach (int number in numbers)
        {
            output += number.ToString() + " ";
        }
        Console.WriteLine(output.Trim());
    }
        
    static List<int> ParseInput(string input)
    {
        List<int> numbers = new List<int>();
        string[] parts = input.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string part in parts)
        {
            if (int.TryParse(part, out int number))
            {
                numbers.Add(number);
            }
        }
        return numbers;
    }
    
    static void QuickSort(List<int> list, int left, int right)
    {
        if (left < right)
        {
            int pivotIndex = Partition(list, left, right);

            QuickSort(list, left, pivotIndex - 1);
            QuickSort(list, pivotIndex + 1, right);
        }
    }
    
    static int Partition(List<int> list, int left, int right)
    {
        int pivot = list[right];
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            if (list[j] <= pivot)
            {
                i++;
                
                Swap(list, i, j);
            }
        }
        
        Swap(list, i + 1, right);
        return i + 1;
    }
    
    static void Swap(List<int> list, int i, int j)
    {
        int temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
    }
}
