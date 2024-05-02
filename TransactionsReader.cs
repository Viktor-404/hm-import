using System.Globalization;

namespace TransactionsImport
{
	internal class TransactionsReader
	{
        private readonly string _inputDecimalSeparator;

        public string FilePath { get; }

		public TransactionsReader(string csvFilePath, string inputDecimalSeparator)
		{
            if (inputDecimalSeparator.Length > 1)
                throw new ArgumentException($"{nameof(inputDecimalSeparator)} '{inputDecimalSeparator}' should be a single character.");

            FilePath = csvFilePath;
            _inputDecimalSeparator = inputDecimalSeparator;
        }

		public IEnumerable<AccountTransaction> Read()
		{
			return File.ReadLines(FilePath).Select(ParseLine);
		}

		private AccountTransaction ParseLine(string csvLine)
		{
			var columns = csvLine.Trim('\"').Split("\",\"");
			if (columns.Length != 6)
			{
				var ex = new ArgumentException("The CSV line invalid format. It is expected to contain 6 columns.", nameof(csvLine));
				ex.Data.Add("Expected format", "\"Value date\",\"Processing date\",\"Debit\",\"Credit\",\"Amount\",\"Description\"");
				ex.Data.Add(nameof(csvLine), csvLine);
				throw ex;
			}

			CultureInfo culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberGroupSeparator = "";
            culture.NumberFormat.NumberDecimalSeparator = _inputDecimalSeparator;

            var tran = new AccountTransaction();
			tran.Date = DateTime.ParseExact(columns[1], "dd.MM.yyyy", culture);
			tran.Debit = !string.IsNullOrEmpty(columns[2]) ? ToDecimal(columns[2], culture) : 0m;
			tran.Credit = !string.IsNullOrEmpty(columns[3]) ? ToDecimal(columns[3], culture) : 0m;
			tran.CurrentAmount = !string.IsNullOrEmpty(columns[4]) ? ToDecimal(columns[4], culture) : null;
			tran.Description = columns[5].Trim();
			return tran;
		}

        private decimal ToDecimal(string value, CultureInfo culture)
		{
			string cleanValue = new string(value.Where(c => Char.IsNumber(c) || c == _inputDecimalSeparator[0]).ToArray());
			decimal result;
			if (Decimal.TryParse(cleanValue, NumberStyles.Any, culture, out result))
			{
				return result;
			}
			else
			{
				throw new FormatException(
					$"Can not convert {value} string to the decimal value with a {_inputDecimalSeparator} separator.");
			}
		}
    }
}
