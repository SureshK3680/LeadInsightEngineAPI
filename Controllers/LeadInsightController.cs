using LeadInsightEngineAPI.Services;
using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;

namespace LeadInsightEngineUploadAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LeadInsightController : ControllerBase
    {
        private readonly ILogger<LeadInsightController> _logger;
        private readonly ILeadQueuePublisher _leadQueuePublisher;

        public LeadInsightController(ILogger<LeadInsightController> logger, ILeadQueuePublisher leadQueuePublisher)
        {
            _logger = logger;
            _leadQueuePublisher = leadQueuePublisher;
        }

        [HttpGet("UploadLead/")]
        public void ReuploadLead(Guid leadId)
        {
            try
            {
                Console.WriteLine(leadId);

                //Kickstart
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(LeadInsightEngineAPI.Models.Constants.AMQPCONNECTIONSTRING)
                };

                var connection = factory.CreateConnection();
                var model = connection.CreateModel();
                _leadQueuePublisher.RunUnprocessedLeads(leadId, model);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpGet("GetLeadJsonByLeadId/")]
        public string GetJsonByLeadId(Guid leadId)
        {
            //call the json class and return json as string
            return "Lead Json :" + leadId;
        }
    }
}