using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using CrystalBlog.Data;
using CrystalBlog.Entities.Posts;
using CrystalBlog.Entities.Sites;
using CrystalBlog.Entities.Users;
using CrystalBlog.ModelMappers;
using CrystalBlog.Models.BlogPosts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaasKit.Multitenancy;

namespace CrystalBlog.Controllers
{
    [Route("posts")]
    public class PostsController : Controller
    {
        private const int PageSize = 10; 

        private readonly CrystalDbContext _dbContext;
        private readonly Site _site;
        private readonly UserManager<User> _userManager;

        public PostsController(CrystalDbContext dbContext,
            ITenant<Site> site,
            UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _site = site?.Value;
            _userManager = userManager;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(int page = 1)
        {
            var model = new PostListViewModel();

            model.CurrentPage = page;
            model.PostCount = await _dbContext.BlogPosts
                .CountAsync(x => x.SiteId == _site.Id
                    && !x.IsDisabled
                    && !x.IsHidden);
            model.PageCount = model.PostCount.GetPageCountForResults(PageSize);
            var recordSkip = ExtensionMethods.CalculateRecordSkip(PageSize, page);

            model.Posts = await _dbContext.BlogPosts
                .Where(x => x.SiteId == _site.Id
                    && !x.IsDisabled
                    && !x.IsHidden)
                .OrderByDescending(x => x.PublishedOn)
                .Skip(recordSkip)
                .Take(PageSize)
                .ProjectTo<PostModel>()
                .ToListAsync();

            return View(model);
        }

        [Route("{slug}")]
        public async Task<IActionResult> ViewPost(string slug)
        {
            var post = await _dbContext.BlogPosts.SingleOrDefaultAsync(x =>
                x.SiteId == _site.Id && x.Slug == slug);

            var model = post.ToViewModel();

            return View(model);
        }

        [Authorize]
        [HttpGet("new")]
        public IActionResult Create()
        {
            var model = new EditPostViewModel();
            model.PublishedOn = DateTime.Now;

            return View(model);
        }

        [Authorize]
        [HttpPost("new")]
        public async Task<IActionResult> Create(EditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await DoesSlugExist(model.Slug))
            {
                ModelState.AddModelError(nameof(model.Slug), "Blog post URL already exists.  Choose a unique URL.");
                return View(model);
            }

            var currentUser = await _userManager.GetUserAsync(User);

            try
            {
                var post = new BlogPost
                {
                    SiteId = _site.Id,
                    Slug = model.Slug,
                    Title = model.Title,
                    PublishedOn = model.PublishedOn,
                    AuthorId = currentUser.Id,
                    Excerpt = model.Excerpt,
                    Body = model.Body,
                    IsHidden = model.IsHidden,
                    IsDisabled = model.IsDisabled
                };

                _dbContext.BlogPosts.Add(post);
                await _dbContext.SaveChangesAsync();

                // TODO: Display success
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [Authorize]
        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _dbContext.BlogPosts.FindAsync(id);
            if (post == null || post.SiteId != _site.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            var model = post.ToEditViewModel();

            return View(model);
        }

        [Authorize]
        [HttpPost("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, EditPostViewModel model)
        {
            var post = await _dbContext.BlogPosts.FindAsync(id);
            if (post == null || post.SiteId != _site.Id)
            {
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await DoesSlugExist(model.Slug, id))
            {
                ModelState.AddModelError(nameof(model.Slug), "Blog post URL already exists.  Choose a unique URL.");
                return View(model);
            }

            try
            {
                post.Slug = model.Slug;
                post.Title = model.Title;
                post.PublishedOn = model.PublishedOn;
                post.Excerpt = model.Excerpt;
                post.Body = model.Body;
                post.IsHidden = model.IsHidden;
                post.IsDisabled = model.IsDisabled;

                // TODO: Display success
                await _dbContext.SaveChangesAsync();

                return RedirectToAction(nameof(ViewPost), new { slug = post.Slug });
            }
            catch (Exception ex)
            {
                return View(model);
            }
        }

        [Authorize]
        [HttpPost("suggest-slug")]
        public async Task<IActionResult> SuggestSlug(string title)
        {
            try
            {
                var slug = title.Clean();
                var slugExists = await DoesSlugExist(slug);

                if (!slugExists)
                {
                    return Json(new { slug });
                }

                var slugBase = slug;
                var append = 1;
                while (slugExists)
                {
                    slug = slugBase + "-" + append;
                    slugExists = await DoesSlugExist(slug);
                }

                return Json(new { slug });
            }
            catch(Exception ex)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        private async Task<bool> DoesSlugExist(string slug, int? postId = null) =>
                        await _dbContext.BlogPosts.AnyAsync(x =>
                            x.SiteId == _site.Id
                            && (!postId.HasValue || x.Id != postId)
                            && x.Slug == slug);
    }
}