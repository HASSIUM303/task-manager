using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

delegate void ToGraphics();

partial class ConsoleManager
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
        Console.CursorVisible = CursorVisible;
        ConsoleColor defaultColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        meth();
        Console.ForegroundColor = defaultColor;
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
}