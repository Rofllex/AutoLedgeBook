using AutoLedgeBook.Shared;

using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace AutoLedgeBook.Updater
{
    internal class Program
    {
        private const string MAIN_FILE_NAME = "AutoLedgeBook.dll";
        private static readonly string ExecutableDirectoryPath;

        private static readonly Uri _remoteUriBase = new Uri("http://89.31.32.27:9090");
        private static readonly HttpClient _httpClient = new HttpClient();


        static Program()
        {
            ExecutableDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

            _httpClient.BaseAddress = _remoteUriBase;
        }



        static async Task Main(string[] args)
        {
            {
                Process[] startedProcesses = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(MAIN_FILE_NAME));

                if (startedProcesses.Length != 0)
                {
                    Console.WriteLine($"Невозможно обновить программу когда она запущена");
                    return;
                }
            }

            {
                if (File.Exists(MAIN_FILE_NAME) == false)
                {
                    Console.WriteLine("Файл не найден");
                    Console.ReadKey(true);
                    return;
                }
            }

            GitVersionModel? currentVersion = GetFileVersion(Path.Combine(ExecutableDirectoryPath, MAIN_FILE_NAME));

            if (currentVersion is null)
            {
                Console.WriteLine("Не удалось найти файл версионности");
                return;
            }

            GitVersionModel remoteVersion = await GetRemoteFileVersion();

            if (currentVersion.Compare(remoteVersion) == true)
            {
                Console.WriteLine($"Программа в обновлении не нуждается");
                return;
            }

            await UpdateApplication();
            Console.WriteLine("Обновление успешно завершено");
        }

        private static GitVersionModel GetFileVersion(string fullFilePath)
        {
            AssemblyLoadContext ctx = new(null, true);

            Assembly asm = ctx.LoadFromAssemblyPath(fullFilePath);

            GitVersionModel verModel = GitVersionUtils.GetFromAssembly(asm);
            ctx.Unload();

            return verModel;
        }

        private static async Task UpdateApplication()
        {
            var response = await _httpClient.GetAsync("Version/File");
            
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception();
            }

            Stream stream = await response.Content.ReadAsStreamAsync();
            ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read);
            archive.ExtractToDirectory(ExecutableDirectoryPath);
        }

        private static async Task<GitVersionModel> GetRemoteFileVersion()
        {
            var response = await _httpClient.GetAsync("Version/Get");

            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<GitVersionModel>(content)!;
            }

            throw new Exception($"Ошибка при выполнении запроса. StatusCode: {response.StatusCode}");
        }
    }

}