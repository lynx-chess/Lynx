using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Lynx.Test;

[Explicit]
[Category(Categories.Configuration)]
[NonParallelizable]
public class ConfigurationTest
{
    [Test]
    public void SynchronizedAppSettings()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        var engineSettingsSection = config.GetRequiredSection(nameof(EngineSettings));
        Assert.IsNotNull(engineSettingsSection);

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        var serializedEngineSettingsConfig = JsonSerializer.Serialize(Configuration.EngineSettings);
        var jsonNode = JsonSerializer.Deserialize<JsonNode>(serializedEngineSettingsConfig);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code
        Assert.IsNotNull(jsonNode);

#pragma warning disable IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code - using sourcegenerator
        engineSettingsSection.Bind(Configuration.EngineSettings);
#pragma warning restore IL2026 // Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code

        var reflectionProperties = Configuration.EngineSettings.GetType().GetProperties(
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        Assert.GreaterOrEqual(reflectionProperties.Length, 35);

        var originalCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        Assert.Multiple(() =>
        {
            foreach (var property in reflectionProperties)
            {
                if (property.PropertyType == typeof(int[]))
                {
                    continue;
                }

                var sourceSetting = jsonNode![property.Name]!.ToString().ToLowerInvariant();
                var configSetting = property.GetValue(Configuration.EngineSettings)!.ToString()!.ToLowerInvariant();

                Assert.AreEqual(sourceSetting, configSetting, $"Error in {property.Name} ({property.PropertyType}): (Configuration.cs) {sourceSetting} != {configSetting} (appSettings.json)");
            }

            Thread.CurrentThread.CurrentCulture = originalCulture;
        });
    }
}
