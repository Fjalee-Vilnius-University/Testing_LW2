using System.Threading.Tasks;

namespace TDD_examples
{
    public interface IRemoteYoutubeServiceFactory
    {
        Task<IRemoteYoutubeService> CreateRemoteYoutubeService(string testPlaylistUrl);
    }
}