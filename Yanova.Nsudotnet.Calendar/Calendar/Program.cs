using System;
using System.Globalization;

namespace Calendar
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введите дату: ");
            string dateString = Console.ReadLine();
            Console.WriteLine("\n");

            DateTime date;

            if(!DateTime.TryParse(dateString, out date))
            {
                Console.WriteLine("Некорректно указана дата.");
                return;
            }

            var names = CultureInfo.CurrentCulture.DateTimeFormat.AbbreviatedDayNames;
            Console.Write("{0}\t{1}\t{2}\t{3}\t{4}\t", names[(int)DayOfWeek.Monday], names[(int)DayOfWeek.Tuesday], names[(int)DayOfWeek.Wednesday], 
                                                       names[(int)DayOfWeek.Thursday], names[(int)DayOfWeek.Friday]);
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("{0}\t{1}\n", names[(int)DayOfWeek.Saturday], names[(int)DayOfWeek.Sunday]);

            Console.ForegroundColor = ConsoleColor.White;

            int firstDay = (int)(new DateTime(date.Year, date.Month, 1)).DayOfWeek;

            if(firstDay == 0)
            {
                firstDay = 7;
            }

            int nextLine = firstDay - 1;
            for(int i = 1; i < firstDay; ++i)
            {
                Console.Write("\t");
            }

            int workDays = DateTime.DaysInMonth(date.Year, date.Month);
            for (int i = 1; i <= DateTime.DaysInMonth(date.Year, date.Month); ++i)
            {
                if (new DateTime(date.Year, date.Month, i) == DateTime.Today)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                }
                else if(i == date.Day)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                }

                Console.Write(i);
                Console.BackgroundColor = ConsoleColor.Black;

                int weekDay = (int)new DateTime(date.Year, date.Month, i).DayOfWeek;
                if (weekDay == 0 || weekDay == 6)
                {
                    workDays--;
                }

                nextLine++;
                if (nextLine % 7 == 0)
                {
                    Console.WriteLine();
                }
                else
                {
                    Console.Write("\t");
                }
            }

            Console.WriteLine("\n");

            Console.WriteLine("Количество рабочих дней в месяце: {0}.", workDays);
            Console.ReadLine();
        }
    }
}
