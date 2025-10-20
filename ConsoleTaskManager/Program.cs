using System;
using System.IO;
using System.Text.Json;

class Task
{
    public bool IsDone;
    public string Name;
    public string Description;
    public DateTime CreationDate { get; private set; }
    public DateOnly DeadLine;

    public Task(string name, DateOnly deadLine, string description = "")
    {
        IsDone = false;
        Name = name;
        Description = description;
        CreationDate = DateTime.Now;
        DeadLine = deadLine;
    }

    public override string ToString()
    {
        return $" [{(IsDone ? "X" : " ")}] - {Name} ";
    }
    public string GetAllInformation()
    {
        return $"""

        [{(IsDone ? "X" : " ")}] - {Name}
        Описание:"
        {Description}
        Дата создания: {CreationDate}
        Дедлайн: {DeadLine}
        """;
    }
}
class Section
{
    public string Name;
    public int index;
    public List<Task> section = new List<Task>();

    public Section(string name, params Task[] tasks)
    {
        Name = name;
        for (int i = 0; i < tasks.Length; i++)
            section.Add(tasks[i]);
    }
    public Section(string name, List<Task> tasks)
    {
        Name = name;
        section = tasks;
    }
    public void ShowAllTasks()
    {
        Console.WriteLine("Все задачи раздела " + Name + ": ");
        for (int i = 0; i < section.Count; i++) Console.WriteLine(i + " - " + section[i]);
    }
}
class ConsoleManager
{
    static private string path = "DataBase.json";
    static private int CurrentSection = 0;
    static List<Section> sections = new List<Section>() { new Section("Main") };
    static void Main()
    {
        Console.Title = "Task Manager";
        Section tasks = new Section("Main");

        while (true)
        {
            Console.WriteLine("Добавить задачу в текущий раздел - 1");
            Console.WriteLine("Удалить задачу в текущем разделе - 2");
            Console.WriteLine("Создать новый раздел - 3");
            Console.WriteLine("Удалить раздел - 4");
            Console.WriteLine("Переключить раздел - 5");

            Console.Write("Выбор опции: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    var task = CreateTask();
                    sections[CurrentSection].section.Add(task);
                    Console.WriteLine("В раздел " + sections[CurrentSection].Name +
                    ", добавлена задача: " + task.GetAllInformation());


                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        IncludeFields = true
                    };

                    string json = JsonSerializer.Serialize(tasks.section, options);
                    File.WriteAllText(path, json);
                    break;
                case "2":
                    sections[CurrentSection].ShowAllTasks();

                    Console.Write("Выберите индекс удаляемой задачи: ");
                    int index = int.Parse(Console.ReadLine());

                    sections[CurrentSection].section.RemoveAt(index);
                    break;
                case "3":
                    Console.Write("Введите имя нового раздела: ");
                    string name = Console.ReadLine();

                    sections.Add(new Section(name));
                    Console.WriteLine("Вы добавили новый раздел с названием \"" + name + "\"");
                    break;
                case "4":
                    ShowSections();
                    CurrentSection = int.Parse(Console.ReadLine());
                    Console.Write("Выберите раздел для удаления (все задачи переместятся раздел main): ");

                    for (int i = 0; i < sections[CurrentSection].section.Count; i++)
                        sections[0].section.Add(sections[CurrentSection].section[i]);

                    sections.RemoveAt(CurrentSection);
                    Console.WriteLine("Удаление прошло успешно");
                    break;
                case "5":
                    ShowSections();
                    CurrentSection = int.Parse(Console.ReadLine());
                    Console.Write("Выберите раздел в который хотите перейти: ");

                    Console.WriteLine("Вы выбрали раздел: " + sections[CurrentSection].Name);
                    break;
                default:
                    Console.WriteLine("Вы ввели неверную команду!");
                    break;

            }

            Console.Write("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
    static void ShowSections()
    {
        Console.WriteLine("Все разделы: ");
        for (int i = 0; i < sections.Count; i++)
        {
            Console.WriteLine(i + 1 + sections[i].Name);
        }
    }
    static Task CreateTask()
    {
        Console.Write("Введите имя задачи: ");
        string name = Console.ReadLine();
        Console.Write("Введите описание (если не описания нет, то нажмите Enter): ");
        string description = Console.ReadLine();

        Console.WriteLine("Введите дедлайн: ");
        Console.Write(" Введите год: ");
        int year = int.Parse(Console.ReadLine());
        Console.Write(" Введите номер месяца: ");
        int month = int.Parse(Console.ReadLine());
        Console.Write(" Введите день: ");
        int day = int.Parse(Console.ReadLine());

        var date = new DateOnly(year, month, day);

        return new Task(name, date, description);
    }
}