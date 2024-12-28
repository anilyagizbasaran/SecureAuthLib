# SecureAuthLib

SecureAuthLib is a robust library designed for user management and email sending functionalities. It includes features such as user registration, login, password reset, and account activation, all implemented with security in mind.

---

## Features

- **User Registration**: Enables new users to register.
- **Login**: Allows users to authenticate and access the system.
- **Password Reset**: Provides users with a secure way to reset passwords.
- **Account Activation**: Sends email verification for account activation.
- **Secure Token Management**: Generates random, secure tokens to protect user operations.
- **Email Sending**: Sends activation and password reset emails via SMTP.

---

## Installation

1. **Clone the Repository:**
   ```bash
   git clone <repository-url>
   ```

2. **Install Dependencies:**
   Ensure all necessary dependencies are installed by restoring the project:
   ```bash
   dotnet restore
   ```

3. **Build the Project:**
   ```bash
   dotnet build
   ```

4. **Run the Project:**
   ```bash
   dotnet run
   ```

---

## Database Setup

To use this library, configure a SQL Server database and create the `Users` table with the following schema:

```sql
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(256) NOT NULL,
    PasswordHash NVARCHAR(256) NOT NULL,
    Salt NVARCHAR(256) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL,
    ActivationToken NVARCHAR(256),
    ResetToken NVARCHAR(256),
    ResetTokenExpiry DATETIME
);
```

---

## Configuration

Add your database connection string in the `app.config` file:

```xml
<configuration>
  <connectionStrings>
    <add name="DefaultConnection"
         connectionString="Server=your_server;Database=your_database;User Id=your_user;Password=your_password;"
         providerName="System.Data.SqlClient" />
  </connectionStrings>
</configuration>
```

---

## Usage

### User Management

#### User Registration
```csharp
var userManager = new UserManager(userDal, emailService);
userManager.Register("newuser@example.com", "securePassword123");
```

#### User Login
```csharp
var user = userManager.Login("newuser@example.com", "securePassword123");
Console.WriteLine($"Welcome, {user.Email}!");
```

#### Password Reset Request
```csharp
userManager.RequestPasswordReset("newuser@example.com");
```

#### Account Activation
```csharp
userManager.ActivateUser("activation-token-from-email");
```

### Email Sending

#### Email Service Setup
```csharp
var smtpClient = new SmtpClientWrapper("smtp.gmail.com", 587, "your-email@gmail.com", "your-email-password");
var emailService = new EmailService(smtpClient);
```

---

## Testing

Unit tests are written using `xUnit` and `Moq`. Run the tests with the following command:

```bash
dotnet test
```

---

## Contributing

Contributions are welcome! If you would like to contribute, please:

1. Fork the repository.
2. Create a feature branch.
3. Submit a pull request.

---

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.

