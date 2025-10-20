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
        //TODO: добавить отображение для IsDone
        string str = "";
        str += "Имя задачи: " + Name + "\n";
        str += "Описание:\n" + Description + "\n";
        str += "Дата создания: " + CreationDate + "\n";
        str += "Дедлайн: " + CreationDate;
        return str;
    }
}
class TaskSection
{
    public string Name;
    public List<Task> Section;

    public TaskSection(string name, params Task[] tasks)
    {
        Name = name;
        for (int i = 0; i < tasks.Length; i++)
            Section.Add(tasks[i]);
    }
    public TaskSection(string name, List<Task> tasks)
    {
        Name = name;
        Section = tasks;
    }
}
class ConsoleManager
{
    static private string path = "DataBase.json";
    static void Main()
    {
        Console.Title = "Task Manager";


    }
    static Task AddTask()
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