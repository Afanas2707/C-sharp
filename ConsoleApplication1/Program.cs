using System;
using System.Collections.Generic;

namespace ConsoleApplication1
{
    public abstract class Question
    {
        public string Text { get; protected set; }
        public int Points { get; protected set; }

        public Question(string text, int points)
        {
            Text = text;
            Points = points;
        }
        
        public abstract void DisplayQuestion();
        public abstract bool CheckAnswer(string userAnswer);
        public abstract void DisplayCorrectAnswer();
    }
    
    public class YesNoQuestion : Question
    {
        private bool _correctAnswer;

        public YesNoQuestion(string text, bool correctAnswer, int points = 1) 
            : base(text, points)
        {
            _correctAnswer = correctAnswer;
        }

        public override void DisplayQuestion()
        {
            Console.WriteLine($"\n{Text} (да/нет)");
        }

        public override bool CheckAnswer(string userAnswer)
        {
            bool answer = userAnswer.ToLower() == "да";
            return answer == _correctAnswer;
        }

        public override void DisplayCorrectAnswer()
        {
            Console.WriteLine($"Правильный ответ: {(_correctAnswer ? "да" : "нет")}");
        }
    }
    
    public class SingleChoiceQuestion : Question
    {
        private List<string> _options;
        private int _correctOption;

        public SingleChoiceQuestion(string text, List<string> options, int correctOption, int points = 1) 
            : base(text, points)
        {
            _options = options;
            _correctOption = correctOption;
        }

        public override void DisplayQuestion()
        {
            Console.WriteLine($"\n{Text}");
            for (int i = 0; i < _options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_options[i]}");
            }
            Console.WriteLine("Выберите один вариант (введите номер):");
        }

        public override bool CheckAnswer(string userAnswer)
        {
            if (int.TryParse(userAnswer, out int answer))
            {
                return answer - 1 == _correctOption;
            }
            return false;
        }

        public override void DisplayCorrectAnswer()
        {
            Console.WriteLine($"Правильный ответ: {_correctOption + 1}. {_options[_correctOption]}");
        }
    }
    
    public class MultipleChoiceQuestion : Question
    {
        private List<string> _options;
        private List<int> _correctOptions;

        public MultipleChoiceQuestion(string text, List<string> options, List<int> correctOptions, int points = 2) 
            : base(text, points)
        {
            _options = options;
            _correctOptions = correctOptions;
        }

        public override void DisplayQuestion()
        {
            Console.WriteLine($"\n{Text}");
            for (int i = 0; i < _options.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {_options[i]}");
            }
            Console.WriteLine("Выберите несколько вариантов (введите номера через запятую):");
        }

        public override bool CheckAnswer(string userAnswer)
        {
            string[] answers = userAnswer.Split(',');
            List<int> userChoices = new List<int>();

            foreach (string answer in answers)
            {
                if (int.TryParse(answer.Trim(), out int choice))
                {
                    userChoices.Add(choice - 1);
                }
            }

            if (userChoices.Count != _correctOptions.Count)
                return false;

            foreach (int choice in userChoices)
            {
                if (!_correctOptions.Contains(choice))
                    return false;
            }

            return true;
        }

        public override void DisplayCorrectAnswer()
        {
            Console.Write("Правильные ответы: ");
            for (int i = 0; i < _correctOptions.Count; i++)
            {
                Console.Write($"{_correctOptions[i] + 1}");
                if (i < _correctOptions.Count - 1)
                    Console.Write(", ");
            }
            Console.WriteLine();
        }
    }
    
    public class TextQuestion : Question
    {
        private string _correctAnswer;

        public TextQuestion(string text, string correctAnswer, int points = 1) 
            : base(text, points)
        {
            _correctAnswer = correctAnswer;
        }

        public override void DisplayQuestion()
        {
            Console.WriteLine($"\n{Text}");
            Console.WriteLine("Введите ваш ответ:");
        }

        public override bool CheckAnswer(string userAnswer)
        {
            return string.Equals(userAnswer.Trim(), _correctAnswer, StringComparison.OrdinalIgnoreCase);
        }

        public override void DisplayCorrectAnswer()
        {
            Console.WriteLine($"Правильный ответ: {_correctAnswer}");
        }
    }
    
    public class Test
    {
        private string _name;
        private List<Question> _questions;

        public Test(string name)
        {
            _name = name;
            _questions = new List<Question>();
        }

        public void AddQuestion(Question question)
        {
            _questions.Add(question);
        }

        public void RunTest()
        {
            Console.WriteLine($"=== Тест: {_name} ===\n");
            Console.WriteLine($"Всего вопросов: {_questions.Count}\n");
            Console.WriteLine("Нажмите Enter, чтобы начать...");
            Console.ReadLine();

            int correctAnswers = 0;
            int totalPoints = 0;
            int earnedPoints = 0;
            List<int> incorrectQuestions = new List<int>();

            for (int i = 0; i < _questions.Count; i++)
            {
                Question question = _questions[i];
                Console.WriteLine($"Вопрос {i + 1}:");
                question.DisplayQuestion();

                string userAnswer = Console.ReadLine();
                bool isCorrect = question.CheckAnswer(userAnswer);

                totalPoints += question.Points;

                if (isCorrect)
                {
                    correctAnswers++;
                    earnedPoints += question.Points;
                    Console.WriteLine("Верно!");
                }
                else
                {
                    incorrectQuestions.Add(i);
                    Console.WriteLine("Неверно.");
                }

                Console.WriteLine();
            }
            
            Console.WriteLine("=== Результаты теста ===");
            Console.WriteLine($"Правильных ответов: {correctAnswers} из {_questions.Count}");
            Console.WriteLine($"Набрано баллов: {earnedPoints} из {totalPoints}");
            Console.WriteLine($"Процент выполнения: {(double)earnedPoints / totalPoints * 100:F1}%\n");
            
            if (incorrectQuestions.Count > 0)
            {
                Console.WriteLine("Вопросы с неправильными ответами:");
                foreach (int i in incorrectQuestions)
                {
                    Console.WriteLine($"Вопрос {i + 1}: {_questions[i].Text}");
                    _questions[i].DisplayCorrectAnswer();
                    Console.WriteLine();
                }
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Test programmingTest = new Test("Основы программирования");
            
            programmingTest.AddQuestion(new YesNoQuestion(
                "C# является объектно-ориентированным языком программирования?", 
                true));

            programmingTest.AddQuestion(new SingleChoiceQuestion(
                "Какой оператор используется для наследования в C#?",
                new List<string> { "extends", ":", "implements", "inherit" },
                1));

            programmingTest.AddQuestion(new MultipleChoiceQuestion(
                "Какие из следующих являются основными принципами ООП?",
                new List<string> { "Инкапсуляция", "Итерация", "Наследование", "Полиморфизм", "Декомпозиция" },
                new List<int> { 0, 2, 3 }));

            programmingTest.AddQuestion(new TextQuestion(
                "Как называется метод, который вызывается при создании объекта класса?",
                "конструктор"));
            
            programmingTest.RunTest();

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}