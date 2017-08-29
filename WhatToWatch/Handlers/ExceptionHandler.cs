using System.Threading.Tasks;
using Xamarin.Forms;

namespace WhatToWatch.Handlers
{
    public static class ExceptionHandler
    {
        public static void HandleMessage(Page page, string message)
        {
            page.DisplayAlert(AppResources.Message, message, AppResources.Continue);
        }

        public static void HandleError(Page page, string message)
        {
            page.DisplayAlert(AppResources.Error, message, AppResources.Continue);
        }

        public static void HandleWarning(Page page, string message)
        {
            page.DisplayAlert(AppResources.Caution, message, AppResources.Continue);
        }

        public static async Task<bool> HandleDialog(Page page, string message, string OK, string CANCEL)
        {
            return await page.DisplayAlert(AppResources.Caution, message, OK, CANCEL);
        }
    }
}
