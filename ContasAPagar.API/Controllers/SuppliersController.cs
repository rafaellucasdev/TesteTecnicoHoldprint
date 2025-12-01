using ContasAPagar.API.DTOs.Supplier;
using ContasAPagar.API.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace ContasAPagar.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    private readonly IValidator<CreateSupplierRequest> _validator;

    public SuppliersController(
        ISupplierService supplierService,
        IValidator<CreateSupplierRequest> validator)
    {
        _supplierService = supplierService;
        _validator = validator;
    }

    /// <summary>
    /// Cria um novo fornecedor
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SupplierResponse>> Create([FromBody] CreateSupplierRequest request)
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

        var result = await _supplierService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Obtém um fornecedor por ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SupplierResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SupplierResponse>> GetById(string id)
    {
        var result = await _supplierService.GetByIdAsync(id);
        if (result == null)
        {
            return NotFound(new { message = $"Fornecedor com ID {id} não foi encontrado" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Lista todos os fornecedores
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<SupplierResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SupplierResponse>>> GetAll()
    {
        var result = await _supplierService.GetAllAsync();
        return Ok(result);
    }
}

