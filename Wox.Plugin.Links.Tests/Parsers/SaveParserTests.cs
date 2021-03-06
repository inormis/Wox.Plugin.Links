﻿using System.Linq;
using FluentAssertions;
using NSubstitute;
using Wox.Plugin;
using Wox.Plugin.Links;
using Wox.Plugin.Links.Parsers;
using Wox.Plugin.Links.Services;
using Xunit;

namespace Wox.Links.Tests.Parsers {
    public class SaveParserTests {
        private readonly SaveParser _saveParser;
        private readonly IStorage _storage;
        private readonly IFileService _fileService;
        private readonly IClipboardService _clipboard;

        public SaveParserTests() {
            _storage = Substitute.For<IStorage>();
            _fileService = Substitute.For<IFileService>();
            _clipboard = Substitute.For<IClipboardService>();
            _saveParser = new SaveParser(_storage, _fileService, _clipboard);
        }

        [Theory]
        [InlineData("-l", "")]
        [InlineData("-l", "this is nice")]
        [InlineData("link", "")]
        [InlineData("link", "some nice description")]
        public void SaveTerms_ReturnTrueAndProposeToSave(string key, string description = "") {
            _saveParser.TryParse((key + " Shortcut https://some.com/link-@@ |" + description).AsQuery(),
                    out var results)
                .Should().BeTrue();
            results.Should().HaveCount(1);

            var result = results[0];
            result.Title.Should().Be($"Save the link as 'Shortcut': '{description}'");
            result.SubTitle.Should().Be("https://some.com/link-@@");

            result.Action(new ActionContext());
            _storage.Received(1).Set("Shortcut", LinkType.Path, "https://some.com/link-@@", description);
        }

        [Fact]
        public void Save_FileLinkContainsSpace_ReturnsSuccessResult() {
            _fileService.FileExists(@"C:\Program Files\Anki\anki.exe").Returns(true);
            _saveParser.TryParse(@"link myeditor C:\Program Files\Anki\anki.exe | My editor".AsQuery(),
                    out var results)
                .Should().BeTrue();
            results.Should().HaveCount(1);

            var result = results[0];
            result.Title.Should().Be("Save the link as 'myeditor': 'My editor'");
            result.SubTitle.Should().Be(@"C:\Program Files\Anki\anki.exe");

            result.Action(new ActionContext());
            _storage.Received(1).Set("myeditor", LinkType.Path, @"C:\Program Files\Anki\anki.exe", "My editor");
        }

        [Fact]
        public void Save_DirectoryLinkContainsSpace_ReturnsSuccessResult() {
            _fileService.DirectoryExists(@"C:\Program Files").Returns(true);
            _saveParser.TryParse(@"link myeditor C:\Program Files | My editor".AsQuery(),
                    out var results)
                .Should().BeTrue();
            results.Should().HaveCount(1);

            var result = results[0];
            result.Title.Should().Be("Save the link as 'myeditor': 'My editor'");
            result.SubTitle.Should().Be(@"C:\Program Files");

            result.Action(new ActionContext());
            _storage.Received(1).Set("myeditor", LinkType.Path, @"C:\Program Files", "My editor");
        }

        [Theory]
        [InlineData("link1")]
        [InlineData("-link")]
        [InlineData("-l2")]
        public void NotSaveKeyWord_ReturnFalse(string key) {
            _saveParser.TryParse($"{key} https://some.com/link Shortcut".AsQuery(), out var results)
                .Should()
                .BeFalse();
            results.Should().HaveCount(0);
        }

        [Fact]
        public void LinkTemplate() {
            const string template = @"Select * from player";
            _clipboard.GetText().Returns(template);
            _saveParser.TryParse("link -t SelectAllPlayers".AsQuery(), out var results)
                .Should()
                .BeTrue();

            results.Should().HaveCount(1);
            var link = results.Single();
            link.Title.Should().Be("Save template 'SelectAllPlayers'");
            link.SubTitle.Should().Be(template);
            link.Action(new ActionContext());
            _storage.Received(1).Set("SelectAllPlayers", LinkType.ClipboardTemplate, template, Arg.Any<string>());
        }
    }
}