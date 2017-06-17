using System.Threading.Tasks;

namespace Poster.MetaWeblogApi
{
    public interface IMediaObjectProvider
    {
        Task<MediaObject> AddMediaObject(string blogId, NewMediaObject mediaObject);
    }
}
