namespace TransactionsImport
{
	internal class AccountTransaction
	{
		public DateTime Date { get; set; }
		public decimal Debit { get; set; }
		public decimal Credit { get; set; }
		public decimal? CurrentAmount { get; set; }
		public string? Description { get; set; }

		public string? Account { get; set; }
		public string? Budget { get; set; }
		public string? Category { get; set; }
		public string? FriendlyDescription { get; set;}
	}
}
