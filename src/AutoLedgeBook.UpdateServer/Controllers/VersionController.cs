using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Newtonsoft.Json.Linq;



namespace AutoLedgeBook.UpdateServer.Controllers
{

#pragma warning disable CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.
    public class TraceFileOptions
    {
        public const string ConfigLocation = "TraceFileOptions";

        public string TraceZipArchive { get; set; }
        public string MainFileName { get; set; }
    }
#pragma warning restore CS8618 // Поле, не допускающее значения NULL, должно содержать значение, отличное от NULL, при выходе из конструктора. Возможно, стоит объявить поле как допускающее значения NULL.

    [Controller]
    public class VersionController : Controller
    {
        private readonly ILogger _logger;
        private readonly IOptions<TraceFileOptions> _options;
        private readonly FileVersionaryWatcher _watcher;

        public VersionController(ILogger<VersionController> logger, IOptions<TraceFileOptions> options, FileVersionaryWatcher watcher)
        {
            _logger = logger;
            _options = options;
            _watcher = watcher;
        }

        [HttpGet]
        public IActionResult Get()
        {
            GitVersionInformationModel model = _watcher.Model;
            var response = Shared.GitVersionUtils.Copy(model);
            return Json(response);
        }

        [HttpGet]
        public IActionResult File()
        {
            var fs = System.IO.File.OpenRead(_watcher.FullZipPath);
            string fileName = Path.GetFileName(_watcher.FullZipPath);
            return File(fs, "application/zip", fileName);
        }
    }
}
