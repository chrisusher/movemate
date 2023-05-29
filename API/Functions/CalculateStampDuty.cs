using System.Net;
using ChrisUsher.MoveMate.API.Services.StampDuty;
using ChrisUsher.MoveMate.Shared.DTOs.StampDuty;
using Microsoft.OpenApi.Models;

namespace ChrisUsher.MoveMate.API.Functions
{
    public class CalculateStampDuty
    {
        private readonly ILogger _logger;
        private readonly StampDutyService _stampDutyService;

        public CalculateStampDuty(
            ILoggerFactory loggerFactory,
            StampDutyService stampDutyService)
        {
            _logger = loggerFactory.CreateLogger<CalculateStampDuty>();
            _stampDutyService = stampDutyService;
        }

        [OpenApiOperation(operationId: "CalculateStampDuty", tags: new[] { "Property Calculations" }, Summary = "Calculates the Stamp Duty for a Property")]
        [OpenApiRequestBody("application/json", typeof(StampDutyRequest))]
        [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(StampDutyResponse))]
        [OpenApiParameter(name: "propertyId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid))]
        [Function("CalculateStampDuty")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Properties/{propertyId}/Calculations/StampDuty")] HttpRequestData request,
            int propertyId)
        {
            HttpResponseData response;

            try
            {
                var requestContent = await request.ReadAsStringAsync();
                var requestBody = JsonSerializer.Deserialize<StampDutyRequest>(requestContent);

                response = request.CreateResponse(HttpStatusCode.OK);
                var stampDutyBody = _stampDutyService.CalculateStampDuty(requestBody);

                await response.WriteAsJsonAsync(stampDutyBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(Convert.ToInt32(DateTime.UtcNow.ToString("HHmmss"))), ex, ex.Message);
                throw;
            }

            return response;
        }
    }
}
