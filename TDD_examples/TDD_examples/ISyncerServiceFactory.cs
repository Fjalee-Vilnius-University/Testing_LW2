using System;
using System.Threading.Tasks;

namespace TDD_examples
{
    public class ISyncerServiceFactory
    {
        public ISyncerServiceFactory()
        {
        }

        public Task<ISyncerService> CreateSyncer(object downloadedVideosListFile, object fileExtensions, string testOutputDir, string testPlaylistUrl)
        {
            throw new NotImplementedException();
        }
    }
}