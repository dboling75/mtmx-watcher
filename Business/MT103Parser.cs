public static class MT103Parser
{
    public static MT103Message Parse(string rawMessage)
    {
        var fields = new Dictionary<string, string>();
        var lines = rawMessage.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        string currentTag = null;

        foreach (var line in lines)
        {
            if (line.StartsWith(":"))
            {
                int secondColon = line.IndexOf(':', 1);
                if (secondColon > 0)
                {
                    currentTag = line.Substring(1, secondColon - 1);
                    fields[currentTag] = line.Substring(secondColon + 1).Trim();
                }
            }
            else if (currentTag != null)
            {
                fields[currentTag] += "\n" + line.Trim();
            }
        }

        // Parse :32A:
        string field32A = fields["32A"];
        string date = field32A.Substring(0, 6);
        string currency = field32A.Substring(6, 3);
        string amount = field32A.Substring(9).Replace(",", ".");

        return new MT103Message
        {
            TransactionReferenceNumber = fields.GetValueOrDefault("20"),
            BankOperationCode = fields.GetValueOrDefault("23B"),
            ValueDate = DateTime.ParseExact(date, "yyMMdd", null),
            Currency = currency,
            Amount = decimal.Parse(amount),
            OrderingCustomer = fields.GetValueOrDefault("50K"),
            BeneficiaryCustomer = fields.GetValueOrDefault("59"),
            RemittanceInformation = fields.GetValueOrDefault("70"),
            DetailsOfCharges = fields.GetValueOrDefault("71A")
        };
    }
}