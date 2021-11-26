using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TDD_examples;

namespace Youtube_Playlost_Manager.IntegrationTests
{
    [TestFixture]
    public class BetterSyncerUnitTests
    {

        private static readonly string _testPlaylistUrl = @"https://www.youtube.com/playlist?list=PLUMRha-TrvA4oRdeD97RZzhV2B0omUbao";
        private static readonly string _testOutputDir = @"C:\\Users\\Rytis\\Videos\\Test-Youtube-Playlist-Manager";

        private static IConfiguration _config;
        private static ISyncerService _syncer;
        private static IRemoteYoutubeService _remoteYoutubeService;
        private static IYoutubeDlService _youtubeDlService;
        private static string _fileNameYoutubeDlArchive;
        private static List<DownloadedFileModel> _initTestLocalItems;
        private static List<DownloadedFileModel> _currentExpectedLocalFiles;
        private static int _howManyItemsToTestDeleteRemotely = 2;
        private static int _howManyItemsToTestDeleteLocally = 2;
        private static List<string> _expectedYoutubeDlArchiveLines;

        private static List<string> _afterMockExpectedDownloadedFiles;
        private static List<string> _afterMockexpectedDownloadedIds;

        [OneTimeSetUp]
        public static void OneTimeSetUp()
        {
            DI.Initialize();
            _config = DI.Create<IConfiguration>();
            _remoteYoutubeService = DI.Create<IRemoteYoutubeServiceFactory>().CreateRemoteYoutubeService(_testPlaylistUrl).Result;
            _youtubeDlService = DI.Create<IYoutubeDlService>();

            _fileNameYoutubeDlArchive = $"{_testOutputDir}\\{_config.GetSection("youtubeDl")}";

            _initTestLocalItems = CreateDownloadedFilesModelsFromFilenames(new List<string>{
                    "2 Second Video.UO_QuXr521I.m4a",
                    "2-Sec. video.MmzadGwyYjY.m4a",
                    "Funny 2 second video.5DEdR5lqnDE.m4a",
                    "Shifuji Amazing 2 Sec Video.nkxMZbHnWmU.m4a",
                    "This Video Is 3 Seconds.s-MsZo02dos.m4a",
                    "Video 2sec.nQf3zHzftDI.m4a"
                }
            );

            _currentExpectedLocalFiles = _initTestLocalItems.ToList();

            _expectedYoutubeDlArchiveLines = _initTestLocalItems.Select(x => $"youtube {x.Id}").ToList();
            _expectedYoutubeDlArchiveLines.Sort();

            _syncer = CreateSyncerInstance();

            var initIds = _initTestLocalItems.Select(x => x.Id).ToList();
            if (!IsTestPlaylistAsExpected(initIds))
            {
                RestoreTestPlaylist();
            }

            DirectoryTool.DeleteIfExistsAndWaitUntilDeleted(_testOutputDir, true);

            _afterMockexpectedDownloadedIds = initIds;
            _afterMockExpectedDownloadedFiles = _initTestLocalItems.Select(x => x.Path).ToList();
        }

        [OneTimeTearDown]
        public static void OneTimeTearDown()
        {
            DirectoryTool.DeleteIfExists(_testOutputDir, true);
            RestoreTestPlaylist();
        }

        [Test, Order(1)]
        public void CheckIfTestDirectory_IsReady()
        {
            var files = Directory.GetFileSystemEntries(_testOutputDir, "*", SearchOption.AllDirectories);
            Assert.IsTrue(files.Length == 0);
        }

        [Test, Order(2)]
        public void CheckIfTestPlaylist_IsReady()
        {
            var initIds = _initTestLocalItems.Select(x => x.Id).ToList();
            Assert.True(IsTestPlaylistAsExpected(initIds));
        }

        [Test, Order(3)]
        public void DownloadInitialPlaylist()
        {
            _syncer.DownloadLocallyMissingRemoteItems();

            var downloadedFiles = Directory.GetFiles(_testOutputDir).ToList();

            var expectedFiles = _initTestLocalItems.Select(x => x.Path).ToList();
            expectedFiles.Add(_fileNameYoutubeDlArchive);

            expectedFiles.Sort();
            downloadedFiles.Sort();
            Assert.AreEqual(expectedFiles, downloadedFiles);
        }

        [Test, Order(4)]
        public void DeleteRemotePlaylistItem()
        {
            //Arrange
            var itemsToDelete = _currentExpectedLocalFiles.GetRange(0, _howManyItemsToTestDeleteRemotely);

            _currentExpectedLocalFiles.RemoveAll(x => itemsToDelete.Contains(x));

            //Act
            itemsToDelete.ForEach(x => _remoteYoutubeService.DeleteItem(x.Id));

            //Assert
            var expectedIds = _currentExpectedLocalFiles.Select(x => x.Id).ToList();
            Assert.True(IsTestPlaylistAsExpected(expectedIds));
        }

        [Test, Order(5)]
        public void RemoveLocalItemsDeletedRemotely()
        {
            //Act
            _syncer.RemoveLocalItemsDeletedRemotely();

            //Assert that files got removed
            var localFilesNames = Directory.GetFiles(_testOutputDir).ToList();
            var expectedFileNames = _currentExpectedLocalFiles.Select(x => x.Path).ToList();
            expectedFileNames.Add(_fileNameYoutubeDlArchive);

            localFilesNames.Sort();
            expectedFileNames.Sort();
            Assert.AreEqual(localFilesNames, expectedFileNames);
            Assert.True(YoutubeDlArchiveMatches(_currentExpectedLocalFiles));
        }

        [Test, Order(6)]
        public void RemoveRemoteItemsDeletedLocally()
        {
            //Arrange
            var filesToDelete = _currentExpectedLocalFiles.GetRange(0, _howManyItemsToTestDeleteLocally);
            filesToDelete.ForEach(f => File.Delete(f.Path));

            //Act
            _syncer.RemoveRemoteItemsDeletedLocally();
            _currentExpectedLocalFiles.RemoveRange(0, 2);

            //Asert that files got removed
            var remoteItems = _youtubeDlService.GetRemotePlaylistList(_testPlaylistUrl);
            var remoteItemIds = remoteItems.Select(x => x.Title).ToList();
            var expectedIds = _currentExpectedLocalFiles.Select(x => x.Id).ToList();

            remoteItemIds.Sort();
            expectedIds.Sort();
            Assert.AreEqual(expectedIds, remoteItemIds);
            Assert.True(YoutubeDlArchiveMatches(_currentExpectedLocalFiles));
        }

        private static bool YoutubeDlArchiveMatches(List<DownloadedFileModel> ids)
        {
            var currentIds = ReadIds(_fileNameYoutubeDlArchive);
            var expectedYoutubeDlArchive = ids.Select(x => $"youtube {x.Id}").ToList();

            currentIds.Sort();
            expectedYoutubeDlArchive.Sort();
            return currentIds.SequenceEqual(expectedYoutubeDlArchive);
        }

        private static List<DownloadedFileModel> CreateDownloadedFilesModelsFromFilenames(List<string> list)
        {
            var result = new List<DownloadedFileModel>();

            list.ForEach(name => result.Add(new DownloadedFileModel
            {
                Path = $"{_testOutputDir}\\{name}",
                Id = name.Split('.')[^2]
            }));

            return result;
        }

        private static List<string> ReadIds(string videoListFilePath)
        {
            var currentIds = new List<string>();
            using (var reader = new StreamReader(videoListFilePath))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    currentIds.Add(line);
                }
                currentIds.Sort();
            };
            return currentIds;
        }

        private static ISyncerService CreateSyncerInstance()
        {
            var downloadedVideosListFile = _config.GetSection("youtubeDl");
            var outputAudioFilesExtensions = _config.GetSection("output-formats:audio");
            var outputMultimediaFilesExtensions = _config.GetSection("output-formats:multimedia");
            var fileExtensions = outputMultimediaFilesExtensions;

            var syncerFactory = DI.Create<ISyncerServiceFactory>();
            return syncerFactory.CreateSyncer(downloadedVideosListFile, fileExtensions, _testOutputDir, _testPlaylistUrl).Result;
        }

        private static void RestoreTestPlaylist()
        {
            var currentPlaylistItemsIds = _youtubeDlService.GetRemotePlaylistList(_testPlaylistUrl).Select(x => x.Title).ToList();

            var initIds = _initTestLocalItems.Select(x => x.Id).ToList();
            var itemsToAdd = initIds.Where(x => !currentPlaylistItemsIds.Contains(x)).ToList();
            var itemsToRemove = currentPlaylistItemsIds.Where(x => !initIds.Contains(x)).ToList();

            itemsToAdd.ForEach(item => _remoteYoutubeService.AddItem(item));
            itemsToRemove.ForEach(item => _remoteYoutubeService.DeleteItem(item));
        }

        private static bool IsTestPlaylistAsExpected(List<string> expectedIdList)
        {
            var playlistItemsIds = _youtubeDlService
                .GetRemotePlaylistList(_testPlaylistUrl)
                .Select(x => x.Title)
                .ToList();

            playlistItemsIds.Sort();
            expectedIdList.Sort();
            return expectedIdList.SequenceEqual(playlistItemsIds);
        }

        [Ignore("Mock download was changed by real download")]
        [Test, Order(4)]
        [TestCase("m4a")]
        [TestCase("mp4")]
        public void AddMockDownloadedFile(string extension)
        {
            //Arrange
            var mockId = "mockId" + extension;
            var filePath = $"{_testOutputDir}\\mockRemotelyDeletedFile.{mockId}.{extension}";
            var videoListFilePath = _testOutputDir + "\\" + _config.GetSection("youtubeDl");

            //Act
            MockDownloadFile(mockId, filePath, videoListFilePath);

            //Asert file mock downloaded
            var localFiles = Directory.GetFiles(_testOutputDir).ToList();

            _afterMockExpectedDownloadedFiles.Add(filePath);

            localFiles.Sort();
            _afterMockExpectedDownloadedFiles.Sort();
            Assert.AreEqual(localFiles, _afterMockExpectedDownloadedFiles);

            //Asert id mock got added
            var currentIds = ReadIds(videoListFilePath);

            _afterMockexpectedDownloadedIds.Add($"youtube {mockId}");

            _afterMockexpectedDownloadedIds.Sort();
            currentIds.Sort();
            Assert.AreEqual(localFiles, _afterMockExpectedDownloadedFiles);
        }

        private void MockDownloadFile(string mockId, string filePath, string videoListFilePath)
        {
            File.Create(filePath).Close();
            File.AppendAllText(videoListFilePath, $"youtube {mockId}");
        }
    }
}
