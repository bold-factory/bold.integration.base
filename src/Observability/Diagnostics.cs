using System.Diagnostics;
using System.Reflection;

namespace Bold.Integration.Base.Observability;

public static class Diagnostics
{
    public const string SourceName = "Bold.Integration";
    private static ActivitySource ActivitySource { get; } = new(SourceName);
    public static Activity? StartActivity(string name)
    {
        var activity = ActivitySource.StartActivity(name);
        return activity;
    }
    public static Activity AddEvent(this Activity activity, string name, object? obj)
    {
        var info = new Dictionary<string, object?>();
        if (obj != null)
        {
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                var propType = property.PropertyType;
                if (propType.IsPrimitive || propType == typeof(string)
                                         || propType == typeof(decimal)
                                         || propType == typeof(DateTimeOffset)
                                         || propType == typeof(Guid))
                {
                    info[property.Name] = property.GetValue(obj);
                }
            }
        }
        var ev = new ActivityEvent(name: name, timestamp: DateTimeOffset.UtcNow, tags: new ActivityTagsCollection(info));
        activity.AddEvent(ev);
        return activity;
    }
    public static Activity AddTags(this Activity activity, object? obj)
    {
        if (obj is null) return activity;
        var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var property in properties)
        {
            var propType = property.PropertyType;
            if (!propType.IsPrimitive && propType != typeof(string)
                                      && propType != typeof(decimal)
                                      && propType != typeof(bool)
                                      && propType != typeof(DateTimeOffset)
                                      && propType != typeof(Guid)) continue;
            var value = property.GetValue(obj);
            if (value != null)
            {
                activity.AddTag(key: property.Name, value: value.ToString());
            }
        }
        return activity;
    }
}