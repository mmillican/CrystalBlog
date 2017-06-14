using System.ComponentModel.DataAnnotations;

namespace CrystalBlog.Entities.Sites
{
    public class Site
    {
        public int Id { get; set; }

        [Required, MaxLength(15)]
        public string Key { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Theme { get; set; }

        [MaxLength(255)]
        public string Hostnames { get; set; }
    }
}
