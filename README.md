# Search & Navigate Project

- In this project Onion Architecture, CQS/CQRS, MediatR patterns and tools are used. 
- Project structure could be viewed in the "ProjectStructure.png" file.
- Database scheme structure could be viewed in "DatabaseDesign.png" file.

NOTE: To be able to run and start to project, there are some things that should be adjusted.
  1. .NET8 must be installed.
  2. PostgreSQL should be installed.
  3. Create a database called "SearchNavigate"
  4. The provided docker container must be running so that MicroServices runs smoothly.
  5. And database connection strings must be specified in the following files. 
      1. Infrastructure/Persistence/Context/SearchNavigateContext.cs : line 25
      2. API/appsettings.json : line 9
      3. MicroServices/StreamReader/Program.cs : line 23
  6. Database migrations that is inside Persistance project must be updated by following command (in Vs Code Terminal) to create tables and relations.
     - ```dotnet-ef database update --context SearchNavigateDbContext  --project Infrastructure/Persistence --startup-project API```
