using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Data
{
    class PersonRepository: IPersonRepository
    {
        public IList<Person> Satisfies(ISpecification<Person> personSpec)
        {
            var list = new List<Person>();
            return list.Where(personSpec.GetLinqExpression()).ToList();
        }
    }
}
