# DigiBuy

DigiBuy is a RESTful API designed for managing categories, coupons, orders, products, and users in an e-commerce system. This API is built with ASP.NET Core.

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Installation](#installation)
- [Configuration](#configuration)
- [How To Run](#how-to-run)
- [API Documentation](#api-documentation)
- [Screenshots](#screenshots)

## Features

- **Category Management**: Create, update, delete, and retrieve product categories.
- **Coupon Management**: Handle creation, usage, and retrieval of discount coupons.
- **Order Management**: Manage customer orders, including details and status tracking.
- **Order Detail Management**: Manage customer orders' items, including details and status tracking.
- **Product Management**: Full CRUD operations for products, including stock and pricing.
- **User Management**: Register, authenticate, and manage users, including admin roles.

## Tech Stack

- **.NET Core**: Backend framework.
- **Entity Framework Core**: ORM for database access.
- **PostgreSQL**: Database management system.
- **Swagger**: API documentation and testing.
- **JWT**: Authentication method.
- **Entity Framework Identity**: Authentication and authorization management.
- **AutoMapper**: Object-object mapping library.
- **Serilog**: Logging library for structured logging.
- **RabbitMQ**: Message broker for handling email task.
- **Redis**: In-memory data structure store for caching.
- **Hangfire**: Background job processing for handling email tasks from RabbitMQ queues and monitorize them.
- **SMTP**: Email service for notifications.

## Installation

### Prerequisites

- .NET 8.0 SDK
- PostgreSQL - You can install and use in Docker.
- Optionally Postman for testing API endpoints but can be used with built-in swagger.
- RabbitMQ - You can install and use in Docker.
- Redis - You can install and use in Docker.

### Clone the Repository
- `git clone git@github.com:Ntolgka/DigiBuy.git`

### Install dependencies
- In DigiBuy.Api directory: `dotnet restore`

## Configuration

### In `appsettings.json`
- Update database connection string `PostgresSqlConnection`
- Update hangfire connection string `HangfireConnection`
- Update JWT configuration `JwtSettings`
- Update Redis configuration `Redis`
- Update RabbitMQ configuration `RabbitMQ`
- Update SMTP configuration `SMTPConfig`

## How To Run

1 - Apply Migrations
- In DigiBuy.Infrastructure directory: `dotnet ef database update`

2 - Start the API
- In DigiBuy.Api directory: `dotnet run`

## API Documentation
Full API documentation is available [here](https://documenter.getpostman.com/view/26248957/2sA3s3JBu2/).

### Available Endpoints

The API provides the following endpoints:
- **Category**: `/api/Category`
- **Coupon**: `/api/Coupon`
- **Order**: `/api/Order`
- **Order Detail**: `/api/OrderDetail`
- **Product**: `/api/Product`
- **User**: `/api/User`
- **Checkout**: `/api/Checkout`

Please check the [API Documentation](https://documenter.getpostman.com/view/26248957/2sA3s3JBu2/) for detailed information on each endpoint, including request payloads.

## Screenshots

Below are some screenshots demonstrating the API requests.

### User Endpoints
![User Endpoints](./assets/user-endpoints.png)

### Product Endpoints
![User Endpoints](./assets/product-endpoints.png)

### Category Endpoints
![User Endpoints](./assets/category-endpoints.png)

### Coupon Endpoints
![User Endpoints](./assets/coupon-endpoints.png)

### Order Endpoints
![User Endpoints](./assets/order-endpoints.png)

### OrderDetail Endpoints
![User Endpoints](./assets/orderdetail-endpoints.png)

### Checkout Endpoint
![User Endpoints](./assets/checkout-endpoint.png)

> These are only some of the endpoints, not all of them.