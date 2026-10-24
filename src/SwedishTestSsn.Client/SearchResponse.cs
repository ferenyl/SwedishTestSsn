namespace SwedishTestSsn.Client;

internal record struct SearchResponse(int ResultCount, int Offset, int Limit, int QueryTime, string Next, Ssn[] Results);

internal record struct Ssn(string Testpersonnummer);
