using System;
using System.Xml.Serialization;

[XmlRoot("Document", Namespace = "urn:iso:std:iso:20022:tech:xsd:pacs.008.001.08")]
public class Pacs008Message
{
    public string MessageId { get; set; }

    public DateTime CreationDateTime { get; set; }

    public string InstructionId { get; set; }

    public string EndToEndId { get; set; }

    public string TransactionId { get; set; }

    public string DebtorName { get; set; }

    public string DebtorAccount { get; set; }

    public string CreditorName { get; set; }

    public string CreditorAccount { get; set; }

    public string Currency { get; set; }

    public decimal Amount { get; set; }

    public string PurposeCode { get; set; }
}
