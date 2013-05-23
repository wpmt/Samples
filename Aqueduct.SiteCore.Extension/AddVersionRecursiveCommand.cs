using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
/* Alterd from the original Download */
namespace Aqueduct.SiteCore.Extension
{
    [Serializable]
    class AddVersionRecursiveCommand : Command
    {
        private HashSet<ID> LinkSet { get; set; }
        private readonly IList<ID> excludeID = new List<ID>(){TemplateIDs.Template, TemplateIDs.TemplateSection};
        
        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            if (null != context.Items && 1 == context.Items.Length)
            {
                Context.ClientPage.Start(this, "Run", context.Parameters);
            }
        }

        protected void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            string id = args.Parameters["id"];
            Language sourceLang = Language.Parse(args.Parameters["sourceLang"]);
            Language targetLang = Language.Parse(args.Parameters["targetLang"]);
            LinkSet = new HashSet<ID>();           
            
            var source = Context.ContentDatabase.GetItem(id, sourceLang);
            var target = Context.ContentDatabase.GetItem(id, targetLang);

            if (null == source || null == target || 0 < target.Versions.Count)
            {
                return;
            }

            // do we have control.
            if (Context.IsAdministrator || (target.Access.CanWrite() && (target.Locking.CanLock() || target.Locking.HasLock())))
            {
                if (SheerResponse.CheckModified())
                {
                    this.buildList(target);

                    foreach (ID linkId in LinkSet) 
                    {
                        this.copyVersion(linkId, sourceLang, targetLang);
                    }
                }
            }
        }



        private void buildList(Item item)
        {
            if (0 >= item.Versions.Count && !excludeID.Contains(item.TemplateID))
            {
                this.LinkSet.Add(item.ID);
            }
            this.addAllChildren(item);
        }

        private void addAllChildren(Item item)
        {
                foreach (Item reference in item.Children)
                {
                     this.buildList(reference); 
                }
        }

        private void copyVersion(ID id, Language sourceLang, Language targetLang)
        {
            Item source = Context.ContentDatabase.GetItem(id, sourceLang);
            Item target = Context.ContentDatabase.GetItem(id, targetLang);
            source.Fields.ReadAll();  //FORCE A READ OF ALL THE FIELDS.
            //Create a new target version and start editing
            target.Versions.AddVersion();
            target.Editing.BeginEdit();
            foreach (Field field in source.Fields)
            {
                if (!field.Shared)
                {
                    // if the target field Lang has a standard value use that.
                    target[field.Name] = String.IsNullOrEmpty(target.Fields[field.Name].GetStandardValue())?field.GetValue(true, true):target.Fields[field.Name].GetStandardValue();

                    // use the value from the copy from language no matter what.
                    //target[field.Name] = field.GetValue(true, true);


                }
            }
            
            target.Editing.EndEdit();
        }
        
    }
}
