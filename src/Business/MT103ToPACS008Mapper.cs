public static class MT103ToPacs008Mapper
{
    public static Pacs008Message Map(MT103Message mt103)
    {
        return new Pacs008Message
        {
            MessageId = $"MSG-{Guid.NewGuid()}",
            CreationDateTime = DateTime.UtcNow,
            InstructionId = mt103.TransactionReferenceNumber,
            EndToEndId = mt103.TransactionReferenceNumber,
            TransactionId = mt103.TransactionReferenceNumber,
            DebtorName = ExtractName(mt103.OrderingCustomer),
            DebtorAccount = ExtractAccount(mt103.OrderingCustomer),
            CreditorName = ExtractName(mt103.BeneficiaryCustomer),
            CreditorAccount = ExtractAccount(mt103.BeneficiaryCustomer),
            Currency = mt103.Currency,
            Amount = mt103.Amount,
            PurposeCode = "OTHR"
        };
    }

    private static string ExtractAccount(string field)
    {
        if (field.StartsWith("/"))
        {
            var lines = field.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return lines[0].Replace("/", "").Trim();
        }
        return "";
    }

    private static string ExtractName(string field)
    {
        var lines = field.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        return lines.Length > 1 ? lines[1].Trim() : "";
    }
}