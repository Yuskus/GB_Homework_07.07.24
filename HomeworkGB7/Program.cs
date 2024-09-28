using System;
using System.Text;
using System.Text.RegularExpressions;

namespace HomeworkGB7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string? startPath;
            string? extention;
            string? text;

            while (true)
            {
                Console.WriteLine("Где производится поиск?\n" + 
                                  "Введите имя диска (в формате \"D\") или директории (в формате \"D:\\some\\path\"):");
                startPath = Console.ReadLine();

                Console.WriteLine("Введите расширение (в формате \".cs\"):");
                extention = Console.ReadLine();

                Console.WriteLine("Введите искомый текст:");
                text = Console.ReadLine();

                if (startPath?.Length > 0 && !string.IsNullOrEmpty(extention) && !string.IsNullOrEmpty(text))
                {
                    if (!startPath.Contains(":\\")) startPath += ":\\";
                    break;
                }

                Console.WriteLine("Некорректный ввод. Попробуйте снова.");
            }

            var list = Search(startPath, extention);

            Console.WriteLine("Список файлов подходящего расширения на этом диске: ");
            Thread.Sleep(1000); //чтобы успеть прочитать консольный вывод
            Console.WriteLine(string.Join("\n", list));
            Console.WriteLine("Конец списка.");

            Console.WriteLine("Начало поиска текста в файлах:\n");
            Thread.Sleep(1000); //чтобы успеть прочитать консольный вывод
            list.ForEach(x => ReadUntil(x, text));
            Console.WriteLine("Конец поиска.");

            Console.WriteLine("Нажмите любую клавишу для выхода.");
            Console.ReadKey(true);
            Console.WriteLine("До свидания!");
            Thread.Sleep(1000);
        }
        static void ReadUntil(string path, string text) //поиск подходящей строки в файле и её вывод на консоль
        {
            StringBuilder line = new();
            Regex regex = new(text, RegexOptions.IgnoreCase);
            using (var sr = new StreamReader(path))
            {
                while (!sr.EndOfStream)
                {
                    line.Append(sr.ReadLine());
                    if (line.Length != 0 && line.ToString().Contains(text, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Console.WriteLine($"В файле по пути {path} найдена искомая строка:");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(regex.Replace(line.ToString(), text.ToUpper()).Trim() + "\n");
                        Console.ResetColor();
                    }
                    line.Length = 0;
                }
            }
        }
        static List<string> Search(string path, string name) //поиск файлов с подходящим расширением
        {
            List<string> list = new();
            DirectoryInfo dir = new(path);
            if (!TryGetFiles(dir, out FileInfo[] files)) return list;
            foreach (var file in files)
            {
                if (file.Extension == name)
                {
                    list.Add(file.FullName);
                }  
            }
            foreach (var item in dir.GetDirectories())
            {
                list.AddRange(Search(item.FullName, name));
            }
            return list;
        }
        static bool TryGetFiles(DirectoryInfo dir, out FileInfo[] files) //перехват ошибки доступа
        {
            bool gotAccess = true;
            files = [];
            try
            {
                files = dir.GetFiles();
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Ошибка доступа к файлу, текст ошибки: {ex.Message}");
                Console.ResetColor();
                gotAccess = false;
            }
            return gotAccess;
        }
    }
}
