using My.Framework.MySQLAccessor;

namespace MyTest.Model
{
    [Table("user")]
    public class UserEntity
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
