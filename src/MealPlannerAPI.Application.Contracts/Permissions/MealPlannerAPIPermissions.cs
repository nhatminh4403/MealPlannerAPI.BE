namespace MealPlannerAPI.Permissions;

public static class MealPlannerAPIPermissions
{
    public const string GroupName = "MealPlannerAPI";



    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";
    public static class Recipes
    {
        public const string Default = GroupName + ".Recipes";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class MealPlans
    {
        public const string Default = GroupName + ".MealPlans";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
    }

    public static class ShoppingLists
    {
        public const string Default = GroupName + ".ShoppingLists";
        public const string Create = Default + ".Create";
        public const string Update = Default + ".Update";
        public const string Delete = Default + ".Delete";
        public const string ManageItems = Default + ".ManageItems";
    }

    public static class Notifications
    {
        public const string Default = GroupName + ".Notifications";
        public const string Delete = Default + ".Delete";
    }

    public static class UserProfiles
    {
        public const string Default = GroupName + ".UserProfiles";
        public const string UpdateOthers = Default + ".UpdateOthers";
    }

    public static class Dashboard
    {
        public const string Default = GroupName + ".Dashboard";
    }
}
