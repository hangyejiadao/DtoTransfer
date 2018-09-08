#年纪大了,苦于AutoMapper的繁琐配置,不想敲太多代码,于是写了 CloneHelper 基于表达式树的c#深拷贝帮助类 性能比AutoMapper高 且不需要配置
这是测试代码
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

            var time = 10000000;
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
</code>
结果输出    表示树:336 AutoMapper:645
