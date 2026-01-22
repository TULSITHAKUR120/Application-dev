//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using System;
//using System.IO;

//namespace DailyJournal.Data.Database
//{
//    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
//    {
//        public AppDbContext CreateDbContext(string[] args)
//        {
//            Console.WriteLine("=== Design-Time DbContext Factory ===");

//            // Get the base path (project directory)
//            var basePath = Directory.GetCurrentDirectory();
//            Console.WriteLine($"Current Directory: {basePath}");

//            // Go up to solution directory if needed
//            var solutionPath = Path.GetFullPath(Path.Combine(basePath, "..", ".."));
//            Console.WriteLine($"Solution Path: {solutionPath}");

//            // Use a simple physical file
//            var dbPath = Path.Combine(basePath, "migration-test.db");
//            Console.WriteLine($"Database Path: {dbPath}");

//            // Ensure the file doesn't exist
//            if (File.Exists(dbPath))
//            {
//                File.Delete(dbPath);
//                Console.WriteLine("Deleted old migration database");
//            }

//            // Create options
//            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
//            optionsBuilder.UseSqlite($"Data Source={dbPath}");

//            Console.WriteLine("DbContext created successfully!");
//            return new AppDbContext(optionsBuilder.Options);
//        }
//    }
//}