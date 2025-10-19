using System;
using System.IO;

class Task
{
    public string Name;
    public string Description;
    public DateTime CreationDate { get; private set; }
    public DateTime DeadLine;

    public Task(string name, DateTime deadLine, string description = "")
    {
        Name = name;
        Description = description;
        CreationDate = DateTime.Now;
        DeadLine = deadLine;
    }

    public override string ToString()
    {
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