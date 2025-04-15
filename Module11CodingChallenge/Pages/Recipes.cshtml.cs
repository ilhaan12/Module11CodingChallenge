using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace Module11CodingChallenge.Pages
{
    // PageModel handles the server-side logic for the Recipes Razor Page
    public class RecipesModel : PageModel
    {
        // Property bound to the dropdown selection in the form
        [BindProperty]
        public int SelectedRecipeId { get; set; }

        // List of recipes for the dropdown menu
        public List<SelectListItem> RecipeList { get; set; }

        // The currently selected recipe's details (to be displayed)
        public Recipe SelectedRecipe { get; set; }

        // This runs when the page is first loaded (HTTP GET)
        public void OnGet()
        {
            LoadRecipeList(); // Populate dropdown with recipes
        }

        // This runs when the form is submitted (HTTP POST)
        public IActionResult OnPost()
        {
            LoadRecipeList(); // Always reload the dropdown
            if (SelectedRecipeId != 0)
            {
                SelectedRecipe = GetRecipeById(SelectedRecipeId); // Load selected recipe
            }
            return Page();
        }

        // Helper method to load all recipe names for the dropdown
        private void LoadRecipeList()
        {
            RecipeList = new List<SelectListItem>();

            using (var connection = new SqliteConnection("Data Source=Recipes.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Id, Name FROM Recipes";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        RecipeList.Add(new SelectListItem
                        {
                            Value = reader.GetInt32(0).ToString(), // Recipe ID
                            Text = reader.GetString(1)              // Recipe Name
                        });
                    }
                }
            }
        }

        // Helper method to get all details of a selected recipe from the database
        private Recipe GetRecipeById(int id)
        {
            using (var connection = new SqliteConnection("Data Source=Recipes.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM Recipes WHERE Id = @Id";
                command.Parameters.AddWithValue("@Id", id); // Prevents SQL injection

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Create and return a Recipe object populated from the DB
                        return new Recipe
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Description = reader.GetString(2),
                            Ingredients = reader.GetString(3),
                            Instructions = reader.GetString(4),
                            ImageFileName = reader.GetString(5)
                        };
                    }
                }
            }

            return null; // If no recipe found
        }

        // Inner class representing the Recipe data model
        public class Recipe
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Ingredients { get; set; }
            public string Instructions { get; set; }
            public string ImageFileName { get; set; }
        }
    }
}
