# Budget REST API

Budget REST API is a budget management system. Using the application, the **Users** can record and maintain financial **Transactions**, which are stored in the **Budget Files** under a chosen **Category**.

### Key features
- The application is written on C# and leverages ASP.Net for web-server and Entity Framework for database interactions.
- The application and database are configured for containerized deployment on Kubernetes cluster.
- The API provides an authentication mechanism based on a JWT token, which may be transferred directly or in the cookie (for safe authentication in a web context).   

## Usage

### Local run
The application requires database to run. You may change the variables in the `BudgetWebApi/Properties/launchSettings.json` to point the app to the database. Alternatively, you can edit `BudgetWebApi/Startup.cs` to use SQLite. 

- In the `BudgetModel` directory, execute `dotnet ef database update` to set up the database.
- In the `BudgetWebApi` directory, execute `dotnet run` to run the application.

### Service deployment
Use `make` to build, deploy and expose the application on the kubernetes cluster.
- `make kube-secrets` - creates kubernetes secrets (required by other deployments)
- `make minikube-images` - points the docker to the minikube environment and builds the images for local deployment
- `make remote-images` - builds the images for amd64 platform and pushes into the docker registry
- `make restart` - restarts the service (reloads the image)
- `make listen-postgres` - exposes the postgres database to the local port 5432
- `make listen-api` - exposes the application on the port 8765 


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

| Endpoint route                                    | Method | DTO               | Description                               |
|:--------------------------------------------------|:------:|-------------------|:------------------------------------------|
| /user/signup                                      |  POST  | SignUp            | Creates a new user                        |
| /user/login                                       |  POST  | SignIn            | Authenticates the user                    |
| /user/logout                                      |  POST  | {Empty}           | Sends a cookie with an empty access_token |
| /user/{user-id}                                   |  GET   |                   | Gets information about user               |
| /budget                                           |  GET   |                   | Gets the list of budgets                  |
| /budget/add                                       |  POST  | BudgetFileAdd     | Adds a new budget                         |
| /budget/{budget-id}                               |  GET   |                   | Gets specific budget                      |
| /budget/{budget-id}                               |  PUT   | BudgetFileUpdate  | Updates the details of the budget         |
| /budget/{budget-id}                               | DELETE |                   | Sets the budget to status "deleted"       |
| /budget/{budget-id}/categories                    |  GET   |                   | Gets all categories for the budget        |
| /budget/{budget-id}/categories/add                |  POST  | CategoryAdd       | Adds a category to the budget             |
| /budget/{budget-id}/categories/{category-id}      |  GET   |                   | Gets specific category                    |
| /budget/{budget-id}/categories/{category-id}      |  PUT   | CategoryUpdate    | Updates the details of the category       |
| /budget/{budget-id}/categories/{category-id}      | DELETE |                   | Sets the category to status "inactive"    |
| /budget/{budget-id}/owners                        |  GET   |                   | Gets the owners of the budget             |
| /budget/{budget-id}/owners/add                    |  POST  | BudgetOwnerAdd    | Adds a new owner to the budget            |
| /budget/{budget-id}/owners/{user-id}              | DELETE |                   | Removes user from the owners              |
| /budget/{budget-id}/transactions                  |  GET   |                   | Gets all transactions for the budget      |
| /budget/{budget-id}/transactions/add              |  POST  | TransactionAdd    | Adds new transaction                      |
| /budget/{budget-id}/transactions/{transaction-id} |  GET   |                   | Gets specific transaction                 |
| /budget/{budget-id}/transactions/{transaction-id} |  PUT   | TransactionUpdate | Updates transaction details               |


## License
This project is distributed under the MIT license.