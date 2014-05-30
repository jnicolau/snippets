using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Domain;

namespace Data
{
    class OldPersonSpecification : Domain.OldPersonSpecification, ILinqExpression<Person>
    {
        public OldPersonSpecification(uint olderThan) : base(olderThan)
        {
        }

        public Func<Person, bool> GetExpression()
        {
            return person => person.Age > OlderThan;
        }
    }
}
