using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CrystalBlog.Entities.Sites;
using CrystalBlog.Entities.Users;

namespace CrystalBlog.Entities.Posts
{
    public class BlogPost
    {
        public int Id { get; set; }

        public int SiteId { get; set; }

        [ForeignKey("SiteId")]
        public virtual Site Site { get; set; }

        [Required, MaxLength(255)]
        public string Slug { get; set; }
        
        [Required, MaxLength(255)]
        public string Title { get; set; }

        public DateTime PublishedOn { get; set; }

        public int AuthorId { get; set; }
        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; }

        [MaxLength(500)]
        public string Excerpt { get; set; }

        public string Body { get; set; }

        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }


    }
}
