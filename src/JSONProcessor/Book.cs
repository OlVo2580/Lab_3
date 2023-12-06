using System.Text.Json.Serialization;

namespace JSONProcessor;

public class Book
{
    [JsonInclude]
    public string Title { get; set; }

    [JsonInclude]
    public string Annotation { get; set; }

    [JsonInclude] 
    public string Edition { get; set; }

    [JsonInclude]
    public BookAuthor Author { get; set; }

    public Book()
    {
        Title = "";
        Annotation = "";
        Edition = "";
        Author = new();
    }
}