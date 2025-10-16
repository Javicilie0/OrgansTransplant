LOADING TEST DATA

🎯 EASIEST WAY – Use the button in the app! ⭐

Steps:
-> Launch the application
-> In the sidebar, find the “🗄️ Load Test Data” button
-> Click the button
-> Confirm the action
Done! ✅

What gets loaded:
✅ 25 donors from 10+ cities
📍 Sofia (5), Plovdiv (3), Varna (3), Burgas (3), Ruse (2), and more
🫀 All organ types: Heart, Kidney, Liver, Lung, Pancreas, Stomach, Artery, Thymus, Intestines
🩸 All blood groups: A+, A-, B+, B-, AB+, AB-, O+, O-


📁 Other ways (for developers)
1. TestDataSeeder.cs – C# class
Use in code:  TestDataSeeder.SeedTestData();
What it does:
-> Checks that the database is empty
-> Automatically loads 25 donors
-> Throws an error if the database isn’t empty
2. SeedData.sql – SQL script
With Railway CLI or a MySQL client:  railway run mysql -u root -p < SeedData.sql
Or copy the SQL and run it in MySQL Workbench/phpMyAdmin


🧹 Clearing the database
If you want a fresh start and to reload the data:
Option 1: Manual delete
-> Open the app
-> Go to “👥 View Donors”
-> Delete all donors one by one using the 🗑 button

Option 2: SQL command (for developers)
DELETE FROM Donors;
After that, you can load the test data again using the button!
