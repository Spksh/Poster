﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poster.MetaWeblogApi
{
    public interface IBlogProvider
    {
        Task<List<Blog>> GetBlogs(string username);
    }
}
