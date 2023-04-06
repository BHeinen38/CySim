namespace CySim.Tests.UnitTests;
using CySim.Controllers;
using CySim.Data;
using CySim.Models.Scenario;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;
using Moq;

public class ScenarioFixture : IDisposable
{
    public ApplicationDbContext context  { get; private set; }
    public List<Scenario> scenarios      { get; private set; }

    public ScenarioFixture()
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseInMemoryDatabase("CySim");
        
        context = new ApplicationDbContext(optionsBuilder.Options);

        // Creates database tables and columns if they don't exist
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        
        // Initialize scenarios in db
        scenarios = new List<Scenario>{
            new Scenario{Id = 1, FileName  = "FirstFile", FilePath = "Test/FirstFile", Description = "First Test Scenario", isRed = false}, 
            new Scenario{Id = 2, FileName  = "SecondFile", FilePath = "Test/SecondFile", Description = "Second Test Scenario", isRed = true}, 
            new Scenario{Id = 3, FileName  = "ThirdFile", FilePath = "Test/ThirdFile", Description = "Third Test Scenario", isRed = false} 
        };
        
        context.Scenarios.AddRange(scenarios);
        context.SaveChanges(); 
    }

    public void Dispose()
    {
        context.Dispose();
        scenarios.Clear();
    }
}


public static class ScenarioTestsUtils 
{
    public static ScenarioController CreateController(ApplicationDbContext context)
    {
        var logger = NullLogger<ScenarioController>.Instance;
        var mockFS = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { "wwwroot/Documents/Scenario/Fake.pdf", new MockFileData("Testing file Fake.pdf") },
            { "wwwroot/Documents/Scenario/Fake2.pdf", new MockFileData("Testing file Fake2.pdf") }
        });

        return new ScenarioController(logger, context, mockFS)
        {
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
    }

    private static Mock<IFormFile> GenerateMockFile(String fileName)
    {
        //Create mock file
        var file = new Mock<IFormFile>();
        file.Setup(f => f.FileName).Returns(fileName).Verifiable();
        file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), CancellationToken.None))
            .Returns(Task.CompletedTask);

        return file;
    }


    public static IEnumerable<object[]> GenerateValidCreatePostData()
    {
        yield return new object[] { GenerateMockFile("Fake.pdf"), "Fake Scenario 1 PDF", false };
        yield return new object[] { GenerateMockFile("Fake2.pdf"), "Fake Scenario 2 PDF", true };
    }

    public static IEnumerable<object[]> GenerateInvalidCreatePostData()
    {
        yield return new object[] { null, "Fake Scenario 1 PDF", false };
        yield return new object[] { GenerateMockFile("Fake.pdf"), null, true };
    }
}

[CollectionDefinition("GetCollection")]
public class GetCollection : ICollectionFixture<ScenarioFixture> { }


[CollectionDefinition("CreatePostCollection", DisableParallelization = true)]
public class CreatePostCollection : ICollectionFixture<ScenarioFixture> { }


[Collection("GetCollection")]
public class ScenarioGetTests
{
    private readonly ScenarioFixture _fixture;
    
    public ScenarioGetTests(ScenarioFixture fixture)
    {
        _fixture = fixture;
    }
    
    [Fact]
    public async Task IndexGet() 
    { 
        var controller = ScenarioTestsUtils.CreateController(_fixture.context);
        
        // Query Index 
        var result = await controller.Index();
        
        // Confirm that a view is returned and has correct model type
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<List<Scenario>>(viewResult.ViewData.Model);
        Assert.Equal(_fixture.scenarios.OrderBy(x => x.isRed), model); 
    }

    [Fact]
    public void CreateGet() 
    { 
        var controller = ScenarioTestsUtils.CreateController(_fixture.context);

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
        var controller = ScenarioTestsUtils.CreateController(_fixture.context);
        
        // Query Index 
        var result = await controller.Edit(id);
        
        // Confirm that a view is returned and has correct model type
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsAssignableFrom<Scenario>(viewResult.ViewData.Model);
   
        /// Confirm Model is the one queried by id number
        Assert.NotNull(model);    
        Assert.Equal(_fixture.scenarios[id-1], model);
    }

    
    [Theory]
    [InlineData(100)]
    public async Task EditGet_NonexistentID(int id) 
    { 
        var controller = ScenarioTestsUtils.CreateController(_fixture.context);

        // Query Index 
        var result = await controller.Edit(id);
        
        // Confirm that we redirect to index with invalid ID
        var RedirectResult = Assert.IsType<NotFoundResult>(result);
    }
}


[Collection("CreatePostCollection")]
public class ScenarioCreatePostTests
{
    private readonly ScenarioFixture _fixture;
    
    public ScenarioCreatePostTests(ScenarioFixture fixture)
    {
        _fixture = fixture;
    }
   
    [Theory]
    [MemberData(nameof(ScenarioTestsUtils.GenerateValidCreatePostData), MemberType = typeof(ScenarioTestsUtils))]
    public async Task CreatePost_ValidData(Mock<IFormFile> file, String Description, bool isRed) 
    { 
        var controller = ScenarioTestsUtils.CreateController(_fixture.context);

        // Query Index 
        var result = await controller.Create(file.Object, Description, isRed);
 
        // Confirm that we redirect to index with invalid ID
        var RedirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(RedirectResult.ControllerName);
        Assert.Equal("Index", RedirectResult.ActionName);
    }

    [Theory]
    [MemberData(nameof(ScenarioTestsUtils.GenerateInvalidCreatePostData), MemberType = typeof(ScenarioTestsUtils))]
    public async Task CreatePost_InvalidData(Mock<IFormFile>? file, String Description, bool isRed) 
    { 
        var controller = ScenarioTestsUtils.CreateController(_fixture.context);

        // Query Index 
        var result = await controller.Create(file?.Object ?? null, Description, isRed);
 
        // Confirm that we redirect to index with invalid ID
        var RedirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Null(RedirectResult.ControllerName);
        Assert.Equal("Create", RedirectResult.ActionName);
    }
}
