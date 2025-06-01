using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace WorkshopManager.PerformanceTests;

public class OrdersLoadTest
{
    
    private static readonly HttpClient HttpClient = new();

    public static void Run()
    {
        var scenario = Scenario.Create("get_all_orders_scenario", async context =>
        {
            // Assuming the API is running on http://localhost:5000 or https://localhost:5001
            // Adjust the base URL as needed.
            // Prefer HTTPS if available and configured.
            var request = Http.CreateRequest("GET", "http://localhost:5000/api/orders"); 
                                                                                      
            var response = await Http.Send(HttpClient, request);
            
            var fsharpOption = response.Payload;
            bool optionIsSome = fsharpOption.IsSome();
            bool successStatusCode = false;
            if (optionIsSome)
            {
                successStatusCode = fsharpOption.Value.IsSuccessStatusCode;
            }
            
            return optionIsSome && successStatusCode 
                ? Response.Ok() 
                : Response.Fail();
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(rate: 50, // 50 requests per second
                              interval: TimeSpan.FromSeconds(1),
                              during: TimeSpan.FromSeconds(2)) // Run for a duration that allows 100 requests (50*2=100)
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
    
} 