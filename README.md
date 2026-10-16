03 September 2026
ERD screenshots uploaded
API endpoint uploaded
Some screenshots uploaded

04 September 2026
More screenshots uploaded
YouTube video created and uploaded
Script file uploaded

PART2- DUE 16/10/2026
 RaceDay.API source code.
 Models matching the Part 1 ERD.
 Database configuration and ApplicationDbContext.
 Authentication and role-based authorization.
 All required endpoints implemented.
 DTOs and input validation.
 Swagger documentation and evidence of endpoint testing.
 A GitHub Actions workflow that builds and tests the project.
 A README explaining how to configure, run and test the API.


 EXPLANATION OF CODE RUNNING
 1. Run the project: Visual Studio starts your RaceDay API when you press F5.
2. Program.cs: Configures the API, database connection, authentication, authorization and Swagger.
3. SQL Server: Stores the RaceDay data, including users, events, participants, entries, payments and results.
4. ApplicationDbContext: Connects the API to SQL Server using Entity Framework Core.
5. Models: Define the structure of the data stored in the database.
6. DTOs: Define the information users can send to or receive from the API.
7. Controllers: Handle requests for users, categories, events, enrolments and results.
8. Authentication: Checks whether a user has logged in successfully.
9. JWT tokens: Allow authenticated users to access protected endpoints.
10. Authorization: Checks whether a user has permission to perform an action based on their role, such as Organiser or Participant.
11. Swagger: Provides a webpage where you can view and test your API endpoints.
12. HTTP methods: GET retrieves data, POST creates data, PUT updates data and DELETE removes data.
13. Validation: Checks that submitted information is correct and required fields are provided.
14. Unit tests: Check whether the API functions correctly, including handling errors and unauthorized requests.
15. GitHub Actions: Automatically builds and tests your project when code is pushed to GitHub.
16. API responses: Return the requested information or an appropriate error message to the client.
17. Database connection: Your API must be able to reach SQL Server for database operations to work. Your Cloud Labs connection still needs to be resolved before this can be confirmed.
    NB: Everythung was implemented any error is due to technical issues

