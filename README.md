# ETicaretSitesi - E-Commerce Platform

A full-featured e-commerce web application built with ASP.NET Core 8.0 and Entity Framework, featuring AI-powered product recommendations and comprehensive order management.

## 🚀 Features

### Core E-Commerce Functionality
- **Product Management**: Complete CRUD operations for products with image uploads
- **Shopping Cart**: Advanced cart system with session management
- **Order Processing**: Full order lifecycle management with status tracking
- **User Authentication**: Dual authentication system for customers and administrators
- **Payment Integration**: Stripe payment gateway integration
- **Admin Panel**: Comprehensive dashboard for managing products, orders, and users

### AI-Powered Features
- **Product Recommendations**: ML.NET-based recommendation engine for personalized shopping experiences
- **Smart Suggestions**: AI-driven product suggestions based on user behavior and purchase history

### User Management
- **Customer Accounts**: User registration, login, and profile management
- **Admin Accounts**: Separate admin authentication with elevated privileges
- **Role-Based Access**: Secure role-based authorization system

## 🛠 Technology Stack

- **Framework**: ASP.NET Core 8.0 (MVC)
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: Cookie-based authentication with dual schemes
- **Payment Processing**: Stripe.NET
- **Machine Learning**: ML.NET with Recommender algorithms
- **Security**: BCrypt.Net for password hashing
- **UI**: Razor Views with modern responsive design

## 📦 Dependencies

```xml
- Microsoft.AspNetCore.Identity.EntityFrameworkCore (8.0.0)
- Microsoft.EntityFrameworkCore.SqlServer (8.0.0)
- Stripe.net (43.20.0)
- Microsoft.ML (3.0.1)
- Microsoft.ML.Recommender (0.21.1)
- BCrypt.Net-Next (4.0.3)
```

## 🏗 Project Structure

```
ETicaretSitesi/
├── Controllers/         # MVC Controllers
│   ├── AdminController.cs
│   ├── HomeController.cs
│   ├── KullaniciController.cs
│   ├── SepetController.cs
│   ├── SiparisController.cs
│   └── RecommendationController.cs
├── Models/             # Data models and ViewModels
├── Views/              # Razor view templates
├── Services/           # Business logic services
├── Data/               # Entity Framework context
├── Migrations/         # Database migrations
└── wwwroot/           # Static files (CSS, JS, images)
```

## 🔧 Setup Instructions

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code

### Installation
1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run database migrations:
   ```bash
   dotnet ef database update
   ```
4. Configure Stripe keys in `appsettings.json`
5. Run the application:
   ```bash
   dotnet run
   ```

## 🔐 Authentication System

The application features a dual authentication system:
- **Customer Authentication**: Standard user accounts with 24-hour session expiry
- **Admin Authentication**: Administrative accounts with 8-hour session expiry and enhanced security

## 🤖 AI Recommendation Engine

The platform includes an advanced recommendation system powered by ML.NET that:
- Analyzes user purchase patterns
- Provides personalized product suggestions
- Improves user engagement and sales conversion

## 📱 Key Features

- **Responsive Design**: Mobile-friendly interface
- **Session Management**: Persistent shopping cart across sessions
- **Order Tracking**: Real-time order status updates
- **Product Search**: Advanced product filtering and search
- **Image Upload**: Support for product image management
- **Security**: Comprehensive security measures with encrypted passwords

## 🌟 Admin Features

- Dashboard with sales analytics
- Product inventory management
- Order management and fulfillment
- User account management
- Real-time statistics and reporting

## 💳 Payment Processing

Secure payment processing through Stripe integration with support for:
- Credit/Debit cards
- Secure checkout process
- Payment status tracking
- Order confirmation system

---

This project represents a modern, scalable e-commerce solution with AI-enhanced features for improved user experience and business intelligence.
