using Microsoft.Xrm.Sdk;
using System;

namespace Pavan___Task_2
{
    public class PostOperationFollowupPhonecallActivityCreatedFromContactCreation : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                var entity = (Entity)context.InputParameters["Target"];

                Entity followup = new Entity("phonecall");

                followup["subject"] = "Follow up phonecall for a new customer.";
                followup["description"] ="Follow up with the customer.";
                followup["scheduledend"] = DateTime.Now.AddDays(2);
                followup["category"] = context.PrimaryEntityName;

                if (context.OutputParameters.Contains("id"))
                {
                    Guid regardingobjectid = new Guid(context.OutputParameters["id"].ToString());
                    string regardingobjectidType = "contact";

                    followup["regardingobjectid"] = new EntityReference(regardingobjectidType, regardingobjectid);
                }

                var activityPartyTo = new Entity("activityparty");
                activityPartyTo["partyid"] = new EntityReference("systemuser", context.UserId);
                followup["from"] = new[] {activityPartyTo};

                var activityPartyFrom = new Entity("activityparty");
                activityPartyFrom["partyid"] = new EntityReference("contact", entity.Id);
                followup["to"] = new[] {activityPartyFrom};

                tracingService.Trace("FollowupPlugin: Creating the task activity.");
                service.Create(followup);
            }
        }
    }
}
