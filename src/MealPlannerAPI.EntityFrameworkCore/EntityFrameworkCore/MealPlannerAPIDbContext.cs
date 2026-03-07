using MealPlannerAPI.Enums;
using MealPlannerAPI.MealPlans;
using MealPlannerAPI.Notifications;
using MealPlannerAPI.Recipes;
using MealPlannerAPI.ShoppingLists;
using MealPlannerAPI.Users;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.BlobStoring.Database.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace MealPlannerAPI.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class MealPlannerAPIDbContext :
    AbpDbContext<MealPlannerAPIDbContext>,
    ITenantManagementDbContext,
    IIdentityDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */


    #region Entities from the modules

    // Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }
    public DbSet<IdentityUserDelegation> UserDelegations { get; set; }
    public DbSet<IdentitySession> Sessions { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion


    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<ShoppingList> ShoppingLists { get; set; }
    public DbSet<ShoppingListItem> ShoppingListItems { get; set; }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public MealPlannerAPIDbContext(DbContextOptions<MealPlannerAPIDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureFeatureManagement();
        builder.ConfigureIdentity();
        builder.ConfigureOpenIddict();
        builder.ConfigureTenantManagement();
        builder.ConfigureBlobStoring();

        /* Configure your own tables/entities inside here */

        builder.Entity<Recipe>(b =>
        {
            b.ToTable("AppRecipes");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(true);

            b.Property(x => x.Cuisine)
                .IsRequired()
                .HasMaxLength(128);

            b.Property(x => x.Difficulty)
                .HasConversion<string>()
                .HasMaxLength(32);

            b.Property(x => x.CookingTimeMinutes)
                .HasDefaultValue(0);

            b.Property(x => x.PrepTimeMinutes)
                .HasDefaultValue(0);

            b.Property(x => x.Servings)
                .HasDefaultValue(4);

            b.Property(x => x.Rating)
                .HasColumnType("decimal(3,2)")
                .HasDefaultValue(0.0);

            b.Property(x => x.ReviewCount)
                .HasDefaultValue(0);

            b.Property(x => x.ImageUrl)
                .HasMaxLength(512);

            b.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(2000)
                .IsUnicode(true);

            b.Property(x => x.Tags)
                .HasMaxLength(1024);

            b.Property(x => x.InstructionsJson)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            b.Property(x => x.AuthorId)
                .IsRequired();

            b.HasIndex(x => x.AuthorId);
            b.HasIndex(x => x.Cuisine);
            b.HasIndex(x => x.Rating);

            b.HasMany(x => x.Ingredients)
                .WithOne()
                .HasForeignKey(x => x.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── RecipeIngredient ──────────────────────────────────────────────────
        builder.Entity<RecipeIngredient>(b =>
        {
            b.ToTable("AppRecipeIngredients");
            b.ConfigureByConvention();

            b.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(128)
                .IsUnicode(true);

            b.Property(x => x.Quantity)
                .HasColumnType("decimal(10,3)");

            b.Property(x => x.Unit)
                .IsRequired()
                .HasMaxLength(32);

            b.HasIndex(x => x.RecipeId);
        });

        // ── MealPlan ──────────────────────────────────────────────────────────
        builder.Entity<MealPlan>(b =>
        {
            b.ToTable("AppMealPlans");
            b.ConfigureByConvention();

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.WeekStartDate)
                .IsRequired()
                .HasColumnType("date");

            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.UserId, x.WeekStartDate }).IsUnique();

            b.HasMany(x => x.Entries)
                .WithOne()
                .HasForeignKey(x => x.MealPlanId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── MealPlanEntry ─────────────────────────────────────────────────────
        builder.Entity<MealPlanEntry>(b =>
        {
            b.ToTable("AppMealPlanEntries");
            b.ConfigureByConvention();

            b.Property(x => x.DayOfWeek)
                .IsRequired()
                .HasMaxLength(16);

            b.Property(x => x.MealName)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(true);

            b.Property(x => x.MealType)
                .HasConversion<string>()
                .HasMaxLength(32);

            b.Property(x => x.ScheduledTime)
                .HasMaxLength(8);

            b.Property(x => x.RecipeId)
                .IsRequired(false);

            b.HasIndex(x => x.MealPlanId);
            b.HasIndex(x => x.RecipeId);
        });

        // ── ShoppingList ──────────────────────────────────────────────────────
        builder.Entity<ShoppingList>(b =>
        {
            b.ToTable("AppShoppingLists");
            b.ConfigureByConvention();

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.Name)
                .HasMaxLength(256)
                .IsUnicode(true);

            b.HasIndex(x => x.UserId);

            b.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey(x => x.ShoppingListId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ── ShoppingItem ──────────────────────────────────────────────────────
        builder.Entity<ShoppingListItem>(b =>
        {
            b.ToTable("AppShoppingItems");
            b.ConfigureByConvention();

            b.Property(x => x.ShoppingListName)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(true);

            b.Property(x => x.Quantity)
                .HasColumnType("decimal(10,3)");

            b.Property(x => x.Unit)
                .IsRequired()
                .HasMaxLength(32);

            b.Property(x => x.Category)
                .HasConversion<string>()
                .HasMaxLength(32);

            b.Property(x => x.IsCompleted)
                .HasDefaultValue(false);

            b.HasIndex(x => x.ShoppingListId);
            b.HasIndex(x => x.Category);
        });

        // ── UserNotification ──────────────────────────────────────────────────
        builder.Entity<UserNotification>(b =>
        {
            b.ToTable("AppUserNotifications");
            b.ConfigureByConvention();

            b.Property(x => x.UserId)
                .IsRequired();

            b.Property(x => x.Type)
                .HasConversion<string>()
                .HasMaxLength(64);

            b.Property(x => x.Title)
                .IsRequired()
                .HasMaxLength(256)
                .IsUnicode(true);

            b.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(1000)
                .IsUnicode(true);

            b.Property(x => x.IsRead)
                .HasDefaultValue(false);

            b.Property(x => x.AvatarUrl)
                .HasMaxLength(512);

            b.HasIndex(x => x.UserId);
            b.HasIndex(x => new { x.UserId, x.IsRead });  // common query pattern
        });

        builder.Entity<UserProfile>(b =>
        {
            b.ToTable(AbpIdentityDbProperties.DbTablePrefix + "Users");
            b.ConfigureByConvention();
            b.Property(x => x.AvatarUrl)
            .HasMaxLength(512)
            .IsUnicode(true)
            .HasColumnName(nameof(UserProfile.AvatarUrl));

            // ── Preferences ───────────────────────────────────────────
            b.Property(x => x.DietaryRestrictions)
                .HasMaxLength(512)
                .IsUnicode(true)
                .HasColumnName(nameof(UserProfile.DietaryRestrictions));

            b.Property(x => x.CuisinePreferences)
                .HasMaxLength(512)
                .IsUnicode(true)
                .HasColumnName(nameof(UserProfile.CuisinePreferences));

            b.Property(x => x.DefaultServingSize)
                .HasDefaultValue(4)
                .HasColumnName(nameof(UserProfile.DefaultServingSize));

            b.Property(x => x.WeeklyBudget)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0m)
                .HasColumnName(nameof(UserProfile.WeeklyBudget));

            b.Property(x => x.MealPlanningDays)
                .HasDefaultValue(7)
                .HasColumnName(nameof(UserProfile.MealPlanningDays));

            b.Property(x => x.Specialty)
                .HasMaxLength(256)
                .IsUnicode(true)
                .HasColumnName(nameof(UserProfile.Specialty));

            // ── Stats ─────────────────────────────────────────────────
            b.Property(x => x.RecipesCreated)
                .HasDefaultValue(0)
                .HasColumnName(nameof(UserProfile.RecipesCreated));

            b.Property(x => x.RecipesLiked)
                .HasDefaultValue(0)
                .HasColumnName(nameof(UserProfile.RecipesLiked));

            b.Property(x => x.MealsPlanned)
                .HasDefaultValue(0)
                .HasColumnName(nameof(UserProfile.MealsPlanned));

            b.Property(x => x.ShoppingListsGenerated)
                .HasDefaultValue(0)
                .HasColumnName(nameof(UserProfile.ShoppingListsGenerated));

            b.Property(x => x.Followers)
                .HasDefaultValue(0)
                .HasColumnName(nameof(UserProfile.Followers));

            b.Property(x => x.Following)
                .HasDefaultValue(0)
                .HasColumnName(nameof(UserProfile.Following));

            // ── Privacy settings ──────────────────────────────────────
            b.Property(x => x.ProfileVisibility)
                .HasDefaultValue(VisibilityLevel.Public)
                .HasConversion<string>()         // store as "Public" / "Private" etc.
                .HasMaxLength(32)
                .HasColumnName(nameof(UserProfile.ProfileVisibility));

            b.Property(x => x.RecipesVisibility)
                .HasDefaultValue(VisibilityLevel.Public)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasColumnName(nameof(UserProfile.RecipesVisibility));

            b.Property(x => x.ShoppingListVisibility)
                .HasDefaultValue(VisibilityLevel.Private)
                .HasConversion<string>()
                .HasMaxLength(32)
                .HasColumnName(nameof(UserProfile.ShoppingListVisibility));

            // ── Notification preferences ──────────────────────────────
            b.Property(x => x.NotifyMealReminders)
                .HasDefaultValue(true)
                .HasColumnName(nameof(UserProfile.NotifyMealReminders));

            b.Property(x => x.NotifyRecipeUpdates)
                .HasDefaultValue(true)
                .HasColumnName(nameof(UserProfile.NotifyRecipeUpdates));

            b.Property(x => x.NotifyCommunityActivity)
                .HasDefaultValue(false)
                .HasColumnName(nameof(UserProfile.NotifyCommunityActivity));

            b.Property(x => x.NotifyShoppingListAlerts)
                .HasDefaultValue(true)
                .HasColumnName(nameof(UserProfile.NotifyShoppingListAlerts));
        });
    }
}
