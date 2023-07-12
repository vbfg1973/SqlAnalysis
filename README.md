# SQL Analysis

Tool for analysing SQL statements. Currently limited to finding references to objects in SQL queries.

Uses a verb like structure from the command line and dumps output to CSV

## Building and running

Is a dotnet CLI app but it must be built before running. "dotnet run" will not cut it here because it's using a library that itself parses the CLI arguments.

From the \<solution_dir\>/SqlAnalyser directory:

    dotnet build

This will put the executable for your system in:

    bin/Debug/net7.0/SqlAnalyser.exe


Run:

    bin/Debug/net7.0/SqlAnalyser --help

This will list all the verbs available.

One verb is "SqlFileReferences"

Run:

    bin/Debug/net7.0/SqlAnalyser SqlFileReferences --help

This will list all the options with which this specific verb can be invoked, noting which are required.

### Output

The output CSV Will have the columns:

    Id,Name,ReferenceType

Name will be the name of the reference found.

ReferenceType will be the type of reference, currently limited to Table or StoredProcedure

Id will depend on the type of source analysed, but at present is either the row number from the table SQL Profiler dumps its output to or the file name containing the SQL

## Verbs

### Profiler References

Parses queries recorded by the SQL Profiler that have been dumped to a table. 

    ./bin/Debug/SqlAnalyser ProfilerReferences -t "<TableName>" -o "<path_to_output_csv>"

For this to run you will need a connection string in an environment variable called SQL_PROFILE_DB

In the output CSV, Id will be the RowNumber of the table that SQL Profiler dumped the recorded queries to. Where there are multiple rows with the same Id, then the references were discovered in the same query.

### SQL FIle references

Analyse a specific file:

    ./bin/Debug/SqlAnalyser SqlFileReferences -f "<path_to_file>" -o "<path_to_output_csv>"

Analyse all files in a directory structure:

    ./bin/Debug/SqlAnalyser SqlFileReferences -d "<path_to_dir>" -o "<path_to_output_csv>"

In the output CSV Id will be the path to the file in which the reference was found.