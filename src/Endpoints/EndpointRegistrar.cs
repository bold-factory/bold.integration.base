namespace Bold.Integration.Base.Endpoints;

public static class EndpointRegistrar
{
    public static WebApplication MapAllEndpoints(this WebApplication app)
    {
        var endpointTypes = typeof(Program).Assembly.GetTypes()
                                           .Where(t => typeof(IEndpoint).IsAssignableFrom(t)
                                                       && t is { IsInterface: false, IsAbstract: false });

        foreach (var endpointType in endpointTypes)
        {
            var endpointInstance = (IEndpoint)Activator.CreateInstance(endpointType)!;
            endpointInstance.Register(app);
        }
        return app;
    }
}