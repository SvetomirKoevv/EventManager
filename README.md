# EventManager

<details>
  <summary>Why ASP.NET</summary>
  In this section, I will explain why I chose to create this app using C# and ASP.NET.  
  One of the main reasons is that I have used C# in multiple projects and I’m familiar with building full-stack applications with it.  
  Additionally, C# has powerful and easy-to-use frameworks like **Entity Framework**, which simplifies the process of creating and managing the application's database.
</details>

<details>
  <summary>How to run the app locally</summary>
  
  ### 1. Requirements
  To run this project, you will need **SQL Server** and **Visual Studio** installed.

  ### 2. Clone the repository
  When opening Visual Studio, click on the **"Clone a repository"** button on the right.  
  Paste the following URL into the repository link field:  
  `https://github.com/SvetomirKoevv/EventManager.git`

  ### 3. Change the connection string
  Open the **DataLayer** project and locate the `EventManagerDbContext` class.  
  Inside it, you'll find a static variable called `ConnectionString`.  
  Change the first part like so:  
  ```
  Server=YourSqlDbName
  ```

  ### 4. Apply migrations
  **Make sure the `DataLayer` project is set as the startup project.**  
  You can do this by right-clicking on **DataLayer** and selecting **"Set as StartUp Project"**.

  Also, in the **Package Manager Console**, set the **Default Project** to **DataLayer**.  
  If you don’t see the console, open it via:  
  `View -> Other Windows -> Package Manager Console`

  #### If a **Migrations** folder already exists:
  Run the following command in the Package Manager Console:  
  ```
  Update-Database
  ```
  If everything is configured correctly, no errors should appear.  
  If you encounter errors, they’re likely related to the connection string.

  #### If a **Migrations** folder does **not** exist:
  In the Package Manager Console, run:  
  ```
  Add-Migration YourMigrationName
  ```
  If no errors occur, then apply the migration by running:  
  ```
  Update-Database
  ```

  ### 5. Seed data
  Set the **SeedingLayer** project as the startup project and run it.  
  This will populate the database with default roles and an admin account.  
  The console will display whether everything was created successfully.

  ### 6. Start the app
  Finally, set the **MVCEventManager** project as the startup project and run the application.

</details>
