using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public class Person: IAggregate
    {
        public Person(string name, uint age)
        {
            Name = name;
            Age = age;
        }

        public string Name { get; private set; }
        public uint Age { get; private set; }

    }
}
