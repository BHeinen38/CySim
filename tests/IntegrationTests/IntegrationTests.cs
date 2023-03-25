namespace CySim.Tests.IntegrationTests;
using CySim;
using CySim.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;


public class IndexPageTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program>
        _factory;

    public IndexPageTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();   
    }
    
    private void ensureDatabaseCreated() 
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureCreated();
        }
    }
 
    private void ensureDatabaseDeleted() 
    {
        using (var scope = _factory.Services.CreateScope())
        {
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();

            db.Database.EnsureDeleted();
        }
    }


    /* 
     * This just goes through the list of controller endpoints to ensure reachability.
     * If authentication was set up correctly, we would error on some endpoints we lack access to.
     * We actually need authentication integration tests to ensure correct access.
     * This would use preset accounts in an in-memory sqlite db.
     */  
    
    [Theory]
    [InlineData("/")]
    [InlineData("/Scoreboard/Scoreboard")]
    [InlineData("/Scenario")]
    [InlineData("/Tutorial")]
    [InlineData("/TeamRegistration")]
    public async Task Get_Endpoints(string url)
    {
        ensureDatabaseCreated();

        // Connect to url
        var response = await _client.GetAsync(url);

        // Assert that connection passed (200-299 status code) and html was recieved
        response.EnsureSuccessStatusCode();

        var responseContentType = response.Content.Headers.ContentType;
        var typeString = responseContentType != null ? responseContentType.ToString() : "";
        
        Assert.Equal("text/html; charset=utf-8", typeString);
        
        ensureDatabaseDeleted();
    }

    /* 
     * Other tests could be done within this class.
     * Ideas: 
     *  - Ensure error page for non-existent endpoint
     *  - Ensure Correct redirects
     *  - Essentially test different requests 
     *  - Tests can NOT involve data (with this setup of test WebApplication)
     */
}

