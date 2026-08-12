# Library App

## Description

Library App is a modular application designed to manage library operations such as book loans, patron management, and inventory tracking. It is built using .NET and follows a clean architecture approach to ensure scalability and maintainability.

## Project Structure

- `AccelerateDevGHCopilot.sln` - Solution file for the project.
- `src/`
  - `Library.ApplicationCore/`
    - `Entities/` - Contains core domain entities.
    - `Enums/` - Defines enumerations used across the application.
    - `Interfaces/` - Declares interfaces for core abstractions.
    - `Services/` - Implements business logic and domain services.
    - `Library.ApplicationCore.csproj` - Project file for the Application Core.
  - `Library.Console/`
    - `appSettings.json` - Configuration file for the console application.
    - `CommonActions.cs` - Contains reusable actions for the console app.
    - `ConsoleApp.cs` - Main application logic for the console interface.
    - `ConsoleState.cs` - Manages the state of the console application.
    - `Program.cs` - Entry point for the console application.
    - `Json/` - Contains JSON-related utilities or data.
    - `Library.Console.csproj` - Project file for the Console application.
  - `Library.Infrastructure/`
    - `Data/` - Contains data access implementations.
    - `Library.Infrastructure.csproj` - Project file for the Infrastructure layer.
- `tests/`
  - `UnitTests/`
    - `LoanFactory.cs` - Factory for creating test data related to loans.
    - `PatronFactory.cs` - Factory for creating test data related to patrons.
    - `ApplicationCore/` - Contains unit tests for the Application Core.
    - `UnitTests.csproj` - Project file for unit tests.

## Key Classes and Interfaces

- **Entities**
  - `Book` - Represents a book in the library.
  - `Patron` - Represents a library patron.
  - `Loan` - Represents a loan transaction.
- **Interfaces**
  - `IBookRepository` - Interface for book/copy query operations (title search, per-book copy retrieval).
  - `IPatronRepository` - Interface for patron-related data operations.
  - `ILoanRepository` - Interface for loan-related data operations, including bulk loan lookup by book copy.
  - `IBookService` - Interface for searching book availability by title.
  - `ILoanService` - Interface for managing loan operations.
- **Services**
  - `LoanService` - Implements loan-related business logic.
  - `BookService` - Calculates title-based book availability (available vs. total physical copies).
  - `NotificationService` - Handles notifications for overdue loans.

## Book Availability Search

The console app supports a read-only, title-based book availability search alongside the existing patron search:

- Press `b` from any patron-facing prompt to switch to book title search.
- Enter a (partial, case-insensitive) title to search; results are capped at 20 matches.
- Each result displays the title, author, and `available / total` physical copies. A copy is considered unavailable while it has an active loan (`ReturnDate` is `null`).
- Press `s` from book search results to switch back to patron search, or `b` to search books again.
- Checking out/reserving books, selecting copy details, and due-date display are out of scope for this feature.

## Usage

1. Clone the repository:

   ```bash
   git clone <repository-url>
   ```

2. Open the solution file `AccelerateDevGHCopilot.sln` in Visual Studio.

3. Build the solution to restore dependencies and compile the code.

4. Run the console application:

   ```bash
   dotnet run --project src/Library.Console/Library.Console.csproj
   ```

5. Execute unit tests:

   ```bash
   dotnet test tests/UnitTests/UnitTests.csproj
   ```

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
