# BookInfoImporter

1. Prerequisite

    Visual studio 2022
    
    MS SQL SERVER
    
    .NET CORE 8.0

2. How to run

    2.1 Run the script/create_table_books.sql to create the table in database
    
    2.2 Set the ConnectionString in appsettings.json

    2.3 Run the program with command

    > BookInfoImporter [Name_OF_CSV_FILE]

    eg. 
    
    > BookInfoImporter books.csv

3. Settings

    Setting | Meaning | Default
    --------|---------|--------
    ConnectionString | Connection string to the database in MS SQL SERVER format | 
    RecordsImportReportName (Optional) | Name of the report of the book records import | RecordsImportReport.txt
    Top100MostRecentlyPublishedBooksReportName | Name of the report of the top 100 most recently published books | Top100MostRecentlyPublishedBooksReport.csv

    eg.

    ```json
    {
        "ConnectionString": "Server=localhost;Database=master;User Id=sa;Password=98uI832h8s;TrustServerCertificate=True",
        "RecordsImportReportName": "RecordsImportReport.txt",
        "Top100MostRecentlyPublishedBooksReportName": "Top100MostRecentlyPublishedBooksReport.csv"
    }

    ```





