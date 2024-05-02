using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using TransactionsImport;

internal class TransactionsWriter
{
	private string _outFile;

	public TransactionsWriter(string outputTransactionsFile)
	{
		_outFile = outputTransactionsFile;
	}

	internal void Write(IEnumerable<AccountTransaction> grouppedTransactions)
	{
		using (var writer = new CsvWriter(
			new StreamWriter(_outFile, new FileStreamOptions { Mode = FileMode.Create, Access = FileAccess.ReadWrite }),
			new CsvConfiguration(CultureInfo.InvariantCulture) { ShouldQuote = _ => true }))
		{
			writer.WriteHeader<AccountTransaction>();
			writer.NextRecord();

			foreach (var tran in grouppedTransactions)
			{
				WriteTransaction(tran, writer);
				writer.NextRecord();
			}
		}
	}

	private void WriteTransaction(AccountTransaction tran, CsvWriter writer)
	{
		writer.WriteField(tran.Date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
		writer.WriteField(-Math.Round(tran.Debit));
		writer.WriteField(Math.Round(tran.Credit));
		writer.WriteField(tran.CurrentAmount);
		writer.WriteField(tran.Description);

		writer.WriteField(tran.Account);
		writer.WriteField(tran.Budget);
		writer.WriteField(tran.Category);
		writer.WriteField(tran.FriendlyDescription);
	}
}