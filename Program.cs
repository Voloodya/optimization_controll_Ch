using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//На выходе поточной линии готовые изделия проходят функциональный контроль на специальных однотипных стендах.Изделия сходят с конвей-ера в случайные интервалы времени, распределенные равномерно на ин-тервале[a, b] мин.
//Если все стенды к моменту поступления изделия на контроль оказыва-ются занятыми, то изделие остается непроверенным и в таком виде мо-жет поступить потребителю.При наличии хотя бы одного свободного стенда изделие подвергается контролю. Каждый из стендов одновремен-но может контролировать только одно изделие. Время контроля на лю-бом стенде случайное и распределено также равномерно на интервале[c, d] мин.
//Требуется определить количество контрольных стендов, которые необ-ходимо установить на выходе поточной линии, чтобы обеспечить кон-троль не менее Р % готовых изделий.

namespace optimization_controller_all
{

    abstract class UpdateTime
    {
        public abstract void update_time();
    }
    enum Status
    {
        Free = 1,
        Busy = 2
    }
    class Performance
    {
        public int min_time;
        public int max_time;

        public Performance(int min_time, int max_time)
        {
            this.min_time = min_time;
            this.max_time = max_time;
        }
    }

    class Mechanism : UpdateTime
    {
        Performance performance;
        public Status status;
        //Время работы контроллера
        public int task_time;

        public Mechanism(Performance performance)
        {
            this.performance = performance;
            this.status = Status.Free;
            this.task_time = 0;
        }

        public void set_task()
        {
            this.status = Status.Busy;
            this.task_time = get_rnd_time();
        }

        public int get_rnd_time()
        {
            Random rnd = new Random();
            return rnd.Next(this.performance.min_time, this.performance.max_time);
        }
        public override void update_time()
        {
            if (this.task_time > 0)
            {
                this.task_time -= 1;
                if (this.task_time == 0)
                {
                    this.status = Status.Free;
                }
            }
        }

    }

    class Perforator : Mechanism
    {
        public Perforator(Performance performance) : base(performance)
        {
        }
    }

    class PiopleFactory : UpdateTime
    {
        public Performance performance;
        public int time_next_piople;
        public bool have_new_piople;

        public PiopleFactory(Performance performance)
        {
            this.performance = performance;
            this.time_next_piople = 1;
            this.have_new_piople = true;
        }

        public override void update_time()
        {
            this.have_new_piople = false;
            if (this.time_next_piople > 0)
            {
                this.time_next_piople -= 1;
                if (this.time_next_piople == 0)
                {
                    set_new_piople();
                }
            }
            else
            {
                Console.WriteLine("Your time is negative");
            }
        }

        public void set_new_piople()
        {
            this.have_new_piople = true;
            Random rnd = new Random();
            this.time_next_piople = rnd.Next(this.performance.min_time, this.performance.max_time);
        }
    }
    class Program
    {
        public static bool any_perforator_avaliable(List<Perforator> perforators)
        {
            foreach (Perforator p in perforators)
            {
                if (p.status == Status.Free)
                {
                    return true;
                }
            }
            return false;
        }

        public static Perforator find_avaliable_perforator(List<Perforator> perforators)
        {
            for (int i = 0; i < perforators.Count; i++)
            {
                if (perforators[i].status == Status.Free)
                {
                    return perforators[i];
                }
            }
            return null;
        }
        static void Main(string[] args)
        {
            int Time_Global = 0;

            //Perforator perforator1 = new Perforator(new Performance(10, 40));
            //Perforator perforator2 = new Perforator(new Performance(15, 50));
            //Perforator perforator3 = new Perforator(new Performance(20, 80));
            //Perforator perforator4 = new Perforator(new Performance(20, 80));

            List<Perforator> perforators = new List<Perforator>();

            int count_of_tasks = 20;
            int count_of_processed_tasks = 0;
            int count_of_rejected_tasks = 0;
            double target_productivity;
            double rezult_productive = 0;

            Console.WriteLine("Введите целевую производительность системыконтроля (целая часть числа от дробной отделяется ' , ':");
            target_productivity = Convert.ToDouble(Console.ReadLine());

            PiopleFactory humanFactory = new PiopleFactory(new Performance(8, 12));
            while (rezult_productive < target_productivity)
            {
                perforators.Add(new Perforator(new Performance(10, 40)));

                count_of_processed_tasks = 0;
                count_of_rejected_tasks = 0;

                while (((count_of_processed_tasks + count_of_rejected_tasks) < count_of_tasks))
                {
                    Time_Global += 1;

                    //Обновляем время для всего
                    for (int i = 0; i < perforators.Count; i++)
                    {
                        perforators[i].update_time();
                    }

                    humanFactory.update_time();
                    Console.WriteLine("Время:  " + Time_Global);

                    //человек может прийти в любой момент времени
                    if (humanFactory.have_new_piople)
                    {
                        Console.WriteLine("Пришел новый человек");
                        if (any_perforator_avaliable(perforators))
                        {
                            Console.WriteLine("Есть свободный перфоратор");
                            count_of_processed_tasks += 1;
                            Perforator perforator_avaliable = find_avaliable_perforator(perforators);
                            perforator_avaliable.set_task();
                        }
                        else
                        {
                            Console.WriteLine("Нет свободного перфоратора");
                            count_of_rejected_tasks += 1;
                        }

                    }

                }
                Console.WriteLine("Количество контроллеров: " + perforators.Count);
                Console.WriteLine();
                Console.WriteLine("Количество обработанных заявок: " + count_of_processed_tasks);
                Console.WriteLine();
                Console.WriteLine("Количество отклоненных заявок: " + count_of_rejected_tasks);
                Console.WriteLine();
                rezult_productive = (Convert.ToDouble(count_of_processed_tasks) - count_of_rejected_tasks) / (Convert.ToDouble(count_of_processed_tasks) + count_of_rejected_tasks);
                Console.WriteLine("Производительность (доля отработанных заявок): " + rezult_productive);
            }
        }
    }
}
