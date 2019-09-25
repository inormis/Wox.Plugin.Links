using System;
using System.IO;
using System.Linq;
using Autofac;
using FluentAssertions;
using Newtonsoft.Json;
using NSubstitute;
using Wox.Plugin;
using Wox.Plugin.Links;
using Wox.Plugin.Links.Services;
using Xunit;

namespace Wox.Links.Tests {
    public class LinksIntegrationTests {
        private readonly IContainer _container;

        private const string Links = @"[
{
    ""Shortcut"": ""jp"",
        ""Path"": ""https://jira.gtoffice.lan/browse/IDPF-@@"",
        ""Description"": ""Open 'IDPF-@@' ticket""
    },
    {
    ""Shortcut"": ""jt"",
    ""Path"": ""https://jira.gtoffice.lan/browse/@@"",
    ""Description"": ""Open '@@' ticket""
},
{
""Shortcut"": ""jstatus"",
""Path"": ""https://jira.gtoffice.lan/secure/RapidBoard.jspa?rapidView=978&view=detail"",
""Description"": ""Open JIRA status board""
},
{
""Shortcut"": ""jmy"",
""Path"": ""https://jira.gtoffice.lan/secure/RapidBoard.jspa?rapidView=978&quickFilter=2961"",
""Description"": ""My tickets in Jira""
}
]";

        private const string PluginDirectory = @"C:\wox\plugins\links";

        private static readonly string ConfigurationPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Wox.Plugins.Links\config.json");

        private const string LinksPath = @"D:\links.json";

        internal IFileService FileService { get; }

        public IPluginContext PluginContext { get; }

        public LinksIntegrationTests() {
            FileService = Substitute.For<IFileService>();
            PluginContext = Substitute.For<IPluginContext>();

            FileService.FileExists(ConfigurationPath).Returns(true);
            FileService.FileExists(LinksPath).Returns(true);
            FileService.ReadAllText(LinksPath).Returns(Links);
            var configuration = JsonConvert.SerializeObject(new Configuration {
                LinksFilePath = LinksPath
            });
            FileService.ReadAllText(ConfigurationPath)
                .Returns(configuration);

            var builder = new ContainerBuilder();

            var pluginContext = Substitute.For<IPluginContext>();
            pluginContext.Directory.Returns(PluginDirectory);

            builder.RegisterModule(new AutofacModule(pluginContext));
            builder.RegisterInstance(FileService);
            _container = builder.Build();
        }

        [Fact]
        public void TypeExistingShortCut_ReturnExistingLink() {
            _container.Resolve<IEngine>()
                .Execute("jmy".AsQuery())
                .Should()
                .HaveCount(1);
        }

        [Fact]
        public void SaveNewLink() {
            var results = _container.Resolve<IEngine>()
                .Execute("link etw http://sm.com |some nice link".AsQuery());
            results.Should()
                .HaveCount(1);

            results.Single().Title.Should().Be("Save the link as 'etw': 'some nice link'");
        }

        [Fact]
        public void TargetNewFile() {
            var path = @"d:\files.json";
            FileService.FileExists(path)
                .Returns(true);

            FileService.GetExtension(path)
                .Returns(".json");

            FileService.ReadAllText(path)
                .Returns(JsonConvert.SerializeObject(new[] {new Link {Shortcut = "a1"}, new Link {Shortcut = "a2"}}));

            var results = _container.Resolve<IEngine>()
                .Execute($"link {path}".AsQuery());

            results.Should()
                .HaveCount(1);

            var importResult = results.Single();
            importResult.Title.Should().Be("Target configuration to 'files.json' file");
            importResult.Action.Invoke(new ActionContext());

            var links = _container.Resolve<IStorage>().GetLinks();
            links.Should().BeEquivalentTo(new Link {Shortcut = "a1"}, new Link {Shortcut = "a2"});
        }
    }
}