

namespace BooksLibrary.Application.Common.Exceptions
{
    public class EntityAlreadyExistException : Exception
    {
        public EntityAlreadyExistException(string entity, int id):base($"Entity {entity} with id {id} already exist") { }
    }
}
