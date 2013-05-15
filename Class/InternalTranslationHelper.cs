using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;

namespace testlocal.Class
{
    public class InternalTranslationHelper
    {
        public bool areTranslatorsAvailableFor(Sitecore.Data.ID id)
        {
            Sitecore.Data.Database master = Sitecore.Data.Database.GetDatabase("master");
            Item languageNode = master.GetItem(id);

            return false;
        }
    }
}