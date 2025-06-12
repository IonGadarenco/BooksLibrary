using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooksLibrary.Application.Commun.Exceptions
{
    public class EntityNotExistException : Exception
    {
        public EntityNotExistException(string entity, int id): base($"Entity {entity} with id {id} does not exist") { }
    }
}
