using ContasAPagar.API.DTOs.Common;
using ContasAPagar.API.DTOs.Payable;
using ContasAPagar.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ContasAPagar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayablesController : ControllerBase
{
    private readonly IPayableService _payableService;
    private readonly IValidator<CreatePayableRequest> _validator;

    public PayablesController(
        IPayableService payableService,
        IValidator<CreatePayableRequest> validator)
    {
        _payableService = payableService;
        _validator = validator;
    }

    /// <summary>
    /// Cria uma nova conta a pagar
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PayableResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PayableResponse>> Create([FromBody] CreatePayableRequest request)
    {
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(new
            {
                message = "Dados inválidos",
                errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
            });
        }

        var result = await _payableService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Obtém uma conta a pagar por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PayableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayableResponse>> GetById(string id)
    {
        var result = await _payableService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = $"Conta a pagar com ID {id} não foi encontrada" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Lista contas a pagar com filtros opcionais e paginação
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<PayableResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<PayableResponse>>> GetFiltered(
        [FromQuery] string? supplierId,
        [FromQuery] DateTime? startDueDate,
        [FromQuery] DateTime? endDueDate,
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var filter = new PayableFilterRequest
        {
            SupplierId = supplierId,
            StartDueDate = startDueDate,
            EndDueDate = endDueDate,
            Status = status,
            Page = page,
            PageSize = pageSize
        };

        var result = await _payableService.GetFilteredAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Marca uma conta a pagar como paga
    /// </summary>
    [HttpPatch("{id}/pay")]
    [ProducesResponseType(typeof(PayableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayableResponse>> MarkAsPaid(string id)
    {
        var result = await _payableService.MarkAsPaidAsync(id);
        return Ok(result);
    }

    /// <summary>
    /// Cancela uma conta a pagar
    /// </summary>
    [HttpPatch("{id}/cancel")]
    [ProducesResponseType(typeof(PayableResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayableResponse>> Cancel(string id)
    {
        var result = await _payableService.CancelAsync(id);
        return Ok(result);
    }
}

