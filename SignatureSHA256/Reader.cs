using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Security.Cryptography;
using System.Diagnostics;

namespace SignatureSHA256
{
    class Reader
    {

        #region Приватные свойства

        private Stopwatch myStopwatch = new System.Diagnostics.Stopwatch();
        private  readonly object Lock = new object();
        private long offset = 0;
        private readonly int blockSize;
        private byte[] buffer;
        private byte[] byteArray;
        private StringBuilder sb = new StringBuilder();
        private int blockNum=0;
        private readonly long LenghtFile = 0;
        private SHA256 sha = SHA256.Create();
        private FileStream fs;

        #endregion

        #region Публичные свойства

        public bool Closed = false;

        #endregion

        #region Конструктор

        public Reader(string _path, int _blockSize)
        {
            blockSize = _blockSize;
            buffer = new byte[_blockSize];
            fs = File.OpenRead(_path);
            myStopwatch.Start();
            Closed = false;
            LenghtFile = fs.Length;
        }

        #endregion

        #region Методы

        /// <summary>
        /// Основной метод для работы с файлом и расчётом хэша из блоков. Метод было решено объединить в один, т.к.
        /// при расчёте в одном методе увеличивалась скорость обработки.
        /// </summary>
        public void Read()
        {
            while (offset < LenghtFile - 2*blockSize && !Closed) 
                {
                Monitor.Enter(Lock);
                try
                {
                    if (Closed || offset > LenghtFile - blockSize)
                        break;
                    buffer = new byte[blockSize];
                    fs.Seek(offset, SeekOrigin.Begin);
                    fs.Read(buffer, 0, blockSize);


                    byteArray = sha.ComputeHash(buffer);
                    sb.Clear();
                    foreach (byte a in byteArray)
                    {
                        //Преобразование a в шестнадцатеричный формат
                        sb.Append(a.ToString("x2"));
                    }
                    Interlocked.Increment(ref blockNum);

                    Console.WriteLine("\nБлок №" + blockNum + " : " + sb);
                    Interlocked.Add(ref offset, blockSize);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("\n Исключение в потоке: " + Thread.CurrentThread.GetHashCode().ToString());
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(new System.Diagnostics.StackTrace().ToString());
                    Console.ReadKey();
                }
                finally
                {
                    Monitor.Exit(Lock);
                }
            }
            Closed = true;
            myStopwatch.Stop();
        }

        /// <summary>
        /// Побочный метод для закрытия FileStream и вывода информации на экран
        /// </summary>
        public void Close()
        {
            fs.Close();
            Console.WriteLine("\nКонец файла. Всего блоков - " + blockNum + "(" + (blockNum + 1) + ")");
            Console.WriteLine("\nВремя выполнения - " + myStopwatch.ElapsedMilliseconds.ToString());
        }

        #endregion

    }
}
