namespace MassiBot.Core.Validation;

public class RegexPatterns
{
    public const string TeamsFileNameExt = @"^([a-zA-Z]+\d{0,}){1}_([a-zA-Z\d-]+){1}_\d{4}_\d{2}_\d+[\s\d]{0,}.[pP]{1}[dD]{1}[fF]{1}$";
    public const string ValidFileNamePart = @"^([a-zA-Z]+\d{0,}){1}_([a-zA-Z\d-]+){1}_\d{4}_\d{2}_\d+";
    public const string PoCodePart = @"[a-zA-Z\d-]+";
    public const string EmployeeAccountLookup = @"^([a-zA-Z]+\d{0,}){1}$";
    public const string PoCodeLookup = @"^([a-zA-Z\d-]+){1}$";
}