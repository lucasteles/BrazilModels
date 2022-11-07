using BrazilModels.Tests.Utils;
using FsCheck;

namespace BrazilModels.Tests;

[SetUpFixture]
public class GlobalFixture
{
    [OneTimeSetUp]
    public void Setup()
    {
        Randomizer.Seed = new(42);
        Arb.Register<CustomGenerators>();
    }
}
