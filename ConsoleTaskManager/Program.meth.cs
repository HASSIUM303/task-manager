using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

delegate void ToGraphics();

partial class ConsoleManager //Не менять саму логику методов, а только исправлять баги.
                            //Менять логику только в том случаи, если это единственный способ исправить баг
{
    static private string path = "DataBase.json";
    static private JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        IncludeFields = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    static private int CurrentSection = 0;
    static List<Section> sections = new List<Section>() { new Section("Main") };


    static void StylizeMessage(ToGraphics meth, ConsoleColor color, bool CursorVisible = false)
    {
        bool defaultVisible = Console.CursorVisible;
        Console.CursorVisible = CursorVisible;

        ConsoleColor defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        meth();
        Console.ForegroundColor = defaultColor;

        Console.CursorVisible = defaultVisible;
    }
    static void StylizeMessage(ToGraphics meth, ConsoleColor color, bool CursorVisible = false, int x = 0, int y = 0)
    {
        Console.SetCursorPosition(x, y);
        StylizeMessage(meth, color, CursorVisible);
    }
    static void GetDataFromDataBase()
    {
        string json = File.ReadAllText(path);
        if (json != null && json != "")
            sections = JsonSerializer.Deserialize<List<Section>>(json, jsonOptions);
    }
    static void ShowSections(bool isIndexationFromZero = false)
    {
        Console.WriteLine(" Все разделы: ");
        for (int i = 0; i < sections.Count; i++)
        {
            Console.WriteLine("  " + (i + (isIndexationFromZero ? 0 : 1)) + " - " + sections[i].Name);
        }
    }

    static void ShowAllSectionsWithTasks()
    {
        Console.WriteLine(" Все раздели и задачи: \n");
        for (int i = 0; i < sections.Count; i++)
        {
            Console.WriteLine(" " + sections[i].Name + ": ");
            sections[i].ShowAllTasks();
        }
    }
    static Task CreateTask()
    {
        Console.Write(" Введите имя задачи: ");
        string name = Console.ReadLine();
        Console.Write(" Введите описание (если не описания нет, то нажмите Enter): ");
        string description = Console.ReadLine();

        Console.WriteLine(" Введите дедлайн: ");
        Console.Write("  Введите год: ");
        int year = int.Parse(Console.ReadLine());
        Console.Write("  Введите номер месяца: ");
        int month = int.Parse(Console.ReadLine());
        Console.Write("  Введите день: ");
        int day = int.Parse(Console.ReadLine());

        var date = new DateOnly(year, month, day);

        return new Task(name, date, description);
    }

    static void EditTask()
    {
        if (sections[CurrentSection].section.Count == 0)
        {
            StylizeMessage(() =>
            {
                Console.WriteLine($"В разделе {sections[CurrentSection].Name} нет задач для редактирования.");
                Console.ReadKey();
            }, ConsoleColor.Yellow, false);
            return;
        }

        sections[CurrentSection].ShowAllTasks();
        Console.WriteLine();

        Console.Write("Выберите индекс задачи для редактирования: ");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 0 || index >= sections[CurrentSection].section.Count)
        {
            StylizeMessage(() =>
            {
                Console.WriteLine("Вы ввели некорректное значение индекса.");
                Console.ReadKey();
            }, ConsoleColor.Red, false);
            return;
        }

        lable:
        Task task = sections[CurrentSection].section[index];

        Console.WriteLine($"Текущая задача: {task.GetAllInformation()}");
        Console.WriteLine(" Что вы хотите изменить?");
        Console.WriteLine(" 1 - Название задачи");
        Console.WriteLine(" 2 - Описание задачи");
        Console.WriteLine(" 3 - Дедлайн задачи");
        Console.WriteLine(" 4 - Статус задачи (выполнено/не выполнено)");
        Console.WriteLine(" 5 - Все параметры задачи");
        Console.WriteLine(" 6 - Выход из редактора");

        Console.Write("Выберите опцию: ");
        string option = Console.ReadLine();

        Console.WriteLine();

        switch (option)
        {
            case "1":
                Console.Write("Введите новое название задачи: ");
                task.Name = Console.ReadLine();
                break;
            case "2":
                Console.Write("Введите новое описание задачи (если нет описания, то нажмите Enter): ");
                task.Description = Console.ReadLine();
                break;
            case "3":
                try
                {
                    Console.WriteLine("Введите новый дедлайн: ");
                    Console.Write(" Введите год: ");
                    int year = int.Parse(Console.ReadLine());
                    Console.Write(" Введите номер месяца: ");
                    int month = int.Parse(Console.ReadLine());
                    Console.Write(" Введите день: ");
                    int day = int.Parse(Console.ReadLine());

                    task.DeadLine = new DateOnly(year, month, day);
                }
                catch (Exception e)
                {
                    StylizeMessage(() =>
                    {
                        Console.WriteLine($"Возникла ошибка \'{e.GetType().Name}\': {e}");
                        Console.ReadKey();
                    }, ConsoleColor.Red, false);
                    goto lable;
                }
                break;
            case "4":
                task.IsDone = !task.IsDone;
                break;
            case "5":
                var newTask = CreateTask();
                sections[CurrentSection].section[index] = newTask;
                break;
                case "7": return;
            default:
                StylizeMessage(() =>
                {
                    Console.WriteLine("Некорректный выбор.");
                    Console.ReadKey();
                }, ConsoleColor.Yellow, false);
                goto lable;
        }

        StylizeMessage(() =>
        {
            Console.WriteLine($"Задача {task.Name} успешно отредактирована.");
            Console.ReadKey();
        }, ConsoleColor.Green, false);
    }

    static void SearchTask()
    {
        Console.Write("Введите текст для поиска задачи: ");
        string searchText = Console.ReadLine()?.ToLower();

        if (string.IsNullOrEmpty(searchText))
        {
            StylizeMessage(() =>
            {
                Console.WriteLine("Текст для поиска не может быть пустым.");
                Console.ReadKey();
            }, ConsoleColor.Red, false);
            return;
        }

        bool found = false;
        Console.WriteLine($"Результаты поиска по запросу \"{searchText}\":\n");

        for (int i = 0; i < sections.Count; i++)
        {
            List<Task> matchingTasks = new List<Task>();

            for (int j = 0; j < sections[i].section.Count; j++)
            {
                Task task = sections[i].section[j];
                if (task.Name.ToLower().Contains(searchText) || task.Description.ToLower().Contains(searchText))
                {
                    matchingTasks.Add(task);
                }
            }

            if (matchingTasks.Count > 0)
            {
                found = true;
                Console.WriteLine($" Найдено в разделе '{sections[i].Name}':");

                for (int k = 0; k < matchingTasks.Count; k++)
                {
                    int taskIndex = sections[i].section.IndexOf(matchingTasks[k]);
                    Console.WriteLine($"  {taskIndex} - {matchingTasks[k]}");
                }
                Console.WriteLine();
            }
        }

        if (!found)
        {
            StylizeMessage(() =>
            {
                Console.WriteLine($"Задачи, содержащие \"{searchText}\", не найдены.");
                Console.ReadKey();
            }, ConsoleColor.Yellow, false);
        }
        else
        {
            Console.WriteLine("Поиск завершен.");
            Console.ReadKey();
        }
    }
}