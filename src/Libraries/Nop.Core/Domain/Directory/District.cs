namespace Nop.Core.Domain.Directory
{
    public class District : BaseEntity
    {
        public int StateProvinceId { get; set; }
        public string Name { get; set; }

        public int DisplayOrder { get; set; }

        public virtual StateProvince StateProvince { get; set; }
    }
}
