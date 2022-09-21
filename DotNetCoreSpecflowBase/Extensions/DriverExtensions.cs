using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;

namespace DotNetCoreSpecflowBase.Extensions
{
    public static class DriverExtensions
    {
        private const int MAX_WAIT_SECONDS = 5;
        public static void SwitchToNewWindow(this IWebDriver driver)
        {
            driver.WaitForNewWindowHandle();
            driver.SwitchTo().Window(driver.WindowHandles.Last());
        }
        public static void SwitchToMainWindow(this IWebDriver driver)
        {
            driver.WaitForWindowToClose();
            driver.SwitchTo().Window(driver.WindowHandles.First());
        }

        private static void WaitForNewWindowHandle(this IWebDriver driver)
        {
            WaitForCondition(() => driver.WindowHandles.Count > 1, "waiting for new handle");
        }

        private static void WaitForWindowToClose(this IWebDriver driver)
        {
            WaitForCondition(() => driver.WindowHandles.Count == 1, "waiting for window to close");
        }

        private static void WaitForCondition(Func<bool> condition, string description = "")
        {
            var maxLoops = MAX_WAIT_SECONDS * 10;
            var i = 0;
            for (; i < maxLoops; i++)
            {
                if (condition()) break;
                Thread.Sleep(100);
            }
            if (i == maxLoops)
            {
                throw new TimeoutException($"Timed out after {MAX_WAIT_SECONDS} seconds{(string.IsNullOrEmpty(description) ? "" : $" {description}")}");
            }
        }
    }
}
