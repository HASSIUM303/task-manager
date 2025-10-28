using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        Описание:" {Description}
        Дата создания: {CreationDate}
        Дедлайн: {DeadLine}
        """;
    }
}
class Section
{
    public string Name { get; set; }
    public List<Task> section { get; set; } = new List<Task>();

    public Section(string name, params Task[] tasks)
    {
        Name = name;
        for (int i = 0; i < tasks.Length; i++)
            section.Add(tasks[i]);
    }

    [JsonConstructor]
    public Section(string name, List<Task> section)
    {
        Name = name;
        this.section = section;
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
    static private JsonSerializerOptions jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        IncludeFields = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    static private int CurrentSection = 0;
    static List<Section> sections = new List<Section>() { new Section("Main") };

    static void Main()
    {
        GetDataFromDataBase();

        while (true)
        {
            Console.Title = "Task Manager - " + sections[CurrentSection].Name;

            Console.WriteLine("1 - Добавить задачу в текущий раздел");
            Console.WriteLine("2 - Удалить задачу в текущем разделе");
            Console.WriteLine("3 - Создать новый раздел");
            Console.WriteLine("4 - Удалить раздел");
            Console.WriteLine("5 - Переключить раздел");
            Console.WriteLine("6 - Сохранить изменения в базы данных");
            Console.WriteLine("7 - Получить данные из базы данных");

            Console.Write(" Выбор опции: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    var task = CreateTask();
                    sections[CurrentSection].section.Add(task);
                    Console.WriteLine("В раздел " + sections[CurrentSection].Name +
                    ", добавлена задача: " + task.GetAllInformation());
                    break;
                case "2":
                    sections[CurrentSection].ShowAllTasks();

                    Console.Write(" Выберите индекс удаляемой задачи: ");
                    int index = int.Parse(Console.ReadLine());

                    sections[CurrentSection].section.RemoveAt(index);
                    break;
                case "3":
                    Console.Write(" Введите имя нового раздела: ");
                    string name = Console.ReadLine();

                    sections.Add(new Section(name));
                    Console.WriteLine("Вы добавили новый раздел с названием \"" + name + "\"");
                    break;
                case "4":
                    {
                        ShowSections();

                        Console.Write(" Выберите раздел для удаления (все задачи переместятся раздел main): ");
                        string userSection = Console.ReadLine();

                        if (int.TryParse(userSection, out CurrentSection) && --CurrentSection >= 0)
                        {
                            for (int i = 0; i < sections[CurrentSection].section.Count; i++)
                                sections[0].section.Add(sections[CurrentSection].section[i]);

                            sections.RemoveAt(CurrentSection);
                            CurrentSection = 0;
                            Console.WriteLine("Удаление прошло успешно");
                        }
                        else
                            Console.WriteLine("Вы ввели некорректное, либо отрицательное значение");
                    }
                    break;
                case "5":
                    {
                        ShowSections();

                        Console.Write(" Выберите раздел в который хотите перейти: ");
                        string userSection = Console.ReadLine();

                        if (int.TryParse(userSection, out CurrentSection) && --CurrentSection >= 0)
                            Console.WriteLine("Вы выбрали раздел: " + sections[CurrentSection].Name);
                        else
                            Console.WriteLine("Вы ввели некорректное, либо отрицательное значение");
                    }
                    break;
                case "6":
                    string json = JsonSerializer.Serialize(sections, jsonOptions);
                    File.WriteAllText(path, json);
                    break;
                case "7":
                    GetDataFromDataBase();
                    break;
                default:
                    Console.WriteLine("Вы ввели неверную команду!");
                    break;
            }
            
            Console.Clear();
        }
    }
    static void GetDataFromDataBase()
    {
        string json = File.ReadAllText(path);
        if (json != null && json != "")
            sections = JsonSerializer.Deserialize<List<Section>>(json, jsonOptions);
    }
    static void ShowSections()
    {
        Console.WriteLine(" Все разделы: ");
        for (int i = 0; i < sections.Count; i++)
        {
            Console.WriteLine("  " + (i + 1) + " - " + sections[i].Name);
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
}