using System.ComponentModel.DataAnnotations;


namespace CodingMilitia.Utils.Pagination.EFCore.Tests.Mocks
{
    public class DbEntity
    {

        [Key]
        public int Key { get; set; }

        public override bool Equals(object obj)
        {
            return Key.Equals(((DbEntity)obj).Key);
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
