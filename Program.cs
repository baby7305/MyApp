using System;

namespace MyApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var hello = new Hello();
            hello.test1();
            var resultTask = hello.test3();
            // resultTask.Wait(5000);
            var result = resultTask.Result;
            if (result)
            {
                Console.WriteLine("摄像头已连接");
            }

            Console.WriteLine("Hello World!");
        }
    }
}
