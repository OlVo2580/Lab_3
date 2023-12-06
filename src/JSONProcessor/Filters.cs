namespace JSONProcessor;

public struct Filters
{
    public string Title { get; set; }

    public string Annotation { get; set; }

    public string Author { get; set; }

    public string Edition { get; set; }

    public Filters()
    {
        Title = "";
        Annotation = "";
        Author = "";
        Edition = "";
    }

    public readonly bool ValidateBook(Book book)
    {
        var title = book.Title.ToLower().Contains(Title.ToLower());
        var edition = book.Edition.ToLower().Contains(Edition.ToLower());
        var annotation = book.Annotation.ToLower().Contains(Annotation.ToLower());
        var author = book.Author.GetFullName().ToLower().Contains(Author.ToLower());

        return title && edition && annotation && author;
    }
}
