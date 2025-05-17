# swetestssn - Swdish test social security numbers

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
