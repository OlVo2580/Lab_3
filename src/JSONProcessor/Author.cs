using System.Text.Json.Serialization;

namespace JSONProcessor;

public class BookAuthor
{
    [JsonInclude]
    public string FirstName { get; set; }

    [JsonInclude]
    public string MiddleName { get; set; }

    [JsonInclude]
    public string LastName { get; set; }

    public BookAuthor()
    {
        FirstName = "";
        MiddleName = "";
        LastName = "";
    }

    public string GetFullName()
    {
        return FirstName + (MiddleName == "" ? "" : " ") + MiddleName + (LastName == "" ? "" : " ") + LastName;
    }
}