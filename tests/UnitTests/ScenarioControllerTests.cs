namespace CySim.Tests.UnitTests;
using CySim.Controllers;
using CySim.Data;
using CySim.Models.Scenario;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class ScenarioControllerTests
{
    [Fact]
    public void IndexGet_ListsScenarios() 
    { 
        // Set up logger and mock db to create controller  
        var logger = NullLogger<ScenarioController>.Instance;
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("CySim");
        var context = new ApplicationDbContext(optionsBuilder.Options);
        context.Database.EnsureCreated();
        var controller = new ScenarioController(logger, context); 

        // Query Index 
        var result = controller.Index();
        
        // Assert from results
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Scenario>>(viewResult.ViewData.Model);
        
        context.Database.EnsureDeleted();
        
        // Could do more tests with model if it was not using mock DbSet
    }
}
