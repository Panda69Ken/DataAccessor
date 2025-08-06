using My.Framework.MySQLAccessor;
using MyTest.Model;

namespace MyTest.Core
{
    public class UserRepository : RepositoryBase<UserEntity>
    {
        public UserRepository(IContextContainer contextContainer) : base(contextContainer)
        {
        }
    }
}
