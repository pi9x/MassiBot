using System.Text.RegularExpressions;

namespace MassiBot.Core.Validation;

public static partial class AttachmentValidation
{
    public static bool TryGetValidFileName(this string fileName, out string? validFileName)
    {
        validFileName = null;
        if (Regex.IsMatch(fileName, RegexPatterns.TeamsFileNameExt))
        {
            validFileName = Regex.Match(fileName, RegexPatterns.ValidFileNamePart).Value + ".pdf";
            return true;
        };

        return false;
    }

    public static string GetPoCodeFromFileName(this string fileName)
    {
        if (!fileName.TryGetValidFileName(out _))
        {
            throw new InvalidDataException("File name does not follow the naming convention.");
        }

        // Take the 2nd part of the file name as PO Code
        return Regex.Matches(fileName, RegexPatterns.PoCodePart)[1].Value;
    }
}