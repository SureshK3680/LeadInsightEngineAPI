using LeadInsightEngineAPI.Models;
using LeadInsightEngineAPI.Repository;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace LeadInsightEngineAPI.Services
{
    public interface ILeadQueuePublisher
    {
        bool RunUnprocessedLeads(Guid LeadId, IModel model);
    }
    public class LeadQueuePublisher : ILeadQueuePublisher
    {
        private IEntitiesRepository _entitiesRepository = null;
        private IAllocationrepository _entitiesallocationRepository = null;
        private IEntitiesBatchRepository _entitiesbatchRepository = null;
        public LeadQueuePublisher(
            IEntitiesRepository entitiesRepository, IAllocationrepository allocationrepository, IEntitiesBatchRepository entitiesBatchRepository)
        {
            _entitiesRepository = entitiesRepository;
            _entitiesallocationRepository = allocationrepository;
            _entitiesbatchRepository = entitiesBatchRepository;
        }

        public bool RunUnprocessedLeads(Guid LeadId, IModel model)
        {
            var records = new List<BatchRecord>();
            // Any leads newer than this will be given the benefit of the doubt. They could still be processing.
            var unprocessedThreshold = DateTime.UtcNow.AddMinutes(-30);

            // We're going to give up on any leads older than this.
            var lookback = TimeSpan.FromHours(24);
            var lookbackDate = unprocessedThreshold.Subtract(lookback);

            var currentMessages = model.MessageCount("rule-processing");

            var emptyLeads = _entitiesRepository.RetrieveById(LeadId);

            Console.Out.WriteLine($"[WARNING] leads queue is currently processing {currentMessages} leads");
            if (currentMessages > 0)
            {
                Console.Out.WriteLine("pending updates will not be processed while leads are queued for processing.");
                return false;
            }

            if (emptyLeads != null)
            {
                _entitiesallocationRepository.FindOneAndUpdate(emptyLeads.ContractId);

                Dictionary<string, FieldValue> latestFields = null;

                if (emptyLeads.Fields != null && emptyLeads.Fields.Any())
                {
                    try
                    {
                        latestFields = emptyLeads.Fields.ToDictionary(field => field.Key,
                            field => field.FieldValueHistory.LastOrDefault());
                    }
                    catch (ArgumentException e)
                    {
                        Console.Out.WriteLine(
                            $"Entity {emptyLeads.Id} on batch {emptyLeads.AllocationKey.Key} has duplicate fields. Giving up on building FieldValues.");
                        Console.Out.WriteLine($"{e.Message}: {e.StackTrace}");
                    }

                    var batch = new BatchRecord
                    {
                        BatchId = emptyLeads.BatchId,
                        ContractId = emptyLeads.ContractId,
                        EntityId = emptyLeads.Id,
                        OwnerId = Guid.Empty,
                        Timestamp = DateTime.UtcNow,
                        AllocationKey = emptyLeads.AllocationKey == null ? string.Empty : emptyLeads.AllocationKey.Key,
                        FieldValues = latestFields
                    };
                    Console.WriteLine($@"adding entity {batch.EntityId}");

                    _entitiesbatchRepository.Insert(batch);
                    Console.WriteLine($@"inserting {records.Count} batch records");
                }

                SendQueueMessage(model, emptyLeads);
            }

            Console.Out.WriteLine();
            return true;
        }

        private static void SendQueueMessage(IModel model, EntityRecord batchGroups)
        {
            var msg = new
            {
                Data = new QueueMessage
                {
                    BatchId = batchGroups.BatchId,
                    ContractId = batchGroups.ContractId,
                    OwnerId = Guid.Empty
                },
                Retry = 0
            };

            Console.WriteLine(@"checking existing batch messages");
            var batchCount = model.MessageCount("batch-processing");
            if (batchCount == 0)
            {
                Console.WriteLine(@"publishing missing messages");

                var output = JsonConvert.SerializeObject(msg, Formatting.Indented);
                model.BasicPublish("batch-processing", string.Empty, null, Encoding.UTF8.GetBytes(output));
            }
        }
    }
}
