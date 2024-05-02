using TransactionsImport;
using TransactionsImport.Properties;

string? file;
if (args.Length == 0)
{
	Console.WriteLine("Please provide a path to CSV file with transactions:");
	file = Console.ReadLine().Trim('"');
}
else
{
	file = args[0];
}

var transactions = new TransactionsReader(file, Settings.InputDecimalSeparator).Read().ToList();
Console.WriteLine("{0} transactions read from the file.", transactions.Count);

var classifier = new TransactionsClassifier(Settings.CategoriesConfigFile);

int classified = classifier.AssignCategories(transactions);
Console.WriteLine("The categories was assigned to {0} transactions. According to the {1} file with categroies.",
	classified, Settings.CategoriesConfigFile);

var groupedTransactions = new TransactionsGrouper().Group(transactions).OrderBy(t => t.Date).ToList();

groupedTransactions.ForEach(t =>
	t.FriendlyDescription += !string.IsNullOrEmpty(t.Budget) ? $"${t.Budget}" : $"@{t.Description}");

var writer = new TransactionsWriter(Path.Combine(Path.GetDirectoryName(file), Settings.OutputTransactionsFile));
writer.Write(groupedTransactions);
Console.WriteLine("{0} transactions were written to {1} file after grouping.",
	groupedTransactions.Count, Settings.OutputTransactionsFile);

return 0;