using OpenQA.Selenium;

namespace DotNetCoreSpecflowBase.Extensions
{
    public static class ElementExtension
    {
        public static bool IsElementDisplayedOrEnabled(this IWebElement webElement)
        {
            bool result = false;
            if (webElement!= null && webElement.Displayed && webElement.Enabled)
                result = true; 
            return result;
        }
    }
}
