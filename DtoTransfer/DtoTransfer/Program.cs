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
                cfg.CreateMap<StuA, Stu>().ForMember(des => des.Age, map =>
                  {

                  });

            });
            var st = new Stu()
            {
                Id = 1,
                Name = "aasdf",
                Age = 4,
                Money = 100
               ,AString ="asdfa"
            };
            var strA = new StuA()
            {
                Age = 555555,
                Name = "DDDDDDDDDDDDDD",
                Money = 12,
                BString = "AAAAAAAAAA"
            };

            var time = 1000000;
            var start = DateTime.Now;
            for (int i = 0; i < time; i++)
            {
                Console.WriteLine(st.Test);
                var tem = CloneHelper<StuA, Stu>.Update(strA, st).ForMember(d => d.AString,strA, s => s.BString);
                Console.WriteLine(tem.Test);
                Console.WriteLine();
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
        public string AString { get; set; }

        public string Test
        {
            get { return AString + "vsfvaffsdfasdfadsf"; }
        }
    }

    public class StuA
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public double Money { get; set; }
        public string BString { get; set; }
    }
}
