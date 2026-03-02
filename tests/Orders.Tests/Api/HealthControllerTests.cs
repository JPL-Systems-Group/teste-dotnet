using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Orders.Api.Controllers;

namespace Orders.Tests.Api;

public sealed class HealthControllerTests
{
    [Fact]
    public void GetLiveness_ShouldReturnOk()
    {
        var controller = new HealthController();

        var result = controller.GetLiveness();

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(200);
    }
}
