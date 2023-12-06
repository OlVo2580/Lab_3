using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace JSONProcessor;

public delegate void MyCallback();

public partial class EditPage : ContentPage
{
	private IList<Book> Books;
	private readonly int ListIndex;
	public EditPage(IList<Book> books, int index)
	{
		InitializeComponent();
		Books = books;
		ListIndex = index;
		EntryForTitle.Text = Books[index].Title;
		EntryForEdition.Text = Books[index].Edition;
		EntryForAuthor.Text = Books[index].Author.GetFullName();
		EntryForAnnotation.Text = Books[index].Annotation;
	}

	protected override bool OnBackButtonPressed()
	{
		ApplyEditing();
		return base.OnBackButtonPressed();
	}


	// Button Actions
	private void On_Apply(object? _, EventArgs a)
	{
		OnBackButtonPressed();
	}


	// Other Methods
	public void ApplyEditing()
	{
		string[] AuthorContent = (EntryForAuthor.Text ?? "").Split(" ");
		StringBuilder builder = new();

		int ind = AuthorContent.Length > 2 ? 1 : 0;
		Books[ListIndex].Author.FirstName = AuthorContent[0];
		Books[ListIndex].Author.MiddleName = ind > 0 ? AuthorContent[ind] : "";

		while (++ind < AuthorContent.Length)
		{
			builder.Append(AuthorContent[ind]);
			if (ind + 1 != AuthorContent.Length)
			{
				builder.Append(" ");
			}
		}

		Books[ListIndex].Author.LastName = builder.ToString();
		Books[ListIndex].Title = EntryForTitle.Text ?? "";
		Books[ListIndex].Edition = EntryForEdition.Text ?? "";
		Books[ListIndex].Annotation = EntryForAnnotation.Text ?? "";
	}
}
