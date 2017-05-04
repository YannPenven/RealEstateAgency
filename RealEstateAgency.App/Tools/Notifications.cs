using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace RealEstateAgency.App.Tools
{
    public static class Notifications
    {
        public async static Task<bool> ShowErrors()
        {
            return await ShowErrors(await Core.DataAccess.Connection.GetCurrentAsync());
        }
        public async static Task<bool> ShowErrors(Core.DataAccess.Connection dbConn)
        {
            return await ShowErrors(dbConn.Errors);
        }
        public async static Task<bool> ShowErrors(List<string> errors)
        {
            if (errors.Count == 0) return false;

            string message = "";
            foreach (string error in errors)
            {
                if (!string.IsNullOrEmpty(message)) message += "\n\r";
                message += error;
            }

            var dialog = new MessageDialog(message, "Erreurs");
            await dialog.ShowAsync();

            return true;
        }
    }
}
