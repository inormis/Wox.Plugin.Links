using System.Collections.Generic;
using System.Windows.Forms;

namespace Wox.Plugin.Links.Parsers {
    public class ExportParser : BaseParser {
        private readonly IFileService _fileService;

        private readonly IStorage _storage;

        public ExportParser(IStorage storage, IFileService fileService) : base("export") {
            _fileService = fileService;
            _storage = storage;
        }

        protected override List<Result> Execute(IQuery query) {
            return new List<Result> {
                new Result {
                    Title = "Export links as a json file",
                    SubTitle = "Save dialog will popup",
                    Action = Export
                }
            };
        }

        private bool Export(ActionContext arg) {
            var content = _storage.ExportAsJsonString();
            var dialog = new SaveFileDialog {
                Filter = "Configuration file | *.json", DefaultExt = "json", FileName = "links.json"
            };
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK) {
                return _fileService.WriteAllText(dialog.FileName, content);
            }

            return true;
        }
    }
}