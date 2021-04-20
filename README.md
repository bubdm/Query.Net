# Query.Net
A simple database query system based on object models amd asyncronous methods.

## Table of Contents
- [General Info](#general-info)
- [Setup](#setup)
- [Usage](#usage)
- [Examples](#examples)
- [Dependencies](#dependencies)

## General Info
Query.Net is a database querying library aimed at integrating queries into the C# workflow. Query.Net follows C# asynronous patterns of **Task** and **await**.

## Setup

### 1. Create an *IDbConnectionFactory* class
Create a class that implements the **IDbConnectionFactory** interface. Return your preferred DbConnection object

```csharp
public class MySampleConnectionFactory : IDbConnectionFactory
{
    public async Task<DbConnection> CreateDbConnection()
    {
        var connectionString = $"server={host};user={username};password={password};database={databaseName}";

        var connection = new MyDatabaseConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}
```

### 2. Assign the factory to *Query*
Assign the created factory class through Query's static method. Best to do this during app startup/init

```csharp
Query.SetConnectionFactory(new MySampleConnectionFactory());
```

### 3. Subclass *DbModel* to model table values

```csharp
public class Employee : DbModel
{
    // The name of the table that contains this object
    public override string tableName => "employees";

    public DbFieldValue<ulong> id => GetField<ulong>();

    public DbFieldValue<string> firstName => GetField<string>();

    public DbFieldValue<string> lastName => GetField<string>();

    public DbFieldValue<double> hourlyRate => GetField<double>();

    public DbFieldValue<DateTime> hireDate => GetField<DateTime>();
}
```

## Usage
Models are manipulated through the **Query** static class. All operations must run through here. Queries have two parts, the **method** and the **condition**.

### Method
The action to apply to the database.

#### Methods
- Select
- Update
- Insert
- Delete

### Condition
The condition applies a search parameter to the query. The condition comes after the method by calling the function **Where**. All methods, except for **Insert**, require a condition to be applied.

#### Conditions
```csharp
.Where(x => new[] { Condition.Equals(x.firstName, "Jack") }); // get employee named Jack
.Where(x => new[] { Condition.LessThan(x.hireDate, DateTime.Now.AddMonths(-1)) }); // get employees hired before last month
```

#### Key Conditions
A key condition returns a single field as a 'key' condition. The where will run with only an equals condition of the key's value
```csharp
.Where(x => x.id);
.Where(x => new[] { Condition.Equals(x.id, x.id.Get()) }); // the same query in expanded form
```

## Examples

### Select
```csharp
// selects all fields from the database (performs a SELECT *)
var allResults = await Query.Select<Employee>()
    .Where(x => new[] { Condition.Equals(x.id, 1234567) });
    
// selects only id, first name, and last name from the database
var specifiedResults = await Query.Select<Employee>(x => new DbFieldValue[] { x.id, x.firstName, x.lastName })
    .Where(x => new[] { Condition.Equals(x.id, 1234567) });
```

### Update
```csharp
// select employee from database
// a select query before update is NOT necessary, however, if the object wasn't selected, the update will run on all fields.
var results = await Query.Select<Employee>()
    .Where(x => new[] { Condition.Equals(x.id, 1234567) });

// get returned employee
var employee = results[0];

// update a value
employee.firstName.Set("John");

// returns the count of rows changed
var updatedRows = await Query.Update<Employee>(employee)
    .Where(x => x.id); // use a key getter where condition
```

### Insert
```csharp
var employee = new Employee();
employee.id.Set(1234567UL);
employee.firstName.Set("Jack");
employee.lastName.Set("Doe");

// Only values specified will be inserted
// hireDate and hourlyRate will be defaulted by the database

var insertSuccess = await Query.Insert(employee); // insert does not have a **where** condition
```

### Delete
```csharp
// no model call
var deletedRowCount = await Query.Delete<Employee>()
  .Where(x => new[] { Condition.Equals(x.id, 1234567) });
  
// call with a model
var employee = new Employee();
employee.id.Set(1234567UL);

var deletedRowCount = await Query.Delete(employee)
  .Where(x => x.id); // uses a key condition
```

## Dependencies
- .NET Standard 2.0
