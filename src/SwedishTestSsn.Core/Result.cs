namespace SwedishTestSsn.Core;

public record struct Result(int ResultCount, int Offset, int Limit, int QueryTime, string Next, Ssn[] Results);

public record struct Ssn(string Testpersonnummer);
