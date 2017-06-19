using System;
using System.ComponentModel.DataAnnotations;

namespace CrystalBlog.Models.BlogPosts
{
    public class PostModel
    {
        public int Id { get; set; }

        public int SiteId { get; set; }
        
        [Required, MaxLength(255)]
        public string Slug { get; set; }

        [Required, MaxLength(255)]
        public string Title { get; set; }

        public DateTime PublishedOn { get; set; }

        public int AuthorId { get; set; }

        [MaxLength(500)]
        public string Excerpt { get; set; }

        public string Body { get; set; }

        public bool IsHidden { get; set; }
        public bool IsDisabled { get; set; }
    }

    public class EditPostViewModel : PostModel
    {

    }

    public class PostViewModel : PostModel
    {

    }
}
