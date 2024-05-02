using System.Text.RegularExpressions;

namespace TransactionsImport
{
	internal class TransactionsClassifier
	{
		private Lazy<List<CategoryDef>> _categories;

		public TransactionsClassifier(string categoriesConfigFile)
		{
			_categories = new Lazy<List<CategoryDef>>(File.ReadLines(categoriesConfigFile).Select(CategoryDef.Parse).ToList());
		}

		public int AssignCategories(IEnumerable<AccountTransaction> transactions)
		{
			int classified = 0;
			foreach (var tran in transactions)
			{
				if (AssignCategory(tran))
					classified++;
			}
			return classified;
		}

		private bool AssignCategory(AccountTransaction tran)
		{
			foreach (var classDef in _categories.Value)
			{
				if (string.IsNullOrEmpty(tran.Description)
					|| !classDef.Regex.Match(tran.Description).Success)
					continue;

				tran.FriendlyDescription = classDef.FriendlyDescription;
				tran.Budget = classDef.Budget;
				tran.Category = classDef.Category;

				return true;
			}

			return false;
		}

		private class CategoryDef
		{
			public Regex Regex { get; set; }

			public string? FriendlyDescription { get; set; }
			public string? Category { get; set; }
			public string? Budget { get; set; }

			public CategoryDef(Regex regex)
			{
				Regex = regex;
			}

			public static CategoryDef Parse(string line)
			{
				var columns = line.Split('\t');
				if (columns.Length != 4)
				{
					var ex = new ArgumentException(
						"The categories configuration line has invalid format. It is expected to contain 4 tab-delimited columns.",
						nameof(line));
					ex.Data.Add("Expected format", "Regex <tab> Friendly description <tab> Budget <tab> Category");
					ex.Data.Add(nameof(line), line);
					throw ex;
				}

				
				var def = new CategoryDef(new Regex(columns[0].TrimEnd('i').Trim('/'), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));
				def.FriendlyDescription = columns[1].Trim();
				def.Budget = columns[2].Trim();
				def.Category = columns[3].Trim();
				return def;
			}
		}
	}
}
