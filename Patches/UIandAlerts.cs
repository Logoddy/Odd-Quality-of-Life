using StardewModdingAPI.Events;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.GameData;

namespace Odd_Quality_of_Life.Patches
{
    internal class UIandAlerts
    {
        /// Festival Notifications
        public static void StartDay(object sender, DayStartedEventArgs e)
        {
            if (ModEntry.Config.Festival_notifications_heads_up)
            {
                if (!StardewValley.Utility.isFestivalDay() && !StardewValley.Utility.IsPassiveFestivalDay())
                {
                    return;
                }
                else
                {
                    if (StardewValley.Utility.isFestivalDay())
                    {
                        Dictionary<string, string> festivalData = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth);
                        var festivalName = festivalData["name"];
                        string festivalLocation = festivalData["conditions"].Split('/')[0];
                        string festivalTimes = festivalData["conditions"].Split('/')[1];
                        var festivalStartTime = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(festivalTimes, 0, "-1"));
                        var festivalEndTime = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(festivalTimes, 1, "-1"));

                        Game1.showGlobalMessage("The " + festivalName + " occurs today. It is located at the " + festivalLocation + ". The festival starts at " + festivalStartTime + " and ends at " + festivalEndTime);
                    }
                    else
                    {
                        if (!StardewValley.Utility.TryGetPassiveFestivalDataForDay(Game1.dayOfMonth, Game1.season, null, out var _, out var data))
                        {
                            ModEntry.TheMonitor.Log("Could not load passive festival data", LogLevel.Error);
                            return;
                        }
                        if (data.DisplayName != null && data.StartTime != null && data.StartTime > 0610)
                        {
                            var festivalName = data.DisplayName;
                            var festivalStartTime = data.StartTime;
                            var festivalEndTime = 2600;

                            Game1.showGlobalMessage("The " + festivalName + " occurs today. The festival starts at " + festivalStartTime);
                        }
                    }
                }
            }
        }
        public static void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (ModEntry.Config.Festival_notifications_warning)
            {
                if (StardewValley.Utility.isFestivalDay())
                {
                    Dictionary<string, string> festivalData = Game1.temporaryContent.Load<Dictionary<string, string>>("Data\\Festivals\\" + Game1.currentSeason + Game1.dayOfMonth);
                    var festivalName = festivalData["name"];
                    string festivalLocation = festivalData["conditions"].Split('/')[0];
                    string festivalTimes = festivalData["conditions"].Split('/')[1];
                    var festivalStartTime = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(festivalTimes, 0, "-1"));
                    var festivalEndTime = Convert.ToInt32(ArgUtility.SplitBySpaceAndGet(festivalTimes, 1, "-1"));

                    if (e.NewTime == festivalEndTime - 100 * ModEntry.Config.Festival_notifications_warning_time)
                    {
                        Game1.showGlobalMessage("The " + festivalName + " is ending soon. It is located at the " + festivalLocation + ". it ends at " + festivalEndTime);
                    }
                }
            }
        }
    }
}
