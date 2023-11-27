using System.Reflection;

using Newtonsoft.Json;


namespace AutoLedgeBook.Shared
{
    public static class GitVersionUtils
    {
        private const string GitVersionTypeName = "GitVersionInformation";

        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static GitVersionModel GetFromAssembly(Assembly asm)
        {
            if (asm is null)
            {
                throw new ArgumentNullException(nameof(asm));
            }

            Type versionType = asm.GetType(GitVersionTypeName) ?? throw new NotSupportedException($"Сборка \"{asm}\" не содержит в себе тип \"{GitVersionTypeName}\" как следствие не может определить версию приложения.");

            string branchName = GetMemberValue<string>(versionType, nameof(GitVersionModel.BranchName));
            string sha = GetMemberValue<string>(versionType, nameof(GitVersionModel.Sha));
            string shortSha = GetMemberValue<string>(versionType, nameof(GitVersionModel.ShortSha));
            string informationalVersion = GetMemberValue<string>(versionType, nameof(GitVersionModel.InformationalVersion));

            return new GitVersionModel(branchName, sha, shortSha, informationalVersion);
        }

        public static GitVersionModel Copy(IGitVersionModel model)
        {
            if (model is null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return new GitVersionModel
            {
                BranchName = model.BranchName,
                Sha = model.Sha,
                ShortSha = model.ShortSha,
                InformationalVersion = model.InformationalVersion
            };
        }

        private static T GetMemberValue<T>(Type type, string memberName)
        {
            MemberInfo member = type.GetMember(memberName).FirstOrDefault() ?? throw new MissingMemberException(memberName);
            
            object? value = member.MemberType switch
            {
                MemberTypes.Property => ((PropertyInfo)member).GetValue(null),
                MemberTypes.Field => ((FieldInfo)member).GetValue(null),
                _ => throw new InvalidOperationException()
            };

            return (T)value!;
        }
    }

    public static class GitVersionModelUtils
    {
        public static bool Compare(this IGitVersionModel model, IGitVersionModel other)
        {
            if (other is null)
            {
                return false;
            }

            return model.InformationalVersion == other.InformationalVersion;
        }
    }

    public interface IGitVersionModel
    {
        string BranchName { get; }
        string InformationalVersion { get; }
        string Sha { get; }
        string ShortSha { get; }
    }

    public class GitVersionModel : IGitVersionModel
    {
        public GitVersionModel(string branchName, string sha, string shortSha, string informationalVersion)
        {
            BranchName = branchName;
            Sha = sha;
            ShortSha = shortSha;
            InformationalVersion = informationalVersion;
        }

        [JsonConstructor]
        internal GitVersionModel() { }

        [JsonProperty("BranchName")]
        public string BranchName { get; internal set; }

        [JsonProperty("Sha")]
        public string Sha { get; internal set; }

        [JsonProperty("ShortSha")]
        public string ShortSha { get; internal set; }

        [JsonProperty("InformationalVersion")]
        public string InformationalVersion { get; internal set; }

        
    }
}