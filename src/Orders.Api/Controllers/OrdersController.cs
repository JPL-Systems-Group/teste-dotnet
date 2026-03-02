using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.DTOs;
using Orders.Application.Services;

namespace Orders.Api.Controllers;

[ApiController]
[Route("orders")]
public sealed class OrdersController(IOrderService orderService, Meter meter) : ControllerBase
{
    private readonly Counter<long> _createdOrdersCounter = meter.CreateCounter<long>("orders_created_total");

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var response = await orderService.CreateOrderAsync(request, cancellationToken);
        _createdOrdersCounter.Add(1);

        return Created($"/orders/{response.OrderId}", response);
    }
}
