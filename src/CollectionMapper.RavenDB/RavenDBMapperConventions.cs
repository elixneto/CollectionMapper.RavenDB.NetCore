using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using CollectionMapper.RavenDB.Attributes;
using Raven.Client.Documents.Conventions;

namespace CollectionMapper.RavenDB;

public static class RavenDBMapperConventions
{
    private static bool _isDebugModeEnabled;
    private static readonly ConcurrentDictionary<Type, string> _collectionsRegistry = new();

    public static void EnableDebugMode()
    {
        _isDebugModeEnabled = true;   
        LogDebug("--- DEBUG MODE ENABLED ---");
    }

    /// <summary>Registers all types in the specified mapper.</summary>
    public static void RegisterMapper(RavenDBCollectionMapper mapper)
    {
        foreach (var definition in mapper.MappedCollections)
        {
            Register(definition.CollectionType, definition.CollectionName, nameof(RavenDBCollectionMapper));
        }
    }
    

    /// <summary>Scans the assembly that contains <typeparamref name="T"/> then registers all decorators and mappers found.</summary>
    public static void RegisterByAssembly<T>()
    {
        var assembly = typeof(T).Assembly;
        RegisterByAssemblies(assembly);
    }
    
    /// <summary>Scans the specified assemblies then registers all decorators and mappers found.</summary>
    public static void RegisterByAssemblies(params Assembly[] assemblies)
    {
        foreach (var assembly in assemblies)
        {
            ScanAndRegistryAssembly(assembly);
        }
    }

    /// <summary> Assign to <c>DocumentStore.Conventions.FindCollectionName</c>.</summary>
    public static string FindCollection(Type type)
    {
        if (_collectionsRegistry.TryGetValue(type, out var name))
        {
            LogDebug($"::FindCollection:: {type.FullName} → \"{name}\"");
            return name;
        }

        var fallback = DocumentConventions.DefaultGetCollectionName(type);
        LogDebug($"::FindCollection:: {type.FullName} → \"{fallback}\" (fallback)");
        return fallback;
    }


    private static void ScanAndRegistryAssembly(Assembly assembly)
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t is not null).ToArray()!;
        }
        
        // Step 1: Mappers
        foreach (var type in types)
        {
            var isMapper = !type.IsGenericType && type.IsAssignableTo(typeof(RavenDBCollectionMapper));
            if (!isMapper)
            {
                continue;
            }

            if (Activator.CreateInstance(type) is RavenDBCollectionMapper instance)
            {
                foreach (var collection in instance.MappedCollections)
                {
                    Register(collection.CollectionType, collection.CollectionName, $"{type.Name} (assembly scanning)");
                }
            }
        }

        // Step 2: [RavenCollection] — must run first so base types are in _registry for step 3
        foreach (var type in types)
        {
            var attr = type.GetCustomAttribute<RavenCollectionAttribute>();
            if (attr is null) continue;
            Register(type, attr.CollectionName, $"[RavenCollection]");
        }

        // Step 3: [RavenCollectionAssignedFrom<T>]
        foreach (var type in types)
        {
            var attr = type.GetCustomAttributes()
                .FirstOrDefault(a => a.GetType().IsGenericType &&
                                     a.GetType().GetGenericTypeDefinition() == typeof(RavenCollectionAssignedFromAttribute<>));
            if (attr is null) continue;

            var baseType = attr.GetType().GetGenericArguments()[0];
            var collectionName = ResolveCollectionName(baseType);
            Register(type, collectionName, $"[RavenCollectionAssignedFrom<{baseType.Name}>]");
        }
    }

    private static string ResolveCollectionName(Type type)
    {
        if (_collectionsRegistry.TryGetValue(type, out var name))
        {
            return name;
        }

        var attr = type.GetCustomAttribute<RavenCollectionAttribute>();
        if (attr is not null)
        {
            return attr.CollectionName;
        }

        return DocumentConventions.DefaultGetCollectionName(type);
    }

    private static void Register(Type type, string collectionName, string source)
    {
        var isRegistered = _collectionsRegistry.ContainsKey(type);
        if (!isRegistered)
        {
            _collectionsRegistry[type] = collectionName;
            LogDebug($"::Register:: {type.FullName} → \"{collectionName}\" | source: {source}");
        }
        
        var isOverridden = isRegistered && _collectionsRegistry[type] != collectionName;
        if (!isOverridden)
        {
            return;
        }
        
        _collectionsRegistry[type] = collectionName;
        LogDebug($"::Register:: (OVERRIDEN) {type.FullName} → \"{collectionName}\" | source: {source}");
    }

    private static void LogDebug(string message)
    {
        if (!_isDebugModeEnabled)
        {
            return;
        }
        
        var patternMessage = $"[CollectionMapper.RavenDB] {message}";
        
        Console.WriteLine(patternMessage);
        Trace.WriteLine(patternMessage);
    }
}
