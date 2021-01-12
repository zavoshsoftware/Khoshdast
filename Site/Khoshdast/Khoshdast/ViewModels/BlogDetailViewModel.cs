using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class BlogDetailViewModel : _BaseViewModel
    {
        public List<Blog> SidebarRecentBlogs { get; set; }
        public Blog Blog { get; set; }
        public List<BlogGroup> SidebarBlogGroups { get; set; }
        public List<BlogComment> BlogComments { get; set; }
        public int BlogCommentsCount { get { return BlogComments.Count(); } }

        public List<Blog> RelatedBlogs { get; set; }
    }
}