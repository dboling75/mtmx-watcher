var mt103 = new MT103Message
{
    TransactionReferenceNumber = "REF123456",
    BankOperationCode = "CRED",
    ValueDate = DateTime.ParseExact("240507", "yyMMdd", null), // from :32A:
    Currency = "USD",
    Amount = 12345.67m,
    OrderingCustomer = "/123456789\nJOHN DOE",
    BeneficiaryCustomer = "/987654321\nJANE SMITH",
    RemittanceInformation = "INVOICE 45678",
    DetailsOfCharges = "SHA"
};

Pacs008Message pacs = MT103ToPacs008Mapper.Map(mt103);

Console.WriteLine($"Debtor: {pacs.DebtorName} ({pacs.DebtorAccount}) ➜ Creditor: {pacs.CreditorName} ({pacs.CreditorAccount})");
Console.WriteLine($"Amount: {pacs.Amount} {pacs.Currency}");
