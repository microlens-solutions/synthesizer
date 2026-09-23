using System;
using System.ComponentModel;

namespace Microlens.Synthesizer.Core.Shared {
    public class Registry {
        public const string ApplicationName = "Microlens Synthesizer";

        public const string PackageId = "699538DE-9133-4B2E-AB4D-42766EFB0B46";

        public const string MenuResourceId = "Menus.ctmenu";

        public const int MenuResourceVersion = 1;

        public static readonly Guid MenuGroupId = new Guid("8A6F8B16-7E2A-4C1B-9E7A-2E8A9B2C5F01");

        public const int FileCommandId = 0x0100;

        public const int FolderCommandId = 0x0101;

        public const int ProjectCommandId = 0x0102;

        public const string OutputExtension = ".cs";

        public const bool OptionsOverwriteExistingDefaultValue = true;

        public const int OptionsElementCountDefaultValue = 3;

        public const string OptionsFakerSuffixDefaultValue = "Faker";

        public const string OptionsLogFileNameDefaultValue = "microlens-synthesizer-{0:yyyyMMdd}.log";

        public const string OptionsLogFilePathDefaultValue = "";

        public const LogWritingMode OptionsLogWritingModeDefaultValue = LogWritingMode.SkippedOrFailed;

        public const string OptionsCategoryScaffoldingLabel = "Scaffolding";

        public const string OptionsCategoryLoggingLabel = "Logging";

        public const string OptionsOverwriteExistingLabel = "Overwrite Existing Fakers";

        public const string OptionsElementCountLabel = "Collection Element Count";

        public const string OptionsFakerSuffixLabel = "Faker Class Suffix";

        public const string OptionsLogFilePathLabel = "File Path";

        public const string OptionsLogWritingModeLabel = "Mode";

        public const string OptionsOverwriteExistingDescription = "When enabled, regenerating a Faker overwrites the existing file. When disabled, generation is skipped if the file already exists.";

        public const string OptionsElementCountDescription = "Number of elements generated for array, List<T>, HashSet<T>, and Dictionary<TKey, TValue> properties.";

        public const string OptionsFakerSuffixDescription = "Suffix appended to the source class name to build the generated Faker's class name, and to the source file name to build the output file name.";

        public const string OptionsLogFilePathDescription = "Full path of the log file. When left blank, the log is written next to the solution file.";

        public const string OptionsLogWritingModeDescription = "Controls when an entry is written to the log file.";

        public const string ErrorUnhandledException = "Something went wrong:\n{0}";

        public static readonly (string[] Suffixes, string Factory)[] Mappings = {
            (new[] { "Email" }, "Internet.Email"),
            (new[] { "PhoneNumber", "Phone", "Mobile" }, "Phone.PhoneNumber"),
            (new[] { "FirstName", "GivenName" }, "Name.FirstName"),
            (new[] { "LastName", "Surname", "FamilyName" }, "Name.LastName"),
            (new[] { "UserName", "Username" }, "Internet.UserName"),
            (new[] { "Url", "Website" }, "Internet.Url"),
            (new[] { "PostalCode", "ZipCode", "Zip" }, "Address.ZipCode"),
            (new[] { "City" }, "Address.City"),
            (new[] { "Country" }, "Address.Country"),
            (new[] { "CompanyName", "Company" }, "Company.CompanyName")
        };

        public enum CollectionKind {
            Array = 1,

            List = 2,

            HashSet = 3
        }

        public enum CommandSource {
            File = 1,

            Folder = 2,

            Project = 3
        }

        public enum ScaffoldStatus {
            [Description("Generated (and included in project)")]
            Included = 1,

            [Description("Generated (but excluded from project)")]
            Excluded = 2,

            [Description("Skipped (as already exists)")]
            Skipped = 3,

            [Description("Failed (couldn't generate)")]
            Failed = 4
        }

        public enum LogWritingMode {
            EveryTime = 1,

            MultipleFilesOnly = 2,

            SkippedOrFailed = 3,

            FailedOnly = 4
        }
    }
}
