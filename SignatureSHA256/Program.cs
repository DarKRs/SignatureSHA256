using System;
using System.Threading;

namespace SignatureSHA256
{
    class Program
    {

        static void  Main()
        {
            int nWorkerThreads;
            int nCompletionThreads;
            ThreadPool.GetMaxThreads(out nWorkerThreads, out nCompletionThreads);
            Console.WriteLine("Максимальное количество потоков: " + nWorkerThreads
                + "\nПотоков ввода-вывода доступно: " + nCompletionThreads);
            Console.WriteLine("Кол-во процессоров на компьютере {0}.", Environment.ProcessorCount);


            Console.WriteLine("Введите путь до файла для которого требуется создать сигнатуру:");
            string path = Console.ReadLine();
            Console.WriteLine("Введите на блоки какой длины требуется разделить файл:");
            int blockSize = Convert.ToInt32(Console.ReadLine());

            var rd = new Reader(path, blockSize);
             for(int i=0; i < Environment.ProcessorCount+1; i++)
             {
                new Thread(() => { rd.Read(); }).Start();
                //Thread.Sleep(100);
             }
            while (!rd.Closed)
            {
                try
                {
                    Thread.Sleep(100);
                }
                catch(Exception ex)
                {
                    Console.WriteLine("\n Исключение в потоке: " + Thread.CurrentThread.GetHashCode().ToString());
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(new System.Diagnostics.StackTrace().ToString());
                    Console.ReadKey();
                }
            }
            rd.Close();

            Console.ReadLine();
        }

    }



}
