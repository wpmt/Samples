using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Globalization;
using Sitecore.Diagnostics;

namespace testlocal.Class
{
    public class InternalTranslationHelper
    {
        public bool areTranslatorsAvailableFor(Sitecore.Data.ID languageNodeID)
        {
            var flag = false;
            Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            Assert.IsNotNull(master,"No Master DB  found");
            Item languageNode = master.GetItem(languageNodeID);
            Assert.IsNotNull(languageNode, "No languageNode found");
            Sitecore.Data.Fields.MultilistField multilistField = languageNode.Fields["Translators"];
            if (multilistField.Count == 0)
                return flag;
            
            foreach (Item item in multilistField.GetItems())
            {
               if(String.IsNullOrEmpty(item.Fields["unavailable"].Value))
               {
                   flag = true;
               }
            }


            return flag;
        }




        public void process(Sitecore.Data.ID sourceId, IList<String> languageNodeIds) 
        {
            foreach(var languageNodeId in languageNodeIds)
            {
                createNewVerion(sourceId, languageNodeId);
            }
        }


        private void createNewVerion(Sitecore.Data.ID sourceId, string languageNodeId) 
        {
            Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            Assert.IsNotNull(master, "No Master DB  found");
            Item languageNode = master.GetItem(languageNodeId);
            Assert.IsNotNull(languageNode, "No languageNode found");
            Sitecore.Data.Fields.InternalLinkField t1 =(Sitecore.Data.Fields.InternalLinkField) languageNode.Fields["Language"];
            Sitecore.Globalization.Language targetLang = Language.Parse(t1.TargetItem.Name);           
            Item newVersion = Sitecore.Context.ContentDatabase.GetItem(sourceId, targetLang);
            try
            {
                newVersion.Editing.BeginEdit();
                newVersion.Versions.AddVersion();
                //What to do about workflow?
                //If it is not in a workflow use the default one.
                if (String.IsNullOrEmpty(newVersion[FieldIDs.Workflow]))
                {
                    //Explicitly set the workflow and state here on your new item (make sure you get the correct GUIDs by viewing raw values on your workflow and workflow state items)...
                    newVersion[FieldIDs.Workflow] =
                        Sitecore.Configuration.Settings.GetSetting("DefaultTranslationWorkFlowID");
                    newVersion[FieldIDs.WorkflowState] =
                        Sitecore.Configuration.Settings.GetSetting("DefaultTranslationWorkFlowState");
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                newVersion.Editing.EndEdit();    
            }
        }

        /*
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
        }
         */
    }
}