using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Diagnostics;
using Aqueduct.Diagnostics;

namespace ManCity.Site.Classes.InternalTranslation
{
    public class InternalTranslationHelper
    {
        private readonly string defaultWorkflowId = Sitecore.Configuration.Settings.GetSetting("DefaultTranslationWorkFlowID");
        private readonly string cmdId = Sitecore.Configuration.Settings.GetSetting("DefaultTranslationWorkFlowCmd");
        private readonly string trustedCmdId = Sitecore.Configuration.Settings.GetSetting("TranslationTrustedCmd");
        private readonly string untrustedCmdId = Sitecore.Configuration.Settings.GetSetting("TranslationUntrustedCmd");
        private readonly string defaultConfigs = Sitecore.Configuration.Settings.GetSetting("ConfigDefaults");

        public bool areTranslatorsAvailableFor(string languageNodeId)
        {
            var flag = false;
            var languageNode = this.getItemFromMasterDb(languageNodeId);
            Sitecore.Data.Fields.MultilistField multilistField = languageNode.Fields["Translators"];
            if (multilistField.Count == 0)
                return flag;

            foreach (Item item in multilistField.GetItems())
            {
                if (String.IsNullOrEmpty(item.Fields["unavailable"].Value))
                {
                    flag = true;
                }
            }
            return flag;
        }

        public Item getItemFromMasterDb(dynamic id)
        {
            Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            Assert.IsNotNull(master, "No Master DB  found ");
            Item item = master.GetItem(id);
            Assert.IsNotNull(item, "requested item not found  in Master Database");
            return item;
        }




        public void process(IList<Sitecore.Data.ID>ids, IList<String> languageNodeIds, string readyByDate,
                            string additionalText)
        {
            foreach (var sourceId in ids)
            {
                foreach (var languageNodeId in languageNodeIds)
                {
                    try
                    {
                        createNewVerion(sourceId, languageNodeId);
                        email(sourceId, languageNodeId, additionalText, readyByDate);

                    }
                    catch (Exception ex)
                    {
                        AppLogger.LogError("Error Internal Translation process", ex);
                    }
                }
            }
        }



        private void createNewVerion(Sitecore.Data.ID sourceId, string languageNodeId)
        {
            var languageNode = this.getItemFromMasterDb(languageNodeId);
            var languageField = (Sitecore.Data.Fields.InternalLinkField) languageNode.Fields["Language"];
            var targetLang = Language.Parse(languageField.TargetItem.Name);
            Item targetVersion = Sitecore.Context.ContentDatabase.GetItem(sourceId, targetLang);
            Item newVersion = null;
            try
            {
                Sitecore.Data.Fields.MultilistField multilistField = languageNode.Fields["Translators"];
                if (multilistField.Count == 0)
                    return;
                var available = false;
                foreach (Item translator in multilistField.GetItems())
                {

                    if (String.IsNullOrEmpty(translator.Fields["unavailable"].Value))
                    {
                        available = true;
                    }
                }
                if (available)
                {
                    targetVersion.Editing.BeginEdit();
                    newVersion = targetVersion.Versions.AddVersion();
                    targetVersion.Editing.EndEdit();
                    this.manageWorkflow(newVersion, !String.IsNullOrEmpty(languageNode.Fields["trusted"].Value));
                }

            }
            catch (Exception exp)
            {
                throw new Exception("Create new Version failed", exp);
            }
            finally
            {
                targetVersion.Editing.EndEdit();
            }
        }

        //Explicitly set the workflow and state here on your new item (make sure you get the correct GUIDs by viewing raw values on your workflow and workflow state items)...
        private void manageWorkflow(Item newVersion, bool trusted)
        {
            try
            {
                newVersion.Editing.BeginEdit();
                using (new Sitecore.Sites.SiteContextSwitcher(Sitecore.Sites.SiteContextFactory.GetSiteContext("shell")))
                {

                    newVersion[FieldIDs.Workflow] = defaultWorkflowId;
                    var workflow =
                        Sitecore.Data.Database.GetDatabase("master").WorkflowProvider.GetWorkflow(
                            newVersion[FieldIDs.Workflow]);

                    workflow.Start(newVersion);
                    workflow.Execute(cmdId, newVersion, "Internal Translation Helper action", false, new object[0]);

                    workflow.Execute((trusted ? trustedCmdId : untrustedCmdId), newVersion, "Internal Translation Helper action", false, new object[0]);
                }
            }
            catch (Exception exp)
            {
                throw new Exception("Workflow management failed", exp);
            }
            finally
            {
                newVersion.Editing.EndEdit();
            }


        }


        private void email(Sitecore.Data.ID sourceId, string languageNodeId, string additionalText, string readyByDate)
        {
            Item email = getItemFromMasterDb(defaultConfigs);
            var body = email["body"];


            var languageNode = this.getItemFromMasterDb(languageNodeId);
            Sitecore.Data.Fields.MultilistField multilistField = languageNode.Fields["Translators"];
            if (multilistField.Count == 0)
                return;

            var configInstance = ConfigSettings.Instance;
            var sourceItem = this.getItemFromMasterDb(sourceId);

            foreach (Item item in multilistField.GetItems())
            {
                if (String.IsNullOrEmpty(item.Fields["unavailable"].Value))
                {
                    body = String.Format(body, item["name"], sourceItem.Paths.FullPath, sourceId, languageNode.Name, readyByDate, additionalText);
                    Aqueduct.Mail.MailMessage message = new Aqueduct.Mail.HtmlMailMessage
                    {
                        Subject = email["subject"],
                        Sender = configInstance.EmailSender,
                        Body = body
                    };
                    message.AddRecipient(new Aqueduct.Mail.EmailUser(item["name"], item["email"]));
                    try
                    {
                        message.Send(configInstance.SmtpServerInfo);
                    }
                    catch (Exception exp)
                    {
                        throw new Exception("Error Internal Translation message send  failed", exp);
                    }

                }
            }

        }

        /*
        public void Notify(Promise promise)
        {

            MailMessage message = new HtmlMailMessage
            {
                Subject = "New promise",
                Sender = ConfigSettings.Instance.EmailSender,
                Body = body
            };

            var recipientEmailAddress = ConfigSettings.Instance.PromiseRecieversEmailAddress;

            if (string.IsNullOrEmpty(recipientEmailAddress))
            {
                throw new ArgumentException("No recipient email address for admin promise created");
            }

            message.AddRecipient(new EmailUser(ConfigSettings.Instance.PromiseRecieversName, ConfigSettings.Instance.PromiseRecieversEmailAddress));

            try
            {
                message.Send(ConfigSettings.Instance.SmtpServerInfo);
            }
            catch (SmtpException ex)
            {
                AppLogger.LogError("Error notifying admin promise recieved", ex);
            }
        }
    
    */
    }
}

/* ref info.
    Database db = Factory.GetDatabase("master");
    if (Request.QueryString["_id"] != null)
    {
        var itm = db.GetItem(new ID(Request.QueryString["_id"]));
        WorkflowCommand[] availableCommands = wf.GetCommands(itm.Fields["__Workflow state"].Value);   
        wf.Execute(Request.QueryString["command"], itm, "Testing working flow new screens", false, new object[] { }); // Execute the workflow step.
    }
         
        using (new SiteContextSwitcher(SiteContextFactory.GetSiteContext("shell")))
    {
        wf.Execute(Request.QueryString["command"], itm, "Testing working flow new screens", false, new object[] { }); // Execute the workflow step.
 *workflow.Execute(this.WorkflowCommandID, ruleContext.Item, this.Comment, false, new object[] {});
    }
*/
