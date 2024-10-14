using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace CountryService;

public class Function
{
    
    /// <summary>
    /// Function that takes a string and returns Countries with names that contain it.
    /// </summary>
    /// <param name="input">The event for the Lambda function handler to process.</param>
    /// <param name="context">The ILambdaContext that provides methods for logging and describing the Lambda environment.</param>
    /// <returns>
    /// </returns>
    public async Task<APIGatewayProxyResponse> FindCountriesByName(APIGatewayProxyRequest input, ILambdaContext context)
    {
        // Your API calling logic here
        return new APIGatewayProxyResponse
        {
            StatusCode = 200,
            Body = "Lambda function executed"
        };
    }
}
