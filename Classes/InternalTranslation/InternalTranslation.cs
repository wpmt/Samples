using System;
using System.Linq;
using Sitecore.Web.UI.Pages;
using Sitecore.Web;
using Sitecore.Data.Items;
using Sitecore;
using Sitecore.Web.UI.Sheer;
using Sitecore.Shell.Framework;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Diagnostics;


namespace ManCity.Site.Classes.InternalTranslation
{

    public class InternalTranslation : DialogForm
    {
        private Item source;
        private bool isBulkTranslate = false;
        private IList<ID> ids = new List<ID>();
        private readonly string defaultConfigs = Sitecore.Configuration.Settings.GetSetting("ConfigDefaults");

        protected Sitecore.Web.UI.HtmlControls.Memo AdditionalText;
        protected Sitecore.Web.UI.HtmlControls.Literal Title;
        protected Sitecore.Web.UI.WebControls.GridPanel CBList; 
        protected Sitecore.Web.UI.HtmlControls.DatePicker ReadyByDatePicker;
        protected Sitecore.Web.UI.HtmlControls.Button ActionButton;
        protected Sitecore.Web.UI.HtmlControls.Button CloseButton;
        protected Sitecore.Web.UI.HtmlControls.Button CancelButton; 
        protected Sitecore.Web.UI.WebControls.GridPanel BulkList;
        protected Sitecore.Web.UI.HtmlControls.Literal ListOfitems;
        

        protected override void OnLoad(EventArgs e)
        {
            var id = WebUtil.GetQueryString("id");
            Sitecore.Diagnostics.Assert.IsNotNullOrEmpty(id, "No id passed");
            Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            source = master.GetItem(id);
            isBulkTranslate = "{E6962F48-42A0-4C10-AF8C-256E7B6CB9CE}".Equals(id);
        
            if (!Context.ClientPage.IsPostBack)
            { 
                init(); 
                this.buildLanguageList();
                if (CBList.Controls.Count == 0)
                {
                    this.ActionButton.Enabled = false;
                }
            }
            Title.Text = String.IsNullOrEmpty(source.DisplayName)?source.Name:source.DisplayName;
            if (isBulkTranslate) 
            {
                this.buildBulkCopyList(source);
                BulkList.Visible = true;
            }
            else
            {
                ids.Clear();
                ids.Add(source.ID);
            }


        }

        private void buildBulkCopyList(Item source)
        {
            ids.Clear();
            var sb = new System.Text.StringBuilder();
            Sitecore.Data.Fields.MultilistField list = source.Fields["Items for translation"];
            foreach (Item item in list.GetItems())
            {
                this.ids.Add(item.ID);
                sb.Append(item.Paths.ContentPath);
                sb.Append("<br/>");
                
            }
            this.ListOfitems.Text= sb.ToString();
          
        }
    

        private void init()
        {
            ReadyByDatePicker.Value = DateUtil.ToIsoDate(DateTime.Now.AddDays(7));
            ReadyByDatePicker.Format = "dd/MM/yyyy";
        }


        private void buildLanguageList()
        {
            Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            Item root = master.GetItem(Sitecore.Configuration.Settings.GetSetting("LanguageRoot"));
            Sitecore.Diagnostics.Assert.IsNotNull(root, "item null");
            CBList.Controls.Clear();
            this.AdditionalText.Value = String.Empty;
            var helper = new InternalTranslationHelper();
            foreach (Item item in root.Children)
            {
                if (helper.areTranslatorsAvailableFor(item.ID.ToString()))
                {
                    var test = new Sitecore.Web.UI.HtmlControls.Checkbox {Header = item.Name, ID = item.ID.ToString()};
                    Context.ClientPage.AddControl(CBList, test);
                }
            }
        }

        [HandleMessage ("internalTranslate:actionbutton")]
        protected void ActionButtonClicked(Message message)
        {
            var helper = new InternalTranslationHelper();
            var list = (from object control in CBList.Controls
                        where control.GetType() == typeof (Sitecore.Web.UI.HtmlControls.Checkbox)
                        select ((Sitecore.Web.UI.HtmlControls.Checkbox) control)
                        into cb where cb.Checked select cb.ID).ToList();
            if (list.Count < 1)
            {
                
                Sitecore.Web.UI.Sheer.SheerResponse.
                    Alert(helper.getItemFromMasterDb(defaultConfigs)["no language selected warning"], string.Empty);
                return;
            }
            this.ActionButton.Enabled = false;
            this.CancelButton.Enabled = false;
            this.CloseButton.Enabled = false;

            
            helper.process(ids, list, (DateUtil.IsoDateToDateTime(this.ReadyByDatePicker.Value)).ToString("dd/MM/yyyy"), this.AdditionalText.Value);
            Windows.Close(CloseMethod.CloseWindow);
        }


        [HandleMessage("internalTranslate:cancelbutton")]
        protected void CancelButtonClicked(Message message)
        {
            Windows.Close(CloseMethod.CloseWindow);
        }

        [HandleMessage("internalTranslate:close")]
        protected void Close(Message message)
        {
            Windows.Close(CloseMethod.CloseWindow);
        }

    }

}