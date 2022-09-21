using AventStack.ExtentReports;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using BoDi;
using DotNetCoreSpecflowBase.Extensions;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using Selenium.Axe;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using WDSE;
using WDSE.Decorators;
using WDSE.ScreenshotMaker;
using Log = Serilog.Log;

namespace DotNetCoreSpecflowBase.Hook
{
    [Binding]
    public class Hooks
    {
        private static ExtentTest _feature;
        private ExtentTest _scenario;
        private static AventStack.ExtentReports.ExtentReports _extent;
        private IObjectContainer _objectContainer;
        private static readonly string PathReport = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\net5.0\\", "") + "ExtentReports\\";

        public string _userIdString;


        public Hooks(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
            _objectContainer.RegisterInstanceAs(new DriverProvider().GetWebDriver());
        }

        [BeforeTestRun]
        public static void ConfigureReport()
        {
            var reporter = new ExtentHtmlReporter(PathReport);
            _extent = new AventStack.ExtentReports.ExtentReports();
            _extent.AttachReporter(reporter);

            LoggingLevelSwitch levelSwitch = new LoggingLevelSwitch(LogEventLevel.Debug);
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(levelSwitch)
                .WriteTo.File(PathReport + @"\Logs",
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} | {Level:u3} | {Message} {NewLine}",
                rollingInterval: RollingInterval.Day).CreateLogger();
        }


        [BeforeFeature]
        public static void CreateFeature(FeatureContext context)
        {
            _feature = _extent.CreateTest<Feature>(context.FeatureInfo.Title);
          Log.Information("Selecting feature file {0} to run");
        }

        [BeforeScenario]
        public async Task BeforeScenario(ScenarioContext context)
        {
            _scenario = _feature.CreateNode<Scenario>(context.ScenarioInfo.Title);
            Log.Information("Selecting scenario {0} to run", context.ScenarioInfo.Title);
        }

        [AfterStep]
        public void InsertReportingSteps(ScenarioContext scenarioContext)
        {
            var webDriver = _objectContainer.Resolve<IWebDriver>();
            var stepType = scenarioContext.StepContext.StepInfo.StepDefinitionType.ToString();
            Console.WriteLine("Element are ", stepType);

            if (scenarioContext.TestError == null)
            {
                if (stepType == "Given")
                    _scenario.CreateNode<Given>(scenarioContext.StepContext.StepInfo.Text);
                else if (stepType == "When")
                    _scenario.CreateNode<When>(scenarioContext.StepContext.StepInfo.Text);
                else if (stepType == "Then")
                    _scenario.CreateNode<Then>(scenarioContext.StepContext.StepInfo.Text);
                else if (stepType == "And")
                    _scenario.CreateNode<And>(scenarioContext.StepContext.StepInfo.Text);
            }
            else if (scenarioContext.TestError.ToString() != null)
            {
                if (stepType == "Given")
                {
                    String path = TakeScreenshot(scenarioContext, webDriver);
                    _scenario.CreateNode<Given>(scenarioContext.StepContext.StepInfo.Text).Fail(scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromPath(path).Build());
                    String[] Tags = scenarioContext.ScenarioInfo.Tags;
                    int TagsLength = scenarioContext.ScenarioInfo.Tags.Length;
                    Log.Error("Test Step Failed | " + scenarioContext.TestError.Message);

                    try
                    {
                        for (int i = 0; i <= TagsLength; i++)
                        {
                            if (Tags[i].Equals("Accessibility"))
                            {
                                AxeResult results = webDriver.Analyze();
                                String test = results.ToString();
                                foreach (var xyz in results.Violations)
                                {
                                    String res = xyz.Description.ToString();
                                    int res2 = xyz.Tags.Length;
                                    _scenario.CreateNode<Given>("Accessibility Errors").Fail(res);
                                    string[] nod = null;
                                    foreach (var node in xyz.Nodes)
                                    {
                                        int j = 0;
                                        String node1 = node.Html;
                                        nod[j] = node1;
                                        j++;
                                    }
                                    String resultTag2 = xyz.Tags.GetValue(1).ToString();
                                }
                                String res3 = results.Violations.GetValue(0).GetType().Name;
                                String res4 = results.Violations.GetValue(0).GetType().Name;
                                String res5 = results.Violations.GetValue(0).GetType().Name;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                else if (stepType == "When")
                {
                    String path = TakeScreenshot(scenarioContext, webDriver);
                    _scenario.CreateNode<When>(scenarioContext.StepContext.StepInfo.Text).Fail(scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromPath(path).Build());
                    Log.Error("Test Step Failed | " + scenarioContext.TestError.Message);
                }

                else if (stepType == "Then")
                {
                    String path = TakeScreenshot(scenarioContext, webDriver);
                    _scenario.CreateNode<Then>(scenarioContext.StepContext.StepInfo.Text).Fail(scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromPath(path).Build());
                    Log.Error("Test Step Failed | " + scenarioContext.TestError.Message);
                }
                else if (stepType == "And")
                {
                    String path = TakeScreenshot(scenarioContext, webDriver);
                    _scenario.CreateNode<And>(scenarioContext.StepContext.StepInfo.Text).Fail(scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromPath(path).Build());
                    Log.Error("Test Step Failed | " + scenarioContext.TestError.Message);
                }
                else
                {
                    String path = TakeScreenshot(scenarioContext, webDriver);
                    _scenario.CreateNode<And>(scenarioContext.StepContext.StepInfo.Text).Fail(scenarioContext.TestError.Message, MediaEntityBuilder.CreateScreenCaptureFromPath(path).Build());
                    Log.Error("Test Step Failed | " + scenarioContext.TestError.Message);
                }
            }

            //Pending Status
            if (TestStatus.Skipped.ToString() == "StepDefinitionPending")
            {
                if (stepType == "Given")
                    _scenario.CreateNode<Given>(scenarioContext.StepContext.StepInfo.Text).Skip("Step Definition Pending");
                else if (stepType == "When")
                    _scenario.CreateNode<When>(scenarioContext.StepContext.StepInfo.Text).Skip("Step Definition Pending");
                else if (stepType == "Then")
                    _scenario.CreateNode<Then>(scenarioContext.StepContext.StepInfo.Text).Skip("Step Definition Pending");
            }

        }

        public string TakeScreenshot(ScenarioContext context, IWebDriver driver)
        {
            string path2 = "ErrorScreenshots\\";
            var directoryinfo = System.IO.Directory.CreateDirectory(Path.Combine(PathReport, path2));
            string path = directoryinfo + context.ScenarioInfo.Title + ".png";
            VerticalCombineDecorator vcd = new VerticalCombineDecorator(new ScreenshotMaker().RemoveScrollBarsWhileShooting());
            driver.TakeScreenshot(vcd).ToMagickImage().Write(path, ImageMagick.MagickFormat.Png);
            String relativePath = "ErrorScreenshots\\" + context.ScenarioInfo.Title + ".png";
            return relativePath;

        }

        [AfterScenario]
        public async Task AfterScenarioAsync()
        {
            _objectContainer.Resolve<IWebDriver>().Quit();
        }

        [AfterTestRun]
        public static void FlushExtent()
        {
            _extent.Flush();
        }


    }
}
