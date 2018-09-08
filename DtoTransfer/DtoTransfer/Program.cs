using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace DtoTransfer
{
    class Base : AutoMapper.Profile
    {


    }

    class Program
    {
        static void Main(string[] args)
        {

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<StuA, Stu>();

            });
            var st = new Stu()
            {
                Id = 1,
                Name = "aasdf",
                Age = 4,
                Money = 100
            };
            var strA = new StuA()
            {
                Age = 555555,
                Name = "DDDDDDDDDDDDDD",
                Money = 12
            };

            var time = 1000000;
            var start = DateTime.Now;
            for (int i = 0; i < time; i++)
            {
                var tem = CloneHelper<StuA, Stu>.Update(strA, st);
            }
            Console.WriteLine("表达式树:" + (DateTime.Now - start).Milliseconds);

            var startB = DateTime.Now;
            for (int i = 0; i < time; i++)
            {
                var tem = AutoMapper.Mapper.Map<StuA, Stu>(strA, st);
            }

            Console.WriteLine("AutoMapper:" + (DateTime.Now - startB).Milliseconds);


    

    



            Console.ReadKey();
        }
    }

    class Stu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public double Money { get; set; }

    }

    public class StuA
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Money { get; set; }

    }
}
