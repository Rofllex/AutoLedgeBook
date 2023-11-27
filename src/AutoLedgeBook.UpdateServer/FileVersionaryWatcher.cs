using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.IO.Compression;
using AutoLedgeBook.Shared;

namespace AutoLedgeBook.UpdateServer
{
    public class FileVersionaryWatcher
    {
        private FileSystemWatcher _watcher;
        private readonly string _mainFileName;
        private AssemblyLoadContext? _asmContext;
        private GitVersionInformationModel? _currentModel;

        private DateTime _lastFileChanged = DateTime.MinValue;
        private readonly TimeSpan _minChangedDelay = TimeSpan.FromSeconds(5);

        public FileVersionaryWatcher(string filePath, string mainFilePath)
        {
            if (File.Exists(filePath) == false)
            {
                throw new FileNotFoundException(filePath);
            }

            string directoryName = Path.GetDirectoryName(filePath)!;
            string fileName = Path.GetFileName(filePath);

            FullZipPath = filePath;

            _mainFileName = mainFilePath;

            _watcher = new FileSystemWatcher(directoryName, fileName);
            _watcher.EnableRaisingEvents = true;
            _watcher.Changed += OnWatcherChanged;
            _watcher.NotifyFilter = NotifyFilters.LastWrite;

            OnWatcherChanged(_watcher, new FileSystemEventArgs(WatcherChangeTypes.Changed, directoryName, fileName));
        }

        public GitVersionInformationModel Model
        {
            get => _currentModel;
            set
            {
                if (value != _currentModel)
                {
                    _currentModel = value;
                    InvokeModelChanged(value);
                }
            }
        }

        public event EventHandler<GitVersionInformationModel>? ModelChanged;

        public string FullZipPath { get; }

        private void OnWatcherChanged(object sender, FileSystemEventArgs e)
        {
            if (_lastFileChanged + _minChangedDelay > DateTime.UtcNow)
            {
                return;
            }

            _lastFileChanged = DateTime.UtcNow;

            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                if (_asmContext != null)
                {
                    _ = Task.Run(() =>
                    {
                        _asmContext.Unload();
                        _asmContext = null;
                    });
                }


                _asmContext = LoadAssembly(e.FullPath, _mainFileName, out Assembly asm);
                var currentModel = new GitVersionInformationModel(asm.GetType("GitVersionInformation")!);
                _asmContext.Unloading += (_) =>
                {
                    currentModel.Dispose();
                };
                Model = currentModel;
            }
        }

        private AssemblyLoadContext LoadAssembly(string archivePath, string mainFileName, out Assembly asm)
        {
            using var fs = File.Open(archivePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using ZipArchive archive = new(fs, ZipArchiveMode.Read, false);


            //using var archive = ZipFile.Open(archivePath, ZipArchiveMode.Read);
            var entry = archive.GetEntry(mainFileName) ?? throw new FileNotFoundException(mainFileName);

            string tempFile = Path.GetTempFileName();

            entry.ExtractToFile(tempFile, true);

            AssemblyLoadContext ctx = new(mainFileName, true);
            asm = ctx.LoadFromAssemblyPath(tempFile);

            return ctx;
        }

        private void InvokeModelChanged(GitVersionInformationModel model) => ModelChanged?.Invoke(this, model);
    }

    public class GitVersionInformationModel : IDisposable, IGitVersionModel
    {
        private Type _type;
        private bool _disposed;

        public GitVersionInformationModel(Type type)
        {
            _type = type;
        }

        public string BranchName => GetTypeMemberValue1<string>(_type);

        public string Sha => GetTypeMemberValue1<string>(_type);

        public string ShortSha => GetTypeMemberValue1<string>(_type);

        public string InformationalVersion => GetTypeMemberValue1<string>(_type);

        public void Dispose()
        {
            ThrowIfDisposed();

            _type = null;
            _disposed = true;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(GitVersionInformationModel));
            }
        }

        private T GetTypeMemberValue1<T>(Type type, [CallerMemberName] string memberName = "") => GetTypeMemberValue<T>(type, memberName);

        private T GetTypeMemberValue<T>(Type type, string memberName)
        {
            ThrowIfDisposed();

            MemberInfo member = type.GetMember(memberName).FirstOrDefault() ?? throw new MissingMemberException(memberName);

            object? value = member.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)member).GetValue(null),
                MemberTypes.Field => ((FieldInfo)member).GetValue(null),
                _ => throw new InvalidOperationException()
            };

            return (T)value;
        }
    }
}
