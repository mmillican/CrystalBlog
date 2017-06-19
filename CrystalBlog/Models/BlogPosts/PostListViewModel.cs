using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrystalBlog.Models.BlogPosts
{
    public class PostListViewModel
    {
        public List<PostModel> Posts { get; set; }

        public int PostCount { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
    }
}
