using BoDi;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Protractor;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using OpenQA.Selenium.Support;

namespace DotNetCoreSpecflowBase.Test.Util
{
    public class JsWebDriver
    {
        private IWebDriver _driver;
        private IJavaScriptExecutor _js;
        //string jsHighlighter = "arguments[0].style.height='auto'; arguments[0].style.visibility='visible';";
        const string HIGHLIGHTER = "return arguments[0].setAttribute('style','border: solid 8px red')";
        const string REMOVEHIGHLIGHER = "return arguments[0].setAttribute('style','')";

        public JsWebDriver(IObjectContainer container)
        {
            _driver = container.Resolve<IWebDriver>();
            if (_driver != null)
            {
                _js = (IJavaScriptExecutor)_driver;
            }
        }
        public WebDriverWait GetWait() => new WebDriverWait(clock:null,driver: _driver, timeout: new TimeSpan(0, 0, 10), sleepInterval: new TimeSpan(0, 0, 1));

        public void waitTillTheLocatorIsDisplayed(By locator)
        {
            GetWait().Until(w => _driver.FindElements(locator).Any(o => o.Displayed));
        }
        public void waitTillTheLocatorIsDisplayed(IList<IWebElement> element)
        {
            GetWait().Until(w => element.Any(o => o.Displayed));
        }
        public void ExecuteScriptOnLocator(string script, By locator)
        {
            _js.ExecuteScript(script, _driver.FindElement(locator));
        }
        public void ExecuteScriptOnWebElement(string script, IWebElement webElement)
        {
            _js.ExecuteScript(script, webElement);
        }

        public void ScrollUpApage()
        {
            _js.ExecuteScript("return window.scrollTo(0, 0)");
        }
        public void ScrollDownAPage()
        {
            _js.ExecuteScript("return window.scrollTo(0, document.body.scrollHeight)");
        }
        public void ScrollToCenterOfPage()
        {
            string Script = "return window.scrollTo(0, document.body.scrollHeight/2)";
            _js.ExecuteScript(script: Script);
        }
        public void ScrollAPageVerticallyToVisibleElement(IWebElement element)
        {
            var script = "return arguments[0].scrollIntoView(true);";
            _js.ExecuteScript("arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'})", element);
            ExecuteScriptOnWebElement(script: script, element);
        }
        public void ScrollElementToViewSmooth(IWebElement element)
        {
            var script = "arguments[0].scrollIntoView({behavior: 'smooth', block: 'center'})";
            ExecuteScriptOnWebElement(script: script, element);
        }
        public void ScrollAPageVerticallyToVisibleLocator(By locator)
        {
            var script = "return arguments[0].scrollIntoView(true);";
            ExecuteScriptOnLocator(script: script, locator);
            HighlightTheElement(_driver.FindElement(locator));
        }

        private void BrowserSleep()
        {
            int TimoutinSeconds = 10 * 1000;
            Stopwatch stopwatch = Stopwatch.StartNew();
            StringBuilder sb = new StringBuilder("return window.setTimeout(arguments[arguments.length - 1], ");
            sb.Append(TimoutinSeconds);
            sb.Append(");");
            _js.ExecuteScript(script: sb.ToString());
            //Thread.Sleep(TimeSpan.FromMilliseconds(TimoutinSeconds));
            stopwatch.Stop();
            long ticksThisTime = stopwatch.ElapsedTicks;
            Log.Information("Elapsed time: " + ticksThisTime);
        }

        public void JsClick(By locator)
        {
            string javascript = "return arguments[0].click()";
            HighlightTheLocator(locator);
            ExecuteScriptOnWebElement(javascript, _driver.FindElement(locator));
        }

        public void JsClick(IWebElement element)
        {
            string javascript = "return arguments[0].click()";
            ScrollElementToViewSmooth(element);
            HighlightTheElement(element);
            ExecuteScriptOnWebElement(javascript, element);
        }
        public void HighlightTheElement(IWebElement element)
        {
            ExecuteScriptOnWebElement(script: HIGHLIGHTER, webElement: element);
            ExecuteScriptOnWebElement(script: REMOVEHIGHLIGHER, webElement: element);
        }
        public void HighlightTheLocator(By locator)
        {
            ScrollElementToViewSmooth(_driver.FindElement(locator));
            ExecuteScriptOnLocator(script: HIGHLIGHTER, locator: locator);
            ExecuteScriptOnLocator(script: REMOVEHIGHLIGHER, locator: locator);
        }

        public void StoreTheLoadingTime()
        {
            var ResponseTime = Convert.ToInt32(_js.ExecuteScript("return window.performance.timing.domContentLoadedEventEnd-window.performance.timing.navigationStart;"));
            Log.Information(string.Format("Page {0} loading time is {1} ms", _driver.Title, ResponseTime));
        }

        public bool CheckAngularVersion()
        {
            bool checkAngular = false;
            string ScriptForNoPageLoaded = "document.body.innerText===''";
            string ScriptForErrorPgLoaded = "document.body.innerText.includes('Server Error')";
            string scriptForReloadThePage = "window.location.reload()";
            string scriptForAngularLoading = "((window.getAllAngularTestabilities()[0]._ngZone.hasPendingMicrotasks=== false) && (window.getAllAngularTestabilities()[0]._ngZone._nesting == 0) &&(window.getAllAngularTestabilities()[0]._ngZone.isStable == true))";
            string scriptForAngularApp = "document.getElementsByTagName('app-root')[0].attributes['ng-version'] === undefined";
            string scriptForJsReadyStateComplete = "document.readyState";
            try
            {
                for (int i = 0; i < 2; i++)
                {
                    BrowserSleep();
                    bool NoPageLoaded = (bool)_js.ExecuteScript("return " + ScriptForNoPageLoaded);
                    if (NoPageLoaded)
                    {
                        Log.Information("Page Loaded: " + NoPageLoaded);
                        Console.WriteLine("Page Loaded: " + NoPageLoaded);
                        Log.Information("Js Reloading..!!.");
                        Console.WriteLine("Js Reloading..!!.");
                        _js.ExecuteScript("return " + scriptForReloadThePage);
                        NoPageLoaded = (bool)_js.ExecuteScript("return " + ScriptForNoPageLoaded);
                        Log.Information("Page Loaded: " + NoPageLoaded);
                        checkAngular = NoPageLoaded;
                    }
                    bool ErrorPageLoaded = (bool)_js.ExecuteScript("return " + ScriptForErrorPgLoaded);
                    if (ErrorPageLoaded)
                    {
                        Log.Information("Page Loaded: " + ErrorPageLoaded);
                        Console.WriteLine("Page Loaded: " + ErrorPageLoaded);
                        Log.Information("Js Reloading..!!.");
                        Console.WriteLine("Js Reloading..!!");
                        _js.ExecuteScript("return " + scriptForReloadThePage);
                        ErrorPageLoaded = (bool)_js.ExecuteScript("return " + ScriptForErrorPgLoaded);
                        Log.Information("Page Loaded: " + ErrorPageLoaded);
                        checkAngular = ErrorPageLoaded;
                    }
                    checkAngular = (bool)_js.ExecuteScript("return " + scriptForAngularLoading) ? true : false;
                    checkAngular = !(bool)_js.ExecuteScript("return " + scriptForAngularApp) ? (bool)_js.ExecuteScript(script: "return " + scriptForJsReadyStateComplete).ToString().Equals("complete", StringComparison.CurrentCultureIgnoreCase) : false;
                    //BrowserSleep();
                    Log.Information("Angular Loading...... " + checkAngular);
                    if (checkAngular)
                    {
                        break;
                    }
                }
            }
            catch (Exception)
            {
            }
            Log.Information("Angular Loading Complete...... " + checkAngular);
            return checkAngular;
            //loading complete angular if this is false
        }


        /*public void WaitForLoad (int timeoutSec)
        {
            _driver.Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(timeoutSec);
            NgDriver.IgnoreSynchronization = true;
            // Summary:
            //     If true, Protractor will not attempt to synchronize with the page before performing
            //     actions. This can be harmful because Protractor will not wait until $timeouts
            //     and $http calls have been processed, which can cause tests to become flaky. This
            //     should be used only when necessary, such as when a page continuously polls an
            //     API using $timeout.
            try
            {
                NgDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(timeoutSec);
                NgDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(timeoutSec);
                WebDriverWait wait = new WebDriverWait(driver: NgDriver, timeout: new TimeSpan(days: 0, hours: 0, minutes: 0, seconds: timeoutSec, milliseconds: 20));
                NgDriver.WaitForAngular();
                for (int i = 0; i < _localSettings.SettingsReader().Timeouts.Retry; i++)
                {
                    if (CheckAngularVersion())
                        break;
                    else
                        continue;
                }
            }
            catch (Exception)
            {
            }
            StoreTheLoadingTime();
            NgDriver.IgnoreSynchronization = false;
        }
*/

    }
}
