using AutoMapper;
using CrystalBlog.Entities.Posts;
using CrystalBlog.Models.BlogPosts;

namespace CrystalBlog.ModelMappers
{
    public static class BlogPostMappers
    {
        internal static IMapper Mapper { get; }

        static BlogPostMappers()
        {
            Mapper = new MapperConfiguration(cfg => cfg.AddProfile<BlogPostMapperProfile>()).CreateMapper();
        }

        public static PostModel ToModel(this BlogPost post)
        {
            return Mapper.Map<PostModel>(post);
        }

        public static PostViewModel ToViewModel(this BlogPost post)
        {
            return Mapper.Map<PostViewModel>(post);
        }
        public static EditPostViewModel ToEditViewModel(this BlogPost post)
        {
            return Mapper.Map<EditPostViewModel>(post);
        }
    }

    public class BlogPostMapperProfile : Profile
    {
        public BlogPostMapperProfile()
        {
            CreateMap<BlogPost, PostModel>();
            CreateMap<BlogPost, EditPostViewModel>();
            CreateMap<BlogPost, PostViewModel>();
        }
    }
}
