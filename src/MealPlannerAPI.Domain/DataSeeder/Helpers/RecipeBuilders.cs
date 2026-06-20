using MealPlannerAPI.Enums;
using MealPlannerAPI.Recipes;
using System;
using System.Collections.Generic;
using Volo.Abp.Guids;

namespace MealPlannerAPI.DataSeeder.Helpers;


public static class RecipeBuilders
{

    public static readonly IReadOnlyList<RecipeDefinition> All = new[]
    {
        new RecipeDefinition(
            Name:         "Grilled Chicken with Broccoli",
            Cuisine:      "American",
            Description:  "A clean high-protein meal with grilled chicken breast and steamed broccoli.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 20,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Chicken Breast", 300, "300g"),
                new IngredientDef("Broccoli",       200, "200g"),
                new IngredientDef("Olive Oil",       10,  "1 tbsp"),
                new IngredientDef("Garlic",           5,  "2 cloves"),
            },
            Instructions: new[]
            {
                "Season chicken breasts with salt, pepper, and minced garlic.",
                "Heat olive oil in a grill pan over medium-high heat.",
                "Grill chicken for 6-7 minutes per side until cooked through and juices run clear.",
                "Meanwhile, steam broccoli florets for 5-6 minutes until tender-crisp.",
                "Rest chicken for 3 minutes, then slice and serve alongside broccoli.",
            }
        ),

        new RecipeDefinition(
            Name:         "Salmon with Brown Rice",
            Cuisine:      "Japanese",
            Description:  "Omega-3 rich salmon fillet served over nutty brown rice.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 25,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Salmon",           250, "250g"),
                new IngredientDef("Brown Rice (Dry)",  80,  "80g dry"),
                new IngredientDef("Olive Oil",          8,  "1 tbsp"),
                new IngredientDef("Spinach",           50,  "handful"),
            },
            Instructions: new[]
            {
                "Cook brown rice according to package directions (about 20-25 minutes).",
                "Pat salmon dry and season with salt and pepper.",
                "Heat olive oil in a non-stick pan over medium-high heat.",
                "Cook salmon skin-side up for 4 minutes, then flip and cook for another 3-4 minutes.",
                "Wilt spinach in the same pan for 1-2 minutes.",
                "Serve salmon over rice with spinach on the side.",
            }
        ),

        new RecipeDefinition(
            Name:         "Pasta Pomodoro",
            Cuisine:      "Italian",
            Description:  "Classic Italian pasta with a simple fresh tomato sauce.",
            Servings:     4, PrepMinutes: 10, CookMinutes: 20,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Pasta (Dry)", 320, "320g"),
                new IngredientDef("Tomato",      400, "4 large"),
                new IngredientDef("Garlic",       10,  "4 cloves"),
                new IngredientDef("Olive Oil",    20,  "2 tbsp"),
                new IngredientDef("Onion",        80,  "1 medium"),
            },
            Instructions: new[]
            {
                "Bring a large pot of salted water to a boil and cook pasta until al dente.",
                "Dice tomatoes and finely chop onion and garlic.",
                "Heat olive oil in a saucepan over medium heat. Sauté onion for 3 minutes until soft.",
                "Add garlic and cook for 1 minute until fragrant.",
                "Add diced tomatoes and simmer for 10 minutes, stirring occasionally.",
                "Season sauce with salt and pepper. Drain pasta and toss with the sauce. Serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Spinach and Cheese Omelette",
            Cuisine:      "French",
            Description:  "Fluffy omelette loaded with wilted spinach and melted cheddar.",
            Servings:     1, PrepMinutes: 5, CookMinutes: 8,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Egg",            150, "3 eggs"),
                new IngredientDef("Spinach",         60,  "handful"),
                new IngredientDef("Cheddar Cheese",  30,  "30g"),
                new IngredientDef("Butter",          10,  "1 tsp"),
            },
            Instructions: new[]
            {
                "Crack eggs into a bowl, season with salt and pepper, and whisk until smooth.",
                "Melt butter in a non-stick pan over medium heat.",
                "Add spinach and sauté for 1-2 minutes until wilted. Set aside.",
                "Pour eggs into the pan and cook, gently pulling edges toward the center.",
                "When eggs are just set, add spinach and shredded cheddar to one half.",
                "Fold omelette in half and slide onto a plate. Serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Sweet Potato & Black Bean Bowl",
            Cuisine:      "Mexican",
            Description:  "Hearty plant-based bowl with roasted sweet potato and spiced black beans.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 30,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Sweet Potato",         300, "2 medium"),
                new IngredientDef("Black Beans (Cooked)", 200, "1 can drained"),
                new IngredientDef("Olive Oil",             15,  "1 tbsp"),
                new IngredientDef("Onion",                 80,  "1 medium"),
                new IngredientDef("Garlic",                 5,  "2 cloves"),
            },
            Instructions: new[]
            {
                "Preheat oven to 200°C (400°F). Cube sweet potatoes and toss with olive oil, salt, and pepper.",
                "Roast sweet potatoes on a baking sheet for 25-30 minutes, flipping halfway, until golden.",
                "Meanwhile, sauté diced onion in a pan over medium heat for 3 minutes.",
                "Add minced garlic and cook for 1 minute.",
                "Add drained black beans, season with cumin and salt, and cook for 5 minutes until heated through.",
                "Assemble bowls with roasted sweet potato and spiced black beans. Serve warm.",
            }
        ),

        new RecipeDefinition(
            Name:         "Chicken Fried Rice",
            Cuisine:      "Chinese",
            Description:  "Classic homemade fried rice with chicken breast, eggs, and onions.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 15,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("White Rice (Dry)", 100, "100g dry"),
                new IngredientDef("Chicken Breast",   150, "150g"),
                new IngredientDef("Egg",              100, "2 eggs"),
                new IngredientDef("Onion",             50,  "half onion"),
                new IngredientDef("Olive Oil",         15,  "1 tbsp"),
            },
            Instructions: new[]
            {
                "Cook rice and let it cool (preferably overnight in the fridge for best texture).",
                "Dice chicken and season with salt and pepper.",
                "Heat oil in a wok or large pan over high heat. Stir-fry chicken for 5-6 minutes until cooked.",
                "Push chicken to the side, add a little oil, and scramble the eggs in the pan.",
                "Add diced onion and cook for 2 minutes, then add the cold rice and stir-fry everything together.",
                "Season with soy sauce (optional) and stir-fry for another 2-3 minutes. Serve hot.",
            }
        ),

        new RecipeDefinition(
            Name:         "Banana Peanut Butter Oatmeal",
            Cuisine:      "Breakfast",
            Description:  "A hearty breakfast bowl of oats topped with fresh banana and peanut butter.",
            Servings:     1, PrepMinutes: 5, CookMinutes: 10,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Oats (Dry)",    50,  "50g"),
                new IngredientDef("Whole Milk",   150,  "150ml"),
                new IngredientDef("Banana",       118,  "1 medium"),
                new IngredientDef("Peanut Butter", 32,  "2 tbsp"),
                new IngredientDef("Honey",         10,  "1 tsp"),
            },
            Instructions: new[]
            {
                "Combine oats and milk in a small saucepan over medium heat.",
                "Cook, stirring frequently, for 5-7 minutes until oats are creamy and milk is absorbed.",
                "Pour oatmeal into a bowl and top with sliced banana.",
                "Add a dollop of peanut butter and drizzle with honey.",
                "Serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Ground Beef and Potato Bake",
            Cuisine:      "Continental",
            Description:  "A comforting casserole with layers of potato, seasoned ground beef, and cheesy goodness.",
            Servings:     4, PrepMinutes: 15, CookMinutes: 45,
            Difficulty:   DifficultyLevel.Hard,
            Ingredients: new[]
            {
                new IngredientDef("Ground Beef",    400, "400g"),
                new IngredientDef("Potato",         500, "4 medium"),
                new IngredientDef("Cheddar Cheese", 100, "100g grated"),
                new IngredientDef("Onion",          100, "1 large"),
                new IngredientDef("Garlic",          10,  "4 cloves"),
                new IngredientDef("Olive Oil",       15,  "1 tbsp"),
            },
            Instructions: new[]
            {
                "Preheat oven to 190°C (375°F). Thinly slice potatoes and set aside.",
                "Heat olive oil in a pan and brown ground beef with onion and garlic over medium-high heat.",
                "Season beef mixture with salt, pepper, and your choice of herbs. Drain excess fat.",
                "Layer half the potato slices in a greased baking dish. Season with salt and pepper.",
                "Spread the beef mixture over the potatoes, then top with remaining potato slices.",
                "Cover with grated cheddar and bake for 40-45 minutes until potatoes are tender and cheese is golden.",
            }
        ),

        new RecipeDefinition(
            Name:         "Greek Yogurt Parfait",
            Cuisine:      "Breakfast",
            Description:  "A quick and healthy snack or breakfast with Greek yogurt and honey.",
            Servings:     1, PrepMinutes: 5, CookMinutes: 0,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Greek Yogurt", 200, "200g"),
                new IngredientDef("Honey",         15,  "1 tbsp"),
                new IngredientDef("Banana",       118,  "1 medium"),
                new IngredientDef("Oats (Dry)",    20,  "2 tbsp"),
            },
            Instructions: new[]
            {
                "Spoon Greek yogurt into a glass or bowl.",
                "Slice banana and layer on top of the yogurt.",
                "Sprinkle dry oats over the banana for crunch.",
                "Drizzle with honey and serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Peanut Butter Banana Toast",
            Cuisine:      "Breakfast",
            Description:  "Simple and delicious toast topped with peanut butter and sliced bananas.",
            Servings:     1, PrepMinutes: 5, CookMinutes: 2,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Bread (White)",  70,  "2 slices"),
                new IngredientDef("Peanut Butter",  30,  "2 tbsp"),
                new IngredientDef("Banana",        118,  "1 medium"),
            },
            Instructions: new[]
            {
                "Toast bread slices until golden.",
                "Spread peanut butter generously over each slice.",
                "Slice banana and arrange on top of the peanut butter.",
                "Serve immediately, optionally drizzled with a little honey.",
            }
        ),

        new RecipeDefinition(
            Name:         "Beef and Bell Pepper Stir Fry",
            Cuisine:      "Asian",
            Description:  "A quick stir fry with ground beef, vibrant bell peppers, and savory garlic.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 15,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Ground Beef",      250, "250g"),
                new IngredientDef("Bell Pepper",      150, "1 large"),
                new IngredientDef("Onion",             80,  "1 medium"),
                new IngredientDef("Garlic",            10,  "4 cloves"),
                new IngredientDef("Olive Oil",         15,  "1 tbsp"),
                new IngredientDef("White Rice (Dry)", 120,  "120g dry"),
            },
            Instructions: new[]
            {
                "Cook rice according to package instructions.",
                "Slice bell pepper and dice onion and garlic.",
                "Heat oil in a wok over high heat. Add ground beef and cook until browned, breaking it up.",
                "Add onion and bell pepper, stir-fry for 3-4 minutes until slightly softened.",
                "Add garlic and cook for 1 more minute. Season with soy sauce, salt, and pepper.",
                "Serve stir-fry over steamed rice.",
            }
        ),

        new RecipeDefinition(
            Name:         "Chicken and Spinach Salad",
            Cuisine:      "American",
            Description:  "A light salad featuring grilled chicken, fresh spinach, and sweet cherry tomatoes.",
            Servings:     2, PrepMinutes: 15, CookMinutes: 15,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Chicken Breast", 200, "200g"),
                new IngredientDef("Spinach",        100, "2 large handfuls"),
                new IngredientDef("Tomato",         150, "1 cup cherry tomatoes"),
                new IngredientDef("Olive Oil",       30,  "2 tbsp"),
            },
            Instructions: new[]
            {
                "Season chicken with salt, pepper, and a drizzle of olive oil.",
                "Grill or pan-fry chicken over medium-high heat for 6-7 minutes per side until cooked through.",
                "Rest chicken for 3 minutes, then slice thinly.",
                "Arrange spinach and halved tomatoes in a bowl.",
                "Top with sliced chicken. Drizzle remaining olive oil over the salad and season to taste.",
            }
        ),

        new RecipeDefinition(
            Name:         "Avocado Toast",
            Cuisine:      "Australian",
            Description:  "Trendy and nutritious avocado on toast with a fried egg.",
            Servings:     1, PrepMinutes: 5, CookMinutes: 5,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Bread (White)", 70,  "2 slices"),
                new IngredientDef("Avocado",       100, "1/2 avocado"),
                new IngredientDef("Egg",            50,  "1 egg"),
                new IngredientDef("Olive Oil",       5,  "1 tsp"),
            },
            Instructions: new[]
            {
                "Toast bread slices until golden and crispy.",
                "Halve the avocado, remove the pit, and scoop flesh into a bowl.",
                "Mash avocado with a fork and season with salt, pepper, and a squeeze of lemon.",
                "Heat olive oil in a small pan and fry the egg to your liking (sunny-side up or poached).",
                "Spread mashed avocado on toast, top with the fried egg, and serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Mushroom Omelette",
            Cuisine:      "Breakfast",
            Description:  "Savory mushroom and cheese omelette.",
            Servings:     1, PrepMinutes: 5, CookMinutes: 10,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Egg",            150, "3 eggs"),
                new IngredientDef("Mushrooms",      100, "1 cup sliced"),
                new IngredientDef("Cheddar Cheese",  30,  "1/4 cup"),
                new IngredientDef("Butter",          10,  "1 tsp"),
                new IngredientDef("Onion",           20,  "1/4 onion"),
            },
            Instructions: new[]
            {
                "Whisk eggs with a pinch of salt and pepper in a bowl.",
                "Melt butter in a non-stick pan over medium heat. Sauté diced onion for 2 minutes.",
                "Add sliced mushrooms and cook for 3-4 minutes until golden and moisture has evaporated.",
                "Remove mushroom mixture from pan and set aside.",
                "Pour beaten eggs into the pan. Cook gently, pulling edges inward as they set.",
                "When eggs are almost set, add mushrooms and shredded cheese to one half. Fold and serve.",
            }
        ),

        new RecipeDefinition(
            Name:         "Tofu Stir Fry",
            Cuisine:      "Asian",
            Description:  "Quick and healthy vegetarian stir fry.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 15,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Tofu",        200, "200g firm"),
                new IngredientDef("Bell Pepper", 150, "1 large"),
                new IngredientDef("Spinach",      80,  "1 cup"),
                new IngredientDef("Soy Sauce",    30,  "2 tbsp"),
                new IngredientDef("Olive Oil",    15,  "1 tbsp"),
                new IngredientDef("Garlic",       10,  "3 cloves"),
            },
            Instructions: new[]
            {
                "Press tofu dry with paper towels and cube.",
                "Heat oil in a wok over high heat. Fry tofu cubes until golden on each side, about 5 minutes. Set aside.",
                "In the same wok, stir-fry garlic and bell pepper for 2-3 minutes.",
                "Add spinach and cook until wilted.",
                "Return tofu to the wok, add soy sauce, and toss everything together.",
                "Serve immediately over steamed rice.",
            }
        ),

        new RecipeDefinition(
            Name:         "Pork Chop with Sweet Potato",
            Cuisine:      "American",
            Description:  "Pan-seared pork chops served with roasted sweet potato.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 30,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Pork Chop",    300, "2 chops"),
                new IngredientDef("Sweet Potato", 300, "2 medium"),
                new IngredientDef("Olive Oil",     15,  "1 tbsp"),
                new IngredientDef("Garlic",         5,  "2 cloves"),
            },
            Instructions: new[]
            {
                "Preheat oven to 200°C (400°F). Cube sweet potato, toss with olive oil and salt, and roast for 25 minutes.",
                "Season pork chops with salt, pepper, and minced garlic.",
                "Heat a pan over medium-high heat and sear chops for 4 minutes per side.",
                "Rest for 3 minutes before serving alongside the roasted sweet potato.",
            }
        ),

        new RecipeDefinition(
            Name:         "Hearty Lentil Soup",
            Cuisine:      "Mediterranean",
            Description:  "A warming lentil soup full of fiber and plant-based protein.",
            Servings:     4, PrepMinutes: 10, CookMinutes: 35,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Lentils (Dry)", 200, "1 cup"),
                new IngredientDef("Carrot",        150, "2 medium"),
                new IngredientDef("Onion",         100, "1 large"),
                new IngredientDef("Garlic",         10,  "3 cloves"),
                new IngredientDef("Tomato",        200, "2 medium"),
                new IngredientDef("Olive Oil",      20,  "2 tbsp"),
            },
            Instructions: new[]
            {
                "Dice onion, carrot, and tomato. Mince garlic.",
                "Heat olive oil in a large pot over medium heat. Sauté onion and carrot for 4 minutes.",
                "Add garlic and cook 1 minute until fragrant.",
                "Stir in lentils and tomato. Pour in 1 litre of water or broth.",
                "Bring to a boil, then simmer for 25-30 minutes until lentils are tender.",
                "Season with salt, cumin, and lemon juice. Serve warm.",
            }
        ),

        new RecipeDefinition(
            Name:         "Greek Salad",
            Cuisine:      "Greek",
            Description:  "Crisp Mediterranean salad with cucumber, tomato, and olives.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 0,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Tomato",         200, "2 medium"),
                new IngredientDef("Cucumber",       200, "1 large"),
                new IngredientDef("Cheddar Cheese", 100, "feta substitute, 100g"),
                new IngredientDef("Olive Oil",       30,  "2 tbsp"),
                new IngredientDef("Onion",           60,  "1/2 red onion"),
            },
            Instructions: new[]
            {
                "Chop tomato and cucumber into large chunks. Thinly slice red onion.",
                "Combine vegetables in a bowl.",
                "Crumble cheese over the top and drizzle with olive oil.",
                "Season with salt, oregano, and a squeeze of lemon. Toss gently and serve.",
            }
        ),

        new RecipeDefinition(
            Name:         "Apple & Almond Snack",
            Cuisine:      "Snack",
            Description:  "A quick high-fiber snack combining fresh apple with protein-rich almonds.",
            Servings:     1, PrepMinutes: 2, CookMinutes: 0,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Apple",   150, "1 medium"),
                new IngredientDef("Almonds",  30,  "small handful"),
            },
            Instructions: new[]
            {
                "Slice the apple into wedges.",
                "Arrange on a plate alongside a small handful of almonds.",
                "Serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Chicken Rice Bowl",
            Cuisine:      "Asian",
            Description:  "Simple and satisfying chicken rice bowl.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 20,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Chicken Breast",   200, "200g"),
                new IngredientDef("White Rice (Dry)", 150, "150g dry"),
                new IngredientDef("Soy Sauce",         20,  "1 tbsp"),
                new IngredientDef("Olive Oil",         10,  "1 tbsp"),
                new IngredientDef("Garlic",             5,  "2 cloves"),
            },
            Instructions: new[]
            {
                "Cook rice according to package directions.",
                "Slice chicken breast and marinate with soy sauce and minced garlic for 5 minutes.",
                "Heat oil in a pan over medium-high heat and cook chicken for 5-6 minutes per side until cooked through.",
                "Slice chicken and serve over rice. Drizzle with any remaining pan juices.",
            }
        ),

        new RecipeDefinition(
            Name:         "Beef Burger Patty Salad",
            Cuisine:      "American",
            Description:  "Deconstructed burger — seasoned beef patty over a fresh lettuce and tomato salad.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 15,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Ground Beef", 300, "300g"),
                new IngredientDef("Lettuce",     100, "2 cups"),
                new IngredientDef("Tomato",      150, "1 large"),
                new IngredientDef("Onion",        60,  "1/2 onion"),
                new IngredientDef("Olive Oil",    10,  "1 tsp"),
            },
            Instructions: new[]
            {
                "Shape ground beef into 2 patties and season generously with salt and pepper.",
                "Cook patties in a hot pan with olive oil for 4-5 minutes per side.",
                "Tear lettuce and slice tomato and onion.",
                "Arrange salad on plates, slice patties, and place on top. Serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Peanut Butter Apple Toast",
            Cuisine:      "Breakfast",
            Description:  "Wholesome toast topped with peanut butter and fresh apple slices.",
            Servings:     1, PrepMinutes: 5, CookMinutes: 2,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Bread (White)",  70,  "2 slices"),
                new IngredientDef("Peanut Butter",  30,  "2 tbsp"),
                new IngredientDef("Apple",         100,  "1 small"),
            },
            Instructions: new[]
            {
                "Toast bread until golden.",
                "Spread peanut butter over each slice.",
                "Thinly slice apple and arrange on top.",
                "Serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Vegetable Fried Rice",
            Cuisine:      "Chinese",
            Description:  "Colorful fried rice packed with mixed vegetables.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 15,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("White Rice (Dry)", 150, "150g dry"),
                new IngredientDef("Carrot",            80,  "1 medium"),
                new IngredientDef("Bell Pepper",      100,  "1 medium"),
                new IngredientDef("Egg",              100,  "2 eggs"),
                new IngredientDef("Soy Sauce",         20,  "1 tbsp"),
                new IngredientDef("Olive Oil",         15,  "1 tbsp"),
            },
            Instructions: new[]
            {
                "Cook rice and let cool.",
                "Dice carrot and bell pepper.",
                "Heat oil in a wok over high heat. Stir-fry carrot and bell pepper for 3 minutes.",
                "Push vegetables to the side and scramble eggs in the pan.",
                "Add cold rice, pour in soy sauce, and stir-fry everything together for 2-3 minutes.",
                "Serve hot.",
            }
        ),

        new RecipeDefinition(
            Name:         "Mushroom Pasta",
            Cuisine:      "Italian",
            Description:  "Earthy mushroom pasta with garlic and olive oil.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 20,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Pasta (Dry)", 200, "200g"),
                new IngredientDef("Mushrooms",   200, "2 cups sliced"),
                new IngredientDef("Garlic",       10,  "3 cloves"),
                new IngredientDef("Olive Oil",    20,  "2 tbsp"),
                new IngredientDef("Spinach",      60,  "large handful"),
            },
            Instructions: new[]
            {
                "Cook pasta in salted boiling water until al dente. Reserve 1/2 cup pasta water.",
                "Heat olive oil in a large pan over medium-high heat. Sauté mushrooms until golden, about 5 minutes.",
                "Add minced garlic and cook for 1 minute.",
                "Add spinach and cook until wilted.",
                "Drain pasta and toss in the pan with mushrooms. Add pasta water as needed for a glossy sauce.",
                "Season with salt and pepper. Serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Spicy Tofu Scramble",
            Cuisine:      "Vegan",
            Description:  "A plant-based scrambled egg alternative packed with flavor.",
            Servings:     2, PrepMinutes: 5, CookMinutes: 10,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Tofu",      200, "200g firm"),
                new IngredientDef("Onion",      80,  "1 small"),
                new IngredientDef("Tomato",    100,  "1 medium"),
                new IngredientDef("Spinach",    60,  "1 cup"),
                new IngredientDef("Olive Oil",  15,  "1 tbsp"),
            },
            Instructions: new[]
            {
                "Press tofu dry with paper towels and crumble into a bowl.",
                "Dice onion and tomato.",
                "Heat olive oil in a pan over medium heat. Sauté onion for 3 minutes.",
                "Add crumbled tofu and cook for 4-5 minutes, stirring to break it up.",
                "Add tomato, spinach, and season with chili flakes, salt, and turmeric.",
                "Cook for 2-3 minutes until spinach is wilted. Serve warm.",
            }
        ),

        new RecipeDefinition(
            Name:         "Garlic Butter Pork Chops",
            Cuisine:      "American",
            Description:  "Rich and tender pork chops with garlic butter and potatoes.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 25,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Pork Chop", 300, "2 chops"),
                new IngredientDef("Garlic",     10,  "4 cloves"),
                new IngredientDef("Butter",     20,  "2 tbsp"),
                new IngredientDef("Potato",    300,  "2 medium"),
            },
            Instructions: new[]
            {
                "Boil or microwave potatoes until just tender. Slice into thick rounds.",
                "Season pork chops generously with salt and pepper.",
                "Melt 1 tbsp butter in a pan over medium-high heat. Sear chops for 4 minutes per side.",
                "Reduce heat to medium. Add remaining butter and minced garlic to the pan.",
                "Baste chops with the garlic butter for 2 minutes. Rest for 3 minutes.",
                "Add potato rounds to the pan and toss in remaining butter. Serve alongside pork chops.",
            }
        ),

        new RecipeDefinition(
            Name:         "Breakfast Burrito Bowl",
            Cuisine:      "Mexican",
            Description:  "Deconstructed burrito bowl — great for a filling breakfast.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 15,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Black Beans (Cooked)", 150, "1/2 can"),
                new IngredientDef("Egg",                  100, "2 eggs"),
                new IngredientDef("Avocado",              100, "1/2 avocado"),
                new IngredientDef("Tomato",               100, "1 medium"),
                new IngredientDef("Brown Rice (Dry)",     100, "100g"),
            },
            Instructions: new[]
            {
                "Cook brown rice according to package instructions.",
                "Warm black beans in a small pan, season with cumin and salt.",
                "Scramble eggs in a non-stick pan over medium heat until just set.",
                "Dice tomato and slice avocado.",
                "Divide rice between bowls. Top with black beans, scrambled eggs, avocado, and tomato.",
                "Season with salt and lime juice. Serve immediately.",
            }
        ),

        new RecipeDefinition(
            Name:         "Chicken & Mushroom Sauté",
            Cuisine:      "French",
            Description:  "Simple pan-fried chicken and mushrooms.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 20,
            Difficulty:   DifficultyLevel.Medium,
            Ingredients: new[]
            {
                new IngredientDef("Chicken Breast", 250, "250g"),
                new IngredientDef("Mushrooms",      200, "2 cups"),
                new IngredientDef("Onion",          100, "1 medium"),
                new IngredientDef("Olive Oil",       15,  "1 tbsp"),
            },
            Instructions: new[]
            {
                "Slice chicken breast into strips. Slice mushrooms and dice onion.",
                "Heat olive oil in a pan over medium-high heat.",
                "Cook chicken strips for 5-6 minutes until golden and cooked through. Set aside.",
                "In the same pan, sauté onion for 3 minutes. Add mushrooms and cook until golden, about 5 minutes.",
                "Return chicken to the pan, season with salt, pepper, and fresh thyme if available.",
                "Toss everything together and serve. Pairs well with crusty bread or rice.",
            }
        ),

        new RecipeDefinition(
            Name:         "Carrot & Lentil Mash",
            Cuisine:      "British",
            Description:  "Soft comforting mash of carrots and lentils.",
            Servings:     2, PrepMinutes: 10, CookMinutes: 30,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Carrot",        200, "2 large"),
                new IngredientDef("Lentils (Dry)", 100, "1/2 cup"),
                new IngredientDef("Butter",         20,  "2 tbsp"),
            },
            Instructions: new[]
            {
                "Rinse lentils under cold water. Peel and chop carrots into chunks.",
                "Combine lentils and carrots in a pot, cover with water, and bring to a boil.",
                "Reduce heat and simmer for 25-30 minutes until both are completely soft.",
                "Drain well. Add butter and mash together until smooth.",
                "Season with salt and pepper. Serve warm as a side dish.",
            }
        ),

        new RecipeDefinition(
            Name:         "Yogurt with Almonds",
            Cuisine:      "Snack",
            Description:  "Protein-rich yogurt snack with honey and almonds.",
            Servings:     1, PrepMinutes: 2, CookMinutes: 0,
            Difficulty:   DifficultyLevel.Easy,
            Ingredients: new[]
            {
                new IngredientDef("Greek Yogurt", 200, "200g"),
                new IngredientDef("Almonds",       30,  "1 handful"),
                new IngredientDef("Honey",         15,  "1 tbsp"),
            },
            Instructions: new[]
            {
                "Spoon Greek yogurt into a bowl.",
                "Scatter almonds over the yogurt.",
                "Drizzle with honey and serve immediately.",
            }
        ),
    };

    // ── Factory ───────────────────────────────────────────────────────────────


    public static Recipe Build(
        RecipeDefinition def,
        Dictionary<string, Guid> lookup,
        IGuidGenerator guidGenerator,
        Guid? authorId,
        string? authorUsername = null)
    {
        var ingredients = new List<(string Name, float Grams, string Display, Guid? NutritionId)>();
        foreach (var ing in def.Ingredients)
        {
            lookup.TryGetValue(ing.IngredientName, out var nutritionId);
            ingredients.Add((ing.IngredientName, ing.Grams, ing.Display, nutritionId == Guid.Empty ? null : nutritionId));
        }

        var recipe = Recipe.CreateSeed(
            id: guidGenerator.Create(),
            name: def.Name,
            cuisine: def.Cuisine,
            description: def.Description,
            servings: def.Servings,
            prepMinutes: def.PrepMinutes,
            cookMinutes: def.CookMinutes,
            difficulty: def.Difficulty,
            authorId: authorId,
            authorUsername: authorUsername,
            ingredients: ingredients
        );

        recipe.SetInstructions(def.Instructions);
        recipe.ImageUrl = RecipeSeedImages.TryGet(def.Name);
        return recipe;
    }
}