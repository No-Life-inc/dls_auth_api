# DLS Backend

## Contributors

- Morten Bendeke
- Betül Iskender
- Yelong Hartl-He
- Zack Ottesen

## General Use

This repository is the backend for DLS project spring 2024. The repository contains the logic handling user registration and user login. 
This repo is divided into seperate folders with separate responsibilities:
- Controller: Contains setting up the user class, a function to connect to database, and a function to hash passwords when a user logs in.
- Migrations: Contains different snapshot of the database. 
- Models: Contains a model of the user, register and login and setting up the user entity in the database.
- Utility: Contains code for JWT generation and RabbitMQ for consuming updates on the user. 

## Environment Variables

Create a .env in the root folder.

- JWT_SECRET='MmIxM2Q1NjNmNjA1YjNiYjZiNWY0M2VjOTVhMmFhZWVmMWQ3ODAwNDlkOTFkNjJlMGQ3YzA0ZDcwZDQ2ZGU0NA=='
- RABBITURL='localhost'
- RABBITUSER='user'
- RABBITPW='password'
- DB_SERVER='localhost,1434'
- DB_BACKEND='usersDb'
- DB_USER='SA'
- DB_PASSWORD='YourStrongPassword123'
- FRONTENDURL='http://localhost:8080'

## How To Run

Make sure the environment variables are set.<br

For Visual Studio, open a terminal and run following each line seperate: 

 - Add-Migration InitialCreate
 - Update-Database
 - Add-Migration MigrationName
 - Update-Database

Make sure to have dotnet installed.
For Jetbrains Rider, open a terminal and run following each line seperate: 

Make sure you have the Entity Framework installed
 - dotnet tool install --global dotnet-ef
 - dotnet ef --version
You can add EntityFrameworkCore.Design like this, but you have to make sure it is version 8.2. anything after this might distrupt the EF at this point in time. 
 - dotnet add package Microsoft.EntityFrameworkCore.Design
If you want to take the data from the database and create a migration file from it:
 - dotnet ef migrations add YourMigrationName --context DlsUserContext
To update the database with a migration file: 
 - dotnet ef database update --context DlsUserContext
If there are any error this code will help show what the mistake might be:
 - dotnet build

## Dependencies

 - Microsoft.EntityFrameworkCore.Design: Using EntityFramework to set up entities based on the database structure.  
 - Microsoft.EntityFrameworkCore.SqlServer: Using EntityFramework to connect to a SQL server 
 - Microsoft.AspNetCore.Mvc: A framework for building web APIs and web applications using the Model-View-Controller (MVC) pattern.
 - Microsoft.EntityFrameworkCore: EntityFramework Core, an object-relational mapping (ORM) framework that simplifies data access for .NET applications.
 - DotNetEnv : Used to set up a environmental file.
 - BCrypt.net-next: Used for crypting password.
 - System.IdentityModel.Tokens.Jwt: Used to generate tokens for users. 
 - Swashbuckle.AspNetCore: Setting up Swagger / API endpoints
 - RabbitMQ.Client: A .NET client library for RabbitMQ, a message broker that implements the Advanced Message Queuing Protocol (AMQP).

