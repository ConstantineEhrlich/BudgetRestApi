# EFBudgetModel

EFBudgetModel is a basic entity model for a budgeting system.
It's designed to manage financial transactions, categories, users, and budget files.
The model is built using C# and leverages Entity Framework for database interactions.

## Structure

### BudgetModel
- **Models**: Defines the main models of the system:
  - **User**: Represents a user, who can create and own budget files, categories and transactions.
  - **Category**: Represents a category for transactions.
  - **BudgetFile**: Represents a budget file, which holds transactions.
    The budget file also defines who is allowed to access it (owners).
  - **Transaction**: Represents a financial transaction, including details like type, date, owner, author, and amount.
- **Context**: Defines the database context, including the connection string and the configuration of the database tables.
  The relationship between the entities is also defined in the context class.
- **Enums**: Contains the `TransactionType` enum, which defines the types of transactions (Income, Expense, Budget).
- **Interfaces**: Contains the `IPeriodic` interface, which represents a periodic entity with year and period properties.
- **Extensions**: Includes extension methods for periodic transactions, such as filtering by period.

### UnitTests
- **BudgetModelTests**: Contains unit tests for creating, reading, and managing users, categories, budget files, and transactions.
- **Helpers**: Provides helper methods for unit tests, such as getting the database path and context.

## Usage

### Initialization
```csharp
// Initialize the database:
string databasePath = "/Path/to/the/database.db";
BudgetModel.Context context = new(databasePath);
```

### Create master data
```csharp
// Create the users:
User usr = new User("john", "John Wilson");
context.Users.Add(usr);
context.SaveChanges();

// Create the categories:
context.Categories.Add(Category(id: "salary", description: "Salary"));
context.Categories.Add(Category("rent", "Rent"));
context.Categories.Add(Category("groceries", "Groceries"));
context.SaveChanges();

// Create the budget file:
BudgetFile budget = new("Default budget");
budget.Owners.Add(usr);
context.BudgetFiles.Add(budget);
```

### Record transaction
```csharp
```
