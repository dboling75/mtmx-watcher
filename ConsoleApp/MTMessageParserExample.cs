string rawMt103 = @"
:20:REFERENCE123
:23B:CRED
:32A:240507USD12345,67
:50A:/123456789
JOHN DOE
:59:/987654321
JANE SMITH
:70:INVOICE 45678
:71A:SHA
";

var parsed = MTMessageParser.Parse(rawMt103);
foreach (var kvp in parsed)
{
    Console.WriteLine($"Tag: {kvp.Key}, Value: {kvp.Value}");
}
