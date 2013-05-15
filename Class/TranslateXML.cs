using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Web.UI.Pages;
using Sitecore.Web;
using Sitecore.Data.Items;

using Sitecore;
using System.Windows.Controls;
using System.Windows;
using System.Reflection;
using System.Text;
using System.Windows.Media;
using Sitecore.Web.UI.Sheer;
using Sitecore.Shell.Framework;

namespace testlocal.Class
{

    public class TranslateXML : DialogForm
    {
        protected Sitecore.Web.UI.HtmlControls.Edit Edit;
        protected Sitecore.Web.UI.HtmlControls.Literal Title;
        protected Sitecore.Web.UI.WebControls.GridPanel CBList; 
        protected Sitecore.Web.UI.HtmlControls.DatePicker ReadyByDatePicker;
        protected Sitecore.Web.UI.HtmlControls.Button Button;
        private Item source;

        protected override void OnLoad(EventArgs e)
        {
            if (!Context.ClientPage.IsPostBack)
            {
                init();
                this.buildList();
                if (CBList.Controls.Count == 0)
                {
                    this.Button.Disabled = true;
                }
            }
            var id = WebUtil.GetQueryString("id");
            Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(id, "No id passed");
            Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            source = master.GetItem(id);
            Title.Text = source.DisplayName;
     
        }

        private void init()
        {
            //var id = WebUtil.GetQueryString("id");
            //Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(id, "No id passed");
            //Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            //source = master.GetItem(id);
            //Title.Text = source.DisplayName;
            //Culture way to doing this?
            ReadyByDatePicker.Value = DateUtil.ToIsoDate(DateTime.Now.AddDays(7));
        }

        [HandleMessage("internalTranslate:close")]
        protected void Close(Message message)
        {
            Windows.Close(CloseMethod.CloseWindow);
        }

        private void buildList()
        {
            
            Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            Item root = master.GetItem("/sitecore/content/MetaData/Global Data/internalTranslation/Languages");
            Sitecore.Diagnostics.Assert.IsNotNull(root, "item null");
            CBList.Controls.Clear();
            Edit.Value = String.Empty;
            var helper = new InternalTranslationHelper();
            foreach (Item item in root.Children)
            {
                if (helper.areTranslatorsAvailableFor(item.ID))
                {

                    Sitecore.Web.UI.HtmlControls.Checkbox test = new Sitecore.Web.UI.HtmlControls.Checkbox();
                    test.Header = item.Name;
                    test.ID = item.ID.ToString();
                    Context.ClientPage.AddControl(CBList, test);
                }
            }
        }

        [HandleMessage("internalTranslate:actionbutton")]
        protected void MessageButton(Message message)
        {
            Edit.Value = "";
            var helper = new InternalTranslationHelper();
            var list = new List<string>();
            foreach (var control in CBList.Controls)
            {
                if (control.GetType() == typeof(Sitecore.Web.UI.HtmlControls.Checkbox))
                {
                    var cb = ((Sitecore.Web.UI.HtmlControls.Checkbox)control);

                    if(cb.Checked)
                    {
                        //helper.createNewVerion(source.ID, cb.ID);
                        list.Add(cb.ID);
                    }
                    Edit.Value += ("  " + cb.Header + ":" + cb.Checked);
                }
            }
            helper.process(source.ID, list);
            Windows.Close(CloseMethod.CloseWindow);
        } 
    }

}