using System.Collections.Generic;

namespace TDD_examples
{
    public interface IYoutubeDlService
    {
        List<RemoteFileModel> GetRemotePlaylistList(string testPlaylistUrl);
    }
}