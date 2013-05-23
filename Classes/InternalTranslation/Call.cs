using System;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Text;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System.Collections.Generic;

namespace ManCity.Site.Classes.InternalTranslation
{
    [Serializable]
    public class Call : Command
    {
        private readonly IList<string> excludeTemplateFullNameList = new List<string>() { "ManCity/Data Templates/Hidden Folder",
                                                                        "ManCity/Content Templates/Expandable Nav/Expandable Nav Columns",
                                                                        "ManCity/Content Templates/Expandable Nav/Links Column",
                                                                        "ManCity/Content Templates/News/NewsFolder",
                                                                        "ManCity/Content Templates/News/MatchReportFolder",
                                                                        "ManCity/Data Templates/Footer Link Parent",
                                                                        "ManCity/Data Templates/Footer Link Image",
                                                                        "ManCity/Content Templates/Fixtures/Season Folder",
                                                                        "ManCity/Content Templates/Fixtures/Fixture Month",
                                                                        "ManCity/Content Templates/Link Node",
                                                                        "ManCity/Content Templates/Players/PlayersLandingNewSignings",
                                                                        "ManCity/Content Templates/Players/PlayerGroup",
                                                                        "ManCity/Content Templates/Playlist/PlaylistLanding",
                                                                        "ManCity/Content Templates/Playlist/PlaylistCategory",
                                                                        "ManCity/Content Templates/Playlist/PlaylistFolder",
                                                                        "ManCity/Content Templates/Search/SearchResults",
                                                                        "ManCity/Content Templates/Hospitality/HospitalityGroup",
                                                                        "ManCity/Content Templates/Tickets/CardEligibilityCheck",
                                                                        "ManCity/Content Templates/Tickets/SeatSelection",
                                                                        "ManCity/Content Templates/Tickets/TicketEligibilityCheck",
                                                                        "ManCity/Content Templates/Tickets/TicketAllocation",
                                                                        "ManCity/Content Templates/AdventCalendar/Day",
                                                                        "ManCity/Content Templates/Aggregator/LatestNewsAggregator",
                                                                        "ManCity/Meta Data/Standard Text",
                                                                        "eAcademy/Meta Data/Range",
                                                                        "ManCityKicks/Data Templates/CompetitionType",
                                                                        "ManCity/Meta Data/Search/CityRecommendsFolder",
                                                                        "ManCity/Data Templates/Club Locale",
                                                                        "ManCity/Meta Data/Standard Text",
                                                                        "ManCity/Meta Data/Search/PrioritisedSearchItem",
                                                                        "ManCity/Meta Data/Region" };


        public override void Execute(CommandContext context)
        {
            Sitecore.Diagnostics.Assert.ArgumentNotNull(context, "context");
            Item[] items = context.Items;
            if (items.Length == 1)
            {
                UrlString str = new UrlString(UIUtil.GetUri("control:InternalTranslation"));
                Item item = items[0];
                str["id"] = item.ID.ToString();
                str["na"] = item.Name;
                str["la"] = item.Language.ToString();
                str["vs"] = item.Version.ToString();
                SheerResponse.ShowModalDialog(str.ToString(), "650", "600");
            }

        }

        public override CommandState QueryState(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");

            if (!ManCity.Site.Classes.Utils.FeatureTogglesParser.IsInternalTranslationEnabled)
            {
                return CommandState.Hidden;
            }

            if (context.Items.Length != 1)
            {
                return CommandState.Hidden;
            }
            Item item = context.Items[0];
            //if (!Context.IsAdministrator)
            //{
            //    return CommandState.Hidden;
            //}
            if (excludeTemplateFullNameList.Contains(item.Template.FullName))
            {
                return CommandState.Disabled;
            }
            return base.QueryState(context);
        }
    }
}
