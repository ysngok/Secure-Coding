using Microsoft.AspNetCore.Mvc;
using ProductService.Models;
using ProductService.Services;
using Shared.Models;

namespace ProductService.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly ProductManagementService _productService;

    public ProductController(ProductManagementService productService)
    {
        _productService = productService;
    }

    /// <summary>
    /// List all products, optionally filtered by category.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetProducts([FromQuery] string? category)
    {
        try
        {
            var products = await _productService.GetProductsAsync(category);
            return Ok(ApiResponse<List<Product>>.Ok(products));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Failed to get products", ex));
        }
    }

    /// <summary>
    /// Search products by name or description.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string q)
    {
        try
        {
            if (string.IsNullOrEmpty(q))
                return BadRequest(ApiResponse.Fail("Search query is required"));

            var resultsJson = await _productService.SearchProductsAsync(q);
            var parsedNode = System.Text.Json.Nodes.JsonNode.Parse(resultsJson);
            return Ok(ApiResponse<System.Text.Json.Nodes.JsonNode>.Ok(parsedNode));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Search failed", ex));
        }
    }

    /// <summary>
    /// Create a new product.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] Product product)
    {
        try
        {
            var result = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetProducts), new { id = result.Id }, ApiResponse<Product>.Ok(result, "Product created"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Failed to create product", ex));
        }
    }

    /// <summary>
    /// Update an existing product.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
    {
        try
        {
            var updated = await _productService.UpdateProductAsync(id, product);
            if (!updated)
                return NotFound(ApiResponse.Fail("Product not found"));

            return Ok(ApiResponse.Ok("Product updated"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Failed to update product", ex));
        }
    }

    /// <summary>
    /// Delete a product.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(string id)
    {
        try
        {
            var deleted = await _productService.DeleteProductAsync(id);
            if (!deleted)
                return NotFound(ApiResponse.Fail("Product not found"));

            return Ok(ApiResponse.Ok("Product deleted"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Failed to delete product", ex));
        }
    }

    /// <summary>
    /// Get debug information and database stats.
    /// </summary>
    [HttpGet("debug")]
    public async Task<IActionResult> Debug()
    {
        try
        {
            var debugInfo = await _productService.GetDebugInfoAsync();
            return Ok(ApiResponse<object>.Ok(debugInfo));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Debug failed", ex));
        }
    }

    /// <summary>
    /// Export products to a JSON file.
    /// </summary>
    [HttpGet("export")]
    public async Task<IActionResult> ExportProducts([FromQuery] string filename)
    {
        try
        {
            if (string.IsNullOrEmpty(filename))
                return BadRequest(ApiResponse.Fail("Filename is required"));

            var result = await _productService.ExportProductsAsync(filename);
            return Ok(ApiResponse<string>.Ok(result, "Export completed"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Export failed", ex));
        }
    }

    /// <summary>
    /// Import products from an uploaded CSV file.
    /// </summary>
    [HttpPost("import-csv")]
    public IActionResult ImportCsv(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse.Fail("No file uploaded"));

            var tempPath = Path.Combine(Path.GetTempPath(), file.FileName);

            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            var result = _productService.ImportCsvProducts(tempPath);
            return Ok(ApiResponse<string>.Ok(result, "CSV processed"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Import failed", ex));
        }
    }

    /// <summary>
    /// Get product statistics grouped by a field.
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetProductStats([FromQuery] string groupBy)
    {
        try
        {
            if (string.IsNullOrEmpty(groupBy))
                return BadRequest(ApiResponse.Fail("groupBy field is required"));

            var statsJson = await _productService.GetProductStatsAsync(groupBy);
            var parsedNode = System.Text.Json.Nodes.JsonNode.Parse(statsJson);
            return Ok(ApiResponse<System.Text.Json.Nodes.JsonNode>.Ok(parsedNode));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse.Fail("Stats failed", ex));
        }
    }
}
