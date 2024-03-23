using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;

namespace Zaza.Web.Tests.Integration;

[TestFixture]
internal class Auth {
    [Test]
    public async Task AuthTest() {
        var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(_ => { });
        var client = factory.CreateClient();

        using var content = new StringContent(
                JsonSerializer.Serialize(new {
                    username = "test",
                    password = "test"
                }));
        var response = await client.PostAsync("/reg", content);
        Assert.IsTrue(201 == (int)response.StatusCode);

    }
}
