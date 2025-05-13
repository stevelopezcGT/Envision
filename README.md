# Crypto Price Tracker – Full Stack Developer Challenge

## 📋 Objective

This challenge is designed to evaluate your skills as a full stack developer using .NET 6 (C#), Entity Framework Core, Razor Pages, and REST API integration.

You'll be working with a project that simulates tracking cryptocurrency prices using the CoinGecko API.

---

## 🧠 What You Need to Do

### ✅ 1. Fix the Service Logic

- Open `CryptoPriceService.cs` and fix the logic so it correctly:
  - Fetches coin data from CoinGecko for all existing coins  
    Documentation: https://docs.coingecko.com/v3.0.1/reference/introduction
  - Handles async operations properly
  - Prevents duplicate entries
  - Saves the results to the SQLite database

  ⚠️ If you encounter duplicate entries or errors from the CoinGecko API, decide how to handle them.  
  You're free to choose the best approach — just make sure you explain your reasoning in a short code comment.

  ⚠️ Some files may have outdated or incorrect `namespace` declarations. Please review and update them as needed to ensure consistency across the project.

### ✅ 2. Complete the Controller

- Open `CryptoController.cs` and:
  - Complete the `POST /api/crypto/update-prices` endpoint
  - Add a `GET /api/crypto/latest-prices` endpoint to return the most recent price per asset. All available cryptocoins must be saved.

### ✅ 3. Test the Razor Frontend

- The view in `/Views/Home/Index.cshtml` includes a button to call the API.
- Ensure that the interface provides clear feedback to the user indicating whether the update succeeded or failed.

### ✅ 4. Create the Frontend Visualization in `Index.cshtml`

Build a clean and user-friendly visualization directly in the existing Razor view:  
**`/Views/Home/Index.cshtml`**

This page must:

- Display **each cryptocurrency asset** with at least the following fields:
  - ✅ **Name**
  - ✅ **Symbol**
  - ✅ **Current Price**
  - ✅ **Currency**
  - ✅ **Icon**
    - Add the `IconUrl` property to the `CryptoAsset` model
    - Retrieve the image url from CoinGecko
    - Save it to the database
  - ✅ **Last Updated**
    - This value must be converted to the client's local timezone using JavaScript
  - ✅ **Trend**
    - Add a visual indicator showing whether the price has increased, decreased, or stayed the same compared to the previous saved price (e.g., 🔼, 🔽, ➖)
    - Optionally, include the percentage change

- Use the existing "Update Prices" button to trigger the API and refresh the data displayed in the view.

> 🎯 **Goal:** The user should be able to open the page, see the most recent cryptocurrency data clearly presented, and update it with one click.

> 💡 Bonus points for improved UI, animations, or anything that enhances clarity and usability.

### ✅ 5. Add Improvements

- Add validation logic or error handling where needed
- Create unit tests for validator or service logic

---

## ⚙️ How to Run the Project

1. Make sure you have **.NET 6 SDK** installed.
2. Navigate to the backend project folder:
   ```bash
   cd Backend/CryptoPriceTracker.Api
   ```
3. Restore dependencies:
   ```bash
   dotnet restore
   ```
4. Generate your own migrations and update the database:
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
5. Run the project:
   ```bash
   dotnet run
   ```
6. Navigate to `http://localhost:5000` to test the Razor page.

---

## 🧪 Testing

You may add unit tests to demonstrate how you validate business logic (e.g., in the `PriceValidator` class).

---

## 💬 Assumptions and Decisions

✏️ If you made any assumptions due to missing or unclear requirements, please **leave a brief comment** in the code explaining what you assumed and why.

---

## 🧠 Optional Architecture Upgrade

💡 You may reorganize the project structure or apply your preferred architecture (e.g., Clean Architecture, layered services, etc.)  
as long as:

- The functionality remains the same  
- The Razor page and API endpoints still work as requested  
- You document your reasoning in code comments or a note in the README

---

## 📝 Notes

- 🔸 **Do not include generated migrations or database files in your submission.**
- The project uses SQLite via EF Core for simplicity.
- CoinGecko API is public and doesn't require authentication for basic price lookups.
- The crypto icon should be retrieved from the CoinGecko API.

---

## 🧾 Deliverables Checklist

Before submitting your solution, please ensure the following items are completed:

- [x] `CryptoPriceService.cs` fetches, saves, and avoids duplicates correctly
- [x] `namespace` declarations are consistent and correct across all files
- [x] `POST /api/crypto/update-prices` endpoint is functional
- [x] `GET /api/crypto/latest-prices` endpoint returns most recent prices
- [x] `IconUrl` property added to `CryptoAsset` model and stored in DB
- [x] Razor page (`Index.cshtml`) includes:
  - [x] Name
  - [x] Symbol
  - [x] Current Price and currency
  - [x] Icon
  - [x] Last Updated (adjusted to client timezone)
  - [x] Trend (up/down indicator and optional percentage change)
- [x] Button updates data and refreshes the view
- [x] Clear UI feedback after update attempts (success/failure)
- [x] Validation/error handling is implemented where needed
- [x] At least one unit test demonstrates validation or service logic
- [x] Comments provided for any assumptions or technical decisions

---

Good luck, and thank you for taking the time to complete this challenge!

---
# Challenge solution

## 1. Clean Architecture for Flexibility, Maintainability & Scalability
The core of the solution is organized into concentric layers:

- **Entities (Domain Models)**  
- **Use Cases (Application Logic)**  
- **Interfaces / Adapters (API, UI, Database)**  
- **Frameworks / Infrastructure**  

By depending only inward, we ensure:
- **Flexibility:**  
  - Swap databases, UI frameworks, or third-party libraries by changing only the outer layers.  
- **Maintainability:**  
  - Each layer has a well-defined responsibility and interface, making it easy to locate and update code.  
- **Scalability:**  
  - Add new features (e.g., extra UI panels or integrations) as new adapters without touching core business logic.  

---

## 2. SOLID Principles & Single Responsibility
Every class and module adheres to SOLID:

1. **Single Responsibility Principle (SRP)**  
   - Classes and methods do exactly one thing—no tangled responsibilities.  
2. **Open/Closed Principle (OCP)**  
   - Core logic is open for extension but closed for modification.  
3. **Liskov Substitution Principle (LSP)**  
   - Abstractions allow interchangeable implementations without side effects.  
4. **Interface Segregation Principle (ISP)**  
   - Clients depend only on the methods they use—no “fat” interfaces.  
5. **Dependency Inversion Principle (DIP)**  
   - High-level modules depend on abstractions, not on low-level details; dependencies are injected.  

---

## 3. DRY, YAGNI & KISS for Readability and Simplicity
- **DRY (Don’t Repeat Yourself):**  
  - Shared functionality lives in utility classes or base components, eliminating duplication.  
- **YAGNI (You Aren’t Gonna Need It):**  
  - Only implement what’s required today; resist “just in case” features.  
- **KISS (Keep It Simple, Stupid):**  
  - Favor straightforward implementations that any team member can understand at a glance.  

These principles keep the codebase lean, readable, and free from unused abstractions.

---

## 4. Separation of Concerns
Responsibilities are divided across layers:

- **Domain Logic**  
  - Unaware of persistence or presentation details.  
- **Data Access**  
  - Abstracted behind repository interfaces.  
- **Presentation / UI**  
  - Handles user interaction, delegates all business decisions to the use-case layer.  

This separation yields clearer code, easier parallel development, and more targeted testing.

---

## 5. Unit Testing for Quality Assurance
Every critical component is covered by unit tests:

- **Domain Tests**  
  - Validate business rules under success and failure scenarios.  
- **Use-Case Tests**  
  - Confirm application workflows behave correctly across various scenarios.  
- **Adapter Mocks**  
  - Fake out persistence, network, or UI layers so tests remain fast and deterministic. 
