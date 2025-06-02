# swetestssn - Swdish test social security numbers

[![GitHub Actions Workflow Status](https://img.shields.io/github/actions/workflow/status/ferenyl/SwedishTestSsn/dotnet.yaml?branch=main&style=for-the-badge)](https://github.com/ferenyl/SwedishTestSsn/actions/workflows/dotnet.yaml)
[![NuGet Version](https://img.shields.io/nuget/v/SwedishTestSsn?style=for-the-badge)](https://www.nuget.org/packages/SwedishTestSsn/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SwedishTestSsn?style=for-the-badge)](https://www.nuget.org/packages/SwedishTestSsn/)

Rust version: [swedish_test_ssn](https://github.com/ferenyl/swedish_test_ssn)

Get test social security numbers from Skatteverket.
You can use these as approved test data

You can use Regular expressions for pattern
this will get you 10 results of ssn that are in the year 1988 and on the 14th every month:

```bash
swetestssn "^1988[0-9][0-9]14" -l 10
```

```bash
--------------------------------------
---        Swedish Test ssn        ---
--------------------------------------

Get test social security numbers from Skatteverket. You can use these as approved test data


Usage
    swetestssn [Pattern] [options]

ARGUMENTS:
    [Pattern]    Test social security number. Regular expressions may be used

OPTIONS:
                    DEFAULT
    -h, --help                 Prints help information
    -l, --limit     100
    -o, --offset    0
        --json                 output as json

COMMANDS:
    swetestssn    Get swedish test social security numbers from The Swedish Tax Agency (Skatteverket). Using their publi API
```
