using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Lynx.Test;

/// <summary>
/// Current logic relies on this
/// </summary>
[Explicit]
[Category(Categories.Configuration)]
[NonParallelizable]
public class ConfigurationValuesTest
{
    [Test]
    public void RazoringValues()
    {
        Assert.Greater(Configuration.EngineSettings.RFP_MaxDepth, Configuration.EngineSettings.Razoring_MaxDepth);

        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .Build();

        var engineSettingsSection = config.GetRequiredSection(nameof(EngineSettings));
        Assert.IsNotNull(engineSettingsSection);
        engineSettingsSection.Bind(Configuration.EngineSettings);

        Assert.Greater(Configuration.EngineSettings.Razoring_MaxDepth, Configuration.EngineSettings.RFP_MaxDepth);
    }
}
