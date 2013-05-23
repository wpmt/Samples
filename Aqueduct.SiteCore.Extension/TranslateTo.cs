using System;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;

namespace Aqueduct.SiteCore.Extension
{

    [Serializable]
    internal class TranslateTo : Command
    {

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            Context.ClientPage.Start(this, "Run", context.Parameters);
        }

        protected void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
            {
                if (args.Result != "yes")
                {
                    //Consume selector window
                    return;
                }
                SheerResponse.Eval("window.location.reload();");  //basic page refresh.
            }

            else
            {
                //produce selector window.
                //Sitecore.Context.ClientPage.ClientResponse.YesNoCancel("Are you sure you want to edit this?", "200", "200"); //basic popup window.
                //args.WaitForPostBack();
               // Sitecore.Globalization.Language sourceLang = Sitecore.Globalization.Language.Parse(args.Parameters["sourceLang"]);
                Sitecore.Text.UrlString popUpUrl = new Sitecore.Text.UrlString("/sitecore/content/MetaData/Translate.aspx");
                foreach (var name in args.Parameters)
                {
                    popUpUrl.Append(name.ToString(), args.Parameters[name.ToString()]+"<br/>");
                }
                
                //popUpUrl.Append("id", args.Parameters["sourceLang"]);
                //popUpUrl.Append("database", args.Parameters["database"]);
                //popUpUrl.Append("language", args.Parameters["language"]);
                Sitecore.Context.ClientPage.ClientResponse.ShowModalDialog(popUpUrl.ToString(), "400", "600", "", true);

                args.WaitForPostBack(false);    // if this is true this command will wait for the modal dialog created above to close
                // at which time the Run method will check for postback & args
    
            }

        }
        
        //public override string GetClick(CommandContext context, string click)
        //{
        //    return string.Empty;
        //}

      //  public override CommandState QueryState(CommandContext context)
     //   {
            //Assert.ArgumentNotNull(context, "context");
            //if (context.Items.Length != 1)
            //{
            //    return CommandState.Hidden;
            //}
            //Item item = context.Items[0];
            //if (!Context.IsAdministrator)
            //{
            //    return CommandState.Hidden;
            //}

            //if (item.Paths.FullPath.ToLowerInvariant().Contains("/sitecore/system"))
            //{
            //    return CommandState.Hidden;
            //}
            //if (item.Paths.FullPath.ToLowerInvariant().Contains("/sitecore/layout"))
            //{
            //    return CommandState.Hidden;
            //}

            //if (item.Paths.FullPath.ToUpperInvariant().Equals(Sitecore.Context.Site.StartPath.ToUpperInvariant()))
            //{
            //    return CommandState.Hidden;
            //}

            //if (item.Versions.Count > 0)
            //{
            //    return CommandState.Disabled;
            //}
            //if (item.Appearance.ReadOnly)
            //{
            //    return CommandState.Disabled;
            //}
            //if (Context.IsAdministrator)
            //{
            //    return CommandState.Enabled;
            //}
            //if (!item.Access.CanWrite())
            //{
            //    return CommandState.Disabled;
            //}
            //if (!item.Locking.CanLock() && !item.Locking.HasLock())
            //{
            //    return CommandState.Disabled;
            //}
            //if (!item.Access.CanWriteLanguage())
            //{
            //    return CommandState.Disabled;
            //}

            //return base.QueryState(context);
       //}
         
    }
}
