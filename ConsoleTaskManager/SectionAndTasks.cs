using System;
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