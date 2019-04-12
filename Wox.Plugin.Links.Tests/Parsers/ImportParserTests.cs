using System.Linq;
using FluentAssertions;
using NSubstitute;
using Wox.Plugin.Links;
using Wox.Plugin.Links.Parsers;
using Xunit;

namespace Wox.Links.Tests.Parsers {
    public class ImportParserTests {
        public ImportParserTests() {
            var storage = Substitute.For<IStorage>();
            _fileService = Substitute.For<IFileService>();
            _saveParser = new ImportParser(storage, _fileService);
            _queryInstance = @"link C:\file.json".AsQuery();
        }

        private const string FilePath = @"C:\file.json";
        private readonly ImportParser _saveParser;
        private readonly IFileService _fileService;
        private readonly IQuery _queryInstance;

        [Fact]
        public void ImportedFileExisting_ReturnFalse() {
            _fileService.FileExists(FilePath).Returns(true);
            _fileService.GetExtension(FilePath).Returns(".json");
            _saveParser.TryParse(_queryInstance, out var results).Should()
                .BeTrue();
            results.Should().HaveCount(1);
            results.Single().Title.Should().Be("Target configuration to 'file.json' file");
        }

        [Fact]
        public void ImportedFileNotExisting_ReturnFalse() {
            _fileService.FileExists(FilePath).Returns(false);

            _saveParser.TryParse(_queryInstance, out var results).Should()
                .BeFalse();
        }
    }
}