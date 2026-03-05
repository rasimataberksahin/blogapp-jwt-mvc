## ASP.NET Core MVC JWT Blog App
This project is a simple blog application built with **ASP.NET Core MVC** and **Entity Framework Core**.

The goal of this project is to understand how **JWT authentication works in an MVC application**.

---

## Features

- JWT based authentication
- Login system
- Blog post creation
- Blog listing
- Personal blog list
- Blog detail page

---

## Technologies

- ASP.NET Core MVC
- Entity Framework Core
- SQL Server
- JWT Authentication
- Bootstrap

---

## Default Users

The application seeds default users automatically.

You can login with:

Email: user1@gmail.com  
Password: 1234

---

## Authentication Flow

1. User submits login form
2. Credentials checked in database
3. JWT token is generated
4. Token stored in cookie (`access_token`)
5. Middleware validates token on each request
6. User identity extracted from JWT claims

---

## Project Structure

```
BlogApp.Data
    Entities
    DbContext
    DataExtensions

BlogApp.Mvc
    Controllers
    Models (ViewModels)
    Views
```

---

## Learning Goals

This project demonstrates:

- JWT authentication in ASP.NET Core MVC
- Claims based identity
- Cookie based token storage
- MVC architecture
- Entity Framework usage
