# Budget REST API

Budget REST API is a budget management system.
It's designed to manage financial Transactions, which are recorded in the Budget Files under a chosen Category. The system also provides user authentication using JWT tokens.
The model is built using C# and leverages ASP.Net for web-server and Entity Framework for database interactions.

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

### BudgetServices
Provides data access services:
- UserService - registration, password verification of the users
- BudgetFileService - get, add, update and delete (deactivate) the Budget File
- CategoryService
- TransactionService

### BudgetWebApi
Provides REST API with the following endpoints:

| Endpoint route                                    | Method | DTO               | Description                            |
|:--------------------------------------------------|:------:|-------------------|:---------------------------------------|
| /user/signup                                      |  POST  | SignUp            | Creates a new user                     |
| /user/login                                       |  POST  | SignIn            | Authenticates the user                 |
| /user/<user-id>                                   |  GET   |                   | Gets information about user            |
| /user/<user-id>                                   |  PUT   |                   | Updates the information about the user |
| /budget                                           |  GET   |                   | Gets the list of budgets               |
| /budget/add                                       |  POST  | BudgetFileAdd     | Adds a new budget                      |
| /budget/<budget-id>                               |  GET   |                   | Gets specific budget                   |
| /budget/<budget-id>                               |  PUT   | BudgetFileUpdate  | Updates the details of the budget      |
| /budget/<budget-id>                               | DELETE |                   | Sets the budget to status "deleted"    |
| /budget/<budget-id>/categories                    |  GET   |                   | Gets all categories for the budget     |
| /budget/<budget-id>/categories/add                |  POST  | CategoryAdd       | Adds a category to the budget          |
| /budget/<budget-id>/categories/<category-id>      |  GET   |                   | Gets specific category                 |
| /budget/<budget-id>/categories/<category-id>      |  PUT   | CategoryUpdate    | Updates the details of the category    |
| /budget/<budget-id>/categories/<category-id>      | DELETE |                   | Sets the category to status "inactive" |
| /budget/<budget-id>/owners                        |  GET   |                   | Gets the owners of the budget          |
| /budget/<budget-id>/owners/add                    |  POST  | BudgetOwnerAdd    | Adds a new owner to the budget         |
| /budget/<budget-id>/owners/<user-id>              | DELETE |                   | Removes user from the owners           |
| /budget/<budget-id>/transactions                  |  GET   |                   | Gets all transactions for the budget   |
| /budget/<budget-id>/transactions/add              |  POST  | TransactionAdd    | Adds new transaction                   |
| /budget/<budget-id>/transactions/<transaction-id> |  GET   |                   | Gets specific transaction              |
| /budget/<budget-id>/transactions/<transaction-id> |  PUT   | TransactionUpdate | Updates transaction details            |


## Usage Examples
This section shows direct programmatic manipulation with the database through the Entity Framework

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
// Detailed constructor:
Transaction t1 = new(budget, owner, author, date, type, category, amount);
context.Transactions.Add(t1);
context.SaveChanges();

//Simplified constructor, assumes that the author of the transaction is its owner, and the date is current date:
Transaction t2 = new(budget, author, type, cat, amount);
context.Transactions.Add(t2);
context.SaveChanges();
```

### Read data
```csharp
// Sum transactions for a specific category:
Category cat = context.Categories.FirstOrDefault(c => c.Id == "groceries");
var groceryExpenses = context.Transactions.Where(t => t.Category == cat);
decimal spent = groceryExpenses.Sum(e => e.Amount);

// Utilize the extensions to calculate values between periods:
decimal spentOnSummer = groceryExpenses.BetweenPeriods(2023, 6, 2023, 8).Sum(t => t.Amount);
```


## License
This project is distributed under the MIT license.