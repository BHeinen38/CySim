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
    private (ApplicationDbContext, ScenarioController, List<Scenario>) InitializeTest()
    {
        // Set up logger and mock db to create controller  
        var logger = NullLogger<ScenarioController>.Instance;
        
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("CySim");
        
        var context = new ApplicationDbContext(optionsBuilder.Options);

        // Creates database tables and columns if they don't exist
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        var controller = new ScenarioController(logger, context); 
        
        // Initialize scenarios in db
        var scenarios = new List<Scenario>{
            new Scenario{Id = 1, FileName  = "FirstFile", FilePath = "Test/FirstFile", Description = "First Test Scenario", isRed = false}, 
            new Scenario{Id = 2, FileName  = "SecondFile", FilePath = "Test/SecondFile", Description = "Second Test Scenario", isRed = true}, 
            new Scenario{Id = 3, FileName  = "ThirdFile", FilePath = "Test/ThirdFile", Description = "Third Test Scenario", isRed = false} 
        };
        
        context.Scenarios.AddRange(scenarios);
        context.SaveChanges(); 

        return (context, controller, scenarios);
    }

       
    [Fact]
    public async Task IndexGet() 
    { 
        (_, var controller, _) = InitializeTest(); 

        // Query Index 
        var result = await controller.Index();
        
        // Confirm that a view is returned and has correct model type
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Scenario>>(viewResult.ViewData.Model);
    }

    [Fact]
    public void CreateGet() 
    { 
        (_, var controller, _) = InitializeTest(); 

        // Query Index 
        var result = controller.Create();
        
        // Confirm that a view is returned and has correct model type
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.ViewData.Model); // Create Get request has no model
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task EditGet(int id) 
    { 
        (_, var controller, var scenarios) = InitializeTest(); 

        // Query Index 
        var result = await controller.Edit(id);
        
        // Confirm that a view is returned and has correct model type
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Scenario>(viewResult.ViewData.Model);
   
        /// Confirm Model is the one queried by id number
        Assert.NotNull(model);    
        Assert.Equal(scenarios[id-1], model);
    }
}
