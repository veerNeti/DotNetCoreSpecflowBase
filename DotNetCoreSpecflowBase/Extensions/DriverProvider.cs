using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using System;
using System.Drawing;

namespace DotNetCoreSpecflowBase.Extensions
{
    public class DriverProvider
    {
        public IWebDriver driver;
        
        public IWebDriver GetWebDriver()
        {
            var browser = "chrome";
            var screenresolution = "";

            if (browser.Equals("IE", StringComparison.OrdinalIgnoreCase))
            {
                var IEOptions = new InternetExplorerOptions() {
                EnsureCleanSession=true,
                AttachToEdgeChrome=true,
                AcceptInsecureCertificates=true,
                };
                var internetExplorerDriverService = InternetExplorerDriverService.CreateDefaultService();
                internetExplorerDriverService.HideCommandPromptWindow = true;
                driver = new InternetExplorerDriver(internetExplorerDriverService, IEOptions);
            }
            else if (browser.Equals("Firefox", StringComparison.OrdinalIgnoreCase))
            {
                FirefoxOptions option = new FirefoxOptions();
                option.SetPreference("permissions.default.desktop-notification", 1);
                option.AddArgument("--start-maximized");
                option.AddArgument("--ignore-certificate-errors");
                option.AddArgument("test-type");
                driver = new FirefoxDriver(option);
                driver.Manage().Window.Maximize();
                driver.Manage().Window.Size = new Size(400, 851);
            }
            else if (browser.Equals("Chrome", StringComparison.OrdinalIgnoreCase))
            {
                ChromeOptions option = new ChromeOptions() { 
                    AcceptInsecureCertificates=true,
                };
                option.AddArgument("start-maximized");
                option.AddArgument("--no-sandbox");
                option.AddArgument("--disable-dev-shm-usage");
                driver = new ChromeDriver(option);

            }
            else if (browser.Equals("Headless Chrome", StringComparison.OrdinalIgnoreCase))
            {
                ChromeOptions option = new ChromeOptions();
                option.AddArgument("--headless");
                option.AddArgument("start-maximized");
                option.AddArgument("--ignore-certificate-errors");
                option.AddArgument("test-type");
                option.AddArgument("--no-sandbox");
                option.AddArgument("--disable-dev-shm-usage");

                driver = new ChromeDriver(option);
                driver.Manage().Window.Maximize();
            }
            else throw new ArgumentException(nameof(browser));

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return driver;
        }
    }
}

