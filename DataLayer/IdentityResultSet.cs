using Microsoft.AspNetCore.Identity;

namespace DataLayer
{
    public class IdentityResultSet<T>
    {
        public IdentityResult IdentityResult { get; private set; }
        public T Entity { get; private set; }

        public IdentityResultSet(IdentityResult identityResult, T entity)
        {
            IdentityResult = identityResult;
            Entity = entity;
        }

        public override string ToString()
        {
            return Entity.ToString();
        }
    }
}