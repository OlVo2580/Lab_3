using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Maui.Graphics;

namespace JSONProcessor;

public partial class MainPage : ContentPage
{
	private FileResult? SelectedFile;

	private IFilePicker FileChooser;

	private IList<Book>? Books;
	public MainPage()
	{
		InitializeComponent();
		FileChooser = FilePicker.Default;
	}


	// Button Actions
	private async void On_Save(object? sender, EventArgs args)
	{
		if (SelectedFile is null)
		{
			await DisplayAlert("Error", "In order to perform this action, you have to open a file", "Ok");
			return;
		}

		JsonSerializerOptions options = new()
		{
			WriteIndented = true
		};
		File.WriteAllText(SelectedFile.FullPath, JsonSerializer.Serialize(Books, options));

		await DisplayAlert("Success", $"The file {SelectedFile.FileName} was saved successfully", "Ok");
	}

	private async void On_Exit(object? sender, EventArgs e)
	{
		var option = await DisplayAlert("Exit Confirmation", "Exit the program ?", "Yes", "No");
		if (option)
		{
			System.Environment.Exit(0);
		}
	}

	private async void On_Open(object? sender, EventArgs e)
	{
		if (!await AskForAbsentPermissions())
		{
			return;
		}

		var allowedFileTypes = new FilePickerFileType(
				new Dictionary<DevicePlatform, IEnumerable<string>>
				{
					{ DevicePlatform.Android, new[] { "application/json" } },
					{ DevicePlatform.WinUI, new[] {".json"} }
				});
		var pickOptions = new PickOptions() { PickerTitle = "Select json file with books", FileTypes = allowedFileTypes };
		SelectedFile = await FileChooser.PickAsync(pickOptions);

		if (SelectedFile == null)
		{
			return;
		}

		using (var stream = await SelectedFile.OpenReadAsync())
		{
			Books = JsonSerializer.Deserialize<List<Book>>(stream);
		}

		if (Books is null)
		{
			await DisplayAlert("Error", "The file was not read successfully", "Ok");
			SelectedFile = null;
			return;
		}

		ClearGridForSearchResults();
		MakeSearch();
	}

	private async void On_Find(object? sender, EventArgs e)
	{
		if (SelectedFile is null)
		{
			await DisplayAlert("Error", "In order to perform this action, you have to open a file", "Ok");
			return;
		}

		MakeSearch();
	}

	private void On_Clear(object? sender, EventArgs e)
	{
		ClearGridForSearchResults();
	}

	private void On_DeleteFilters(object? sender, EventArgs args)
	{
		ClearFilters();
	}

	private async void On_Info(object? sender, EventArgs args)
	{
		await DisplayAlert("Information", "Author: Voloshyna Oleksandra Anatoliivna, 2 course, K-27, 2023\n\nWith this program you could read, edit and search in the json files", "Ok");
	}

	private async void On_Add(object? sender, EventArgs args)
	{
		if (Books is null)
		{
			await DisplayAlert("Error", "In order to perform this action, you have to open a file", "Ok");
			return;
		}

		Books.Add(new Book());
		await Navigation.PushModalAsync(new EditPage(Books, Books.Count - 1));
	}

	private void On_FilterChanged(object? sender, EventArgs args)
	{
		if (SelectedFile is null)
		{
			return;
		}

		MakeSearch();
	}

	// Other Methods
	private async Task<bool> AskForAbsentPermissions()
	{
		if (await Permissions.CheckStatusAsync<Permissions.StorageRead>() != PermissionStatus.Granted
				&& await Permissions.RequestAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
		{
			return false;
		}

		if (await Permissions.CheckStatusAsync<Permissions.StorageRead>() != PermissionStatus.Granted
		&& await Permissions.RequestAsync<Permissions.StorageRead>() != PermissionStatus.Granted)
		{
			return false;
		}

		return true;
	}

	private Filters CollectFiltersFromGUI()
	{
		var searchFilters = new Filters();
		if (CheckboxForTitle.IsChecked)
		{
			searchFilters.Title = EntryForTitle.Text ?? "";
		}

		if (CheckboxForAnnotation.IsChecked)
		{
			searchFilters.Annotation = EntryForAnnotation.Text ?? "";
		}

		if (CheckboxForAuthor.IsChecked)
		{
			searchFilters.Author = EntryForAuthor.Text ?? "";
		}

		if (CheckboxForEdition.IsChecked)
		{
			searchFilters.Edition = EntryForEdition.Text ?? "";
		}

		return searchFilters;
	}

	private void ClearFilters()
	{
		EntryForTitle.Text = "";
		CheckboxForTitle.IsChecked = false;
		EntryForAuthor.Text = "";
		CheckboxForAuthor.IsChecked = false;
		EntryForEdition.Text = "";
		CheckboxForEdition.IsChecked = false;
		EntryForAnnotation.Text = "";
		CheckboxForAnnotation.IsChecked = false;
	}

	private void ClearGridForSearchResults()
	{
		while (GridForSearchResults.Children.Count > 4)
		{
			GridForSearchResults.Children.RemoveAt(4);
		}
		while (GridForSearchResults.RowDefinitions.Count > 1)
		{
			GridForSearchResults.RowDefinitions.RemoveAt(1);
		}
	}

	private void CreateLabel(int row, int column, string text)
	{
		var label = new Label
		{
			Text = text,
			VerticalOptions = LayoutOptions.Center,
			HorizontalOptions = LayoutOptions.Center,
		};
		GridForSearchResults.SetRow(label, row);
		GridForSearchResults.SetColumn(label, column);
		GridForSearchResults.Children.Add(label);
	}

	private Button CreateDefaultButton()
	{
		return new Button
		{
			VerticalOptions = LayoutOptions.Center,
			HorizontalOptions = LayoutOptions.Center,
			BackgroundColor = Colors.Black,
			TextColor = Colors.White,
		};
	}

	private void CreateButtonForViewAnnotation(int row, Book b)
	{
		var button = CreateDefaultButton();
		button.Text = "View";
		button.Clicked += async (object? sender, EventArgs args) => await DisplayAlert($"{b.Title} - Annotation", b.Annotation, "Ok");

		GridForSearchResults.SetRow(button, row);
		GridForSearchResults.SetColumn(button, 3);
		GridForSearchResults.Children.Add(button);
	}

	private void CreateButtonForEditing(int row, Book b)
	{
		if (Books is null)
		{
			return;
		}

		var button = CreateDefaultButton();
		button.Text = "Edit";

		button.Clicked += async (object? sender, EventArgs args) =>
		{
			var page = new EditPage(Books, Books.IndexOf(b));
			await Navigation.PushModalAsync(page);
		};

		GridForSearchResults.SetRow(button, row);
		GridForSearchResults.SetColumn(button, 4);
		GridForSearchResults.Children.Add(button);
	}

	private void CreateButtonForDeleting(int row, Book b)
	{
		if (Books is null)
		{
			return;
		}

		var button = CreateDefaultButton();
		button.Text = "Delete";

		button.Clicked += (object? sender, EventArgs args) =>
		{
			Books.Remove(b);
			MakeSearch();
		};

		GridForSearchResults.SetRow(button, row);
		GridForSearchResults.SetColumn(button, 5);
		GridForSearchResults.Children.Add(button);
	}

	private void MakeSearch()
	{
		var filterOptions = CollectFiltersFromGUI();
		var results = (from book in Books where filterOptions.ValidateBook(book) select book).ToList();

		ClearGridForSearchResults();
		ShowSearchResults(results);
	}

	private void ShowSearchEntry(int row, Book book)
	{
		GridForSearchResults.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
		CreateLabel(row, 0, book.Title);
		CreateLabel(row, 1, book.Author.GetFullName());
		CreateLabel(row, 2, book.Edition);
		CreateButtonForViewAnnotation(row, book);
		CreateButtonForEditing(row, book);
		CreateButtonForDeleting(row, book);
	}

	private void ShowSearchResults(IList<Book> books)
	{
		for (int i = 1; i <= books.Count; ++i)
		{
			ShowSearchEntry(i, books[i - 1]);
		}
	}
}
