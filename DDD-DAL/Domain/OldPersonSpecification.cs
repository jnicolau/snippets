using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    [ImplemmentedInDal]
    abstract public class OldPersonSpecification: ISpecification<Person>
    {
        protected uint OlderThan;

        public OldPersonSpecification(uint olderThan)
        {
            OlderThan = olderThan;
        }

        public bool IsSatisfiedBy(Person person)
        {
            return person.Age > OlderThan;
        }
    }
}
