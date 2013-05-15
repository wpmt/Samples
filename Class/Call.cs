using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Text;
using Sitecore;
using Sitecore.Data.Items;

namespace testlocal.Class
{
    [Serializable]
    public class Call : Command
    {
        public override void Execute(CommandContext context)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(context, "context");
          //  Sitecore.Text.UrlString url = new Sitecore.Text.UrlString("/TestXML");
      Item[] items = context.Items;
      if (items.Length == 1)
      {
          UrlString str = new UrlString(UIUtil.GetUri("control:TestXML"));
          Item item = items[0];
          str["id"] = item.ID.ToString();
          str["na"] = item.Name;
          str["la"] = item.Language.ToString();
          str["vs"] = item.Version.ToString();
          SheerResponse.ShowModalDialog(str.ToString(), "650", "600");
      }
            // SheerResponse.ShowModalDialog(url.ToString(), "650", "500");
            
            
            //Assert.ArgumentNotNull(context, "context");
            //if (null != context.Items && 1 == context.Items.Length)
            //{
            //    Context.ClientPage.Start(this, "Run", context.Parameters);
            //}
        }

        protected void Run(ClientPipelineArgs args)
        {
        }
    }
}
