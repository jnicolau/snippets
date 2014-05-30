using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public interface IPersonRepository: IRepository
    {
        IList<Person> Satisfies(ISpecification<Person> person);
    }
}
