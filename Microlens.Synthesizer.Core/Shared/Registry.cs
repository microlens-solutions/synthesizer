using System;

namespace Microlens.Synthesizer.Core.Shared;

public class Registry {
    public const string ApplicationName = "Microlens Synthesizer";

    public const string PackageId = "699538DE-9133-4B2E-AB4D-42766EFB0B46";

    public const string MenuResourceId = "Menus.ctmenu";

    public const int MenuResourceVersion = 1;

    public static readonly Guid MenuGroupId = new("8A6F8B16-7E2A-4C1B-9E7A-2E8A9B2C5F01");

    public const int FileCommandId = 0x0100;

    public const int FolderCommandId = 0x0101;

    public const int ProjectCommandId = 0x0102;

    public const string OutputExtension = ".cs";

    public const bool OverwriteExisting = true;

    public const int ElementCount = 3;

    public const string FakerSuffix = "Faker";

    public const string ErrorUnhandledException = "Something went wrong:\n{0}";

    public static readonly (string[] Suffixes, string Factory)[] Mappings = [
        (["Email"], "Internet.Email"),
        (["PhoneNumber", "Phone", "Mobile"], "Phone.PhoneNumber"),
        (["FirstName", "GivenName"], "Name.FirstName"),
        (["LastName", "Surname", "FamilyName"], "Name.LastName"),
        (["UserName", "Username"], "Internet.UserName"),
        (["Url", "Website"], "Internet.Url"),
        (["PostalCode", "ZipCode", "Zip"], "Address.ZipCode"),
        (["City"], "Address.City"),
        (["Country"], "Address.Country"),
        (["CompanyName", "Company"], "Company.CompanyName")
    ];

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
        Generated = 1,

        Skipped = 2,

        Failed = 3
    }
}
