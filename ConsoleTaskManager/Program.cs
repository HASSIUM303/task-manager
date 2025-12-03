using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

partial class ConsoleManager
{
    static void Main()
    {
        try
        {
            GetDataFromDataBase();
        }
        catch (FileNotFoundException) { }
        catch (Exception e)
        {
            StylizeMessage(() =>
            {
                Console.WriteLine("Возникла ошибка при загрузке данных: ");
                Console.WriteLine(e.Message);
                Console.ReadKey();
            },
            ConsoleColor.Red, false);
        }

        while (true)
        {
            Console.Title = "Task Manager - " + sections[CurrentSection].Name;

            Console.WriteLine("1 - Добавить задачу в текущий раздел");
            Console.WriteLine("2 - Удалить задачу в текущем разделе\n");

            Console.WriteLine("3 - Создать новый раздел");
            Console.WriteLine("4 - Удалить раздел");
            Console.WriteLine("5 - Переключить раздел\n");

            Console.WriteLine("6 - Показать все задачи с разделами"); //TODO:
            Console.WriteLine("7 - Редактировать задачу");
            Console.WriteLine("8 - Поиск задачи\n");

            Console.WriteLine("9 - Сохранить изменения в базы данных");
            Console.WriteLine("10 - Получить данные из базы данных\n");

            Console.Write("Выбор опции: ");
            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    {
                        var task = CreateTask();
                        sections[CurrentSection].section.Add(task);

                        StylizeMessage(() =>
                        {
                            Console.WriteLine("В раздел " + sections[CurrentSection].Name +
                            ", успешно добавлена задача " + task.Name);
                            Console.ReadKey();
                        },
                        ConsoleColor.Green, false);
                    }
                    break;
                case "2":
                    {
                        sections[CurrentSection].ShowAllTasks();

                        Console.Write("Выберите индекс удаляемой задачи: ");
                        int index = int.Parse(Console.ReadLine());
                        string taskName = sections[CurrentSection].section[index].Name;

                        sections[CurrentSection].section.RemoveAt(index);

                        if (!sections[CurrentSection].section.Exists(t => t.Name == taskName))
                            StylizeMessage(() =>
                            {
                                Console.WriteLine($"Задача {taskName} успешно удалена из текущего раздела");
                                Console.ReadKey();
                            },
                            ConsoleColor.Green, false);
                    }
                    break;
                case "3":
                    {
                        Console.Write("Введите имя нового раздела: ");
                        string name = Console.ReadLine();

                        sections.Add(new Section(name));

                        StylizeMessage(() =>
                        {
                            Console.WriteLine("Вы успешно добавили новый раздел с названием \"" + name + "\"");
                            Console.ReadKey();
                        },
                        ConsoleColor.Green, false);
                    }
                    break;
                case "4":
                    {
                        ShowSections();

                        Console.Write(" Выберите раздел для удаления (все задачи переместятся раздел main): ");
                        string userSection = Console.ReadLine();

                        if (int.TryParse(userSection, out int sectionIndex) && --sectionIndex >= 0 && sectionIndex < sections.Count)
                        {
                            if (sectionIndex == 0)
                            {
                                StylizeMessage(() =>
                                {
                                    Console.WriteLine("Нельзя удалить основной раздел 'Main'.");
                                    Console.ReadKey();
                                },
                                ConsoleColor.Red, false);
                                break;
                            }

                            string sectionName = sections[sectionIndex].Name;

                            for (int i = 0; i < sections[sectionIndex].section.Count; i++)
                                sections[0].section.Add(sections[sectionIndex].section[i]);

                            sections.RemoveAt(sectionIndex);
                            CurrentSection = 0;

                            StylizeMessage(() =>
                            {
                                Console.Write($"Вы успешно удалили раздел {sectionName}");
                                Console.ReadKey();
                            },
                            ConsoleColor.Green, false);
                        }
                        else
                            StylizeMessage(() =>
                            {
                                Console.WriteLine("Вы ввели некорректное, либо отрицательное значение");
                                Console.ReadKey();
                            },
                            ConsoleColor.Red, false);
                    }
                    break;
                case "5":
                    {
                        ShowSections();

                        Console.Write("Выберите раздел в который хотите перейти: ");
                        string userSection = Console.ReadLine();

                        if (int.TryParse(userSection, out int sectionIndex) && --sectionIndex >= 0 && sectionIndex < sections.Count)
                        {
                            CurrentSection = sectionIndex;
                            StylizeMessage(() =>
                            {
                                Console.WriteLine("Вы успешно выбрали раздел " + sections[CurrentSection].Name);
                                Console.ReadKey();
                            },
                            ConsoleColor.Green, false);
                        }
                        else
                            StylizeMessage(() =>
                            {
                                Console.WriteLine("Вы ввели некорректное, либо отрицательное значение");
                                Console.ReadKey();
                            },
                            ConsoleColor.Red, false);
                    }
                case "6":
                    break;
                case "7":
                    break;
                case "8":
                    break;
                case "9":
                    try
                    {
                        string json = JsonSerializer.Serialize(sections, jsonOptions);
                        File.WriteAllText(path, json);
                    }
                    catch (Exception e)
                    {
                        StylizeMessage(() =>
                        {
                            Console.WriteLine("Возникла ошибка: ");
                            Console.WriteLine(e.GetType().Name);
                            Console.WriteLine(e.Message);
                        },
                        ConsoleColor.Red, false);
                    }
                    break;
                case "10":
                    try
                    {
                        GetDataFromDataBase();
                    }
                    catch (Exception e)
                    {
                        StylizeMessage(() =>
                        {
                            Console.WriteLine("Возникла ошибка: ");
                            Console.WriteLine(e.GetType().Name);
                            Console.WriteLine(e.Message);
                        },
                        ConsoleColor.Red, false);
                    }
                    break;
                default:
                    StylizeMessage(() =>
                    {
                        Console.Write("Вы ввели неверную команду!");
                        Console.ReadKey();
                    },
                    ConsoleColor.Red, false);
                    break;
            }

            Console.Clear();
        }
    }
}