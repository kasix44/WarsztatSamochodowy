using NBomber.Contracts;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace WorkshopManager.PerformanceTests;

public class OrdersLoadTest
{
    private static readonly HttpClient HttpClient = new();
    private const string BaseUrl = "http://localhost:5124"; // Changed to HTTPS and correct port

    public static void Run()
    {
        var scenario = Scenario.Create("get_all_orders_scenario", async context =>
        {
            try
            {
                var request = Http.CreateRequest("GET", $"{BaseUrl}/ServiceOrder");
                var response = await Http.Send(HttpClient, request);
                
                var fsharpOption = response.Payload;
                if (!fsharpOption.IsSome())
                {
                    return Response.Fail<string>("No response received", "No response", 0);
                }

                var httpResponse = fsharpOption.Value;
                if (!httpResponse.IsSuccessStatusCode)
                {
                    return Response.Fail<string>($"Request failed with status code: {httpResponse.StatusCode}", "HTTP error", 0);
                }

                return Response.Ok();
            }
            catch (Exception ex)
            {
                return Response.Fail<string>($"Request failed: {ex.Message}", "Exception", 0);
            }
        })
        .WithoutWarmUp()
        .WithLoadSimulations(
            Simulation.Inject(rate: 50,
                            interval: TimeSpan.FromSeconds(1),
                            during: TimeSpan.FromSeconds(2))
        );

        NBomberRunner
            .RegisterScenarios(scenario)
            .Run();
    }
} 