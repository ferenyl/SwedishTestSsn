namespace SwedishTestSsn;

public class Result
{
    public string Next { get; set; } = string.Empty;
    public int ResultCount { get; set; }
    public int Offset { get; set; }
    public int Limit { get; set; }
    public int QueryTime { get; set; }
    public Ssn[] Results { get; set; } = [];
}

public class Ssn
{
    public string Testpersonnummer { get; set; } = string.Empty;
}

