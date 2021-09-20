using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace computerDB
{
    [TestFixture, Timeout (3000)]
    [Author("Ryan Cabreros")]
    public class Tests
    {
        IWebDriver  driver;
        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Url = "http://computer-database.gatling.io/computers";
        }

        [TestCase ("new computer")]
        [TestCase ("latest computer")]
        public void TC1(string compName)
        {
            IWebElement add = driver.FindElement(By.Id("add"));
            add.Click();

            IWebElement name = driver.FindElement(By.Id("name"));
            name.SendKeys(compName);

            IWebElement submit = driver.FindElement(By.XPath("//input[@type='submit']"));
            submit.Click();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement bodyTag = wait.Until(e => e.FindElement(By.XPath("//div[contains(@class,'alert-message') and contains(@class,'warning')]")));
            Assert.IsTrue(bodyTag.Text.Contains("Done ! Computer " + compName + " has been created"));
        }

        [TestCase ("Pilot ACE")]
        [TestCase ("PalmPilot")]
        public void TC2(string compName)
        {
            // waiting for result table to appear
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IWebElement searchbox = wait.Until(e => e.FindElement(By.Id("searchbox")));
            searchbox.SendKeys(compName);

            IWebElement searchsubmit = driver.FindElement(By.Id("searchsubmit"));
            searchsubmit.Click();

            IWebElement bodyTag = wait.Until(e => e.FindElement(By.XPath("//table/tbody//td/a")));
            Assert.IsTrue(bodyTag.Text.Contains(compName));

            IWebElement searchboxClear = driver.FindElement(By.Id("searchbox"));
            searchboxClear.Clear();
        }

        [TestCase ("Pilot ACE", "1991-01-31", "Atari")]
        public void TC3(string compName, string introDate, string computerComp)
        {
            IWebElement searchbox = driver.FindElement(By.Id("searchbox"));
            searchbox.SendKeys(compName);

            IWebElement searchsubmit = driver.FindElement(By.Id("searchsubmit"));
            searchsubmit.Click();

            // waiting for result table to appear
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement bodyTag = wait.Until(e => e.FindElement(By.XPath("//table/tbody//td/a")));
            bodyTag.Click();

            IWebElement introduced = driver.FindElement(By.Id("introduced"));
            introduced.SendKeys(introDate);

            IWebElement company = driver.FindElement(By.Id("company"));
            SelectElement select = new SelectElement(company);
            select.SelectByText(computerComp);

            IWebElement submit = driver.FindElement(By.XPath("//input[@type='submit']"));
            submit.Click();
            
            IWebElement doneTag = wait.Until(e => e.FindElement(By.XPath("//div[contains(@class,'alert-message') and contains(@class,'warning')]")));
            Assert.IsTrue(doneTag.Text.Contains("Done ! Computer " + compName + " has been updated"));
        }

        [TestCase ("ARRA")]
        public void TC4(string compName)
        {
            // waiting for result table to appear
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement searchbox = wait.Until(e => e.FindElement(By.Id("searchbox")));
            searchbox.SendKeys(compName);

            IWebElement searchsubmit = driver.FindElement(By.Id("searchsubmit"));
            searchsubmit.Click();

            IWebElement bodyTag = wait.Until(e => e.FindElement(By.XPath("//table/tbody//td/a")));
            bodyTag.Click();

            IWebElement delete = driver.FindElement(By.XPath("//input[@value='Delete this computer']"));
            delete.Click();

            IWebElement doneTag = wait.Until(e => e.FindElement(By.XPath("//div[contains(@class,'alert-message') and contains(@class,'warning')]")));
            Assert.IsTrue(doneTag.Text.Contains("Done ! Computer " + compName + " has been deleted"));
        }

        [OneTimeTearDown]
        public void Close()
        {
           driver.Quit();
        }
    }

    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }
    }
}

namespace JSAlerts
{
    [TestFixture, Timeout (3000)]
    [Author("Ryan Cabreros")]
    public class Tests
    {
        IWebDriver  driver;
        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Url = "https://the-internet.herokuapp.com/javascript_alerts";
        }

        [Test]
        public void TC1()
        {
            IWebElement jsAlert = driver.FindElement(By.XPath("//button[contains(.,'Click for JS Alert')]"));
            jsAlert.Click();

            var alert_win = driver.SwitchTo().Alert();
            alert_win.Accept();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement resultTag = wait.Until(e => e.FindElement(By.Id("result")));
            Assert.IsTrue(resultTag.Text.Contains("You successfully clicked an alert"));
        }

        [Test]
        public void TC2()
        {
            IWebElement jsAlert = driver.FindElement(By.XPath("//button[contains(.,'Click for JS Confirm')]"));
            jsAlert.Click();

            var confirm_win = driver.SwitchTo().Alert();
            confirm_win.Dismiss();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement resultTag = wait.Until(e => e.FindElement(By.Id("result")));
            Assert.IsTrue(resultTag.Text.Contains("You clicked: Cancel"));
        }

        [Test]
        public void TC3()
        {
            IWebElement jsAlert = driver.FindElement(By.XPath("//button[contains(.,'Click for JS Confirm')]"));
            jsAlert.Click();

            var confirm_win = driver.SwitchTo().Alert();
            confirm_win.Accept();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement resultTag = wait.Until(e => e.FindElement(By.Id("result")));
            Assert.IsTrue(resultTag.Text.Contains("You clicked: Ok"));
        }

        [TestCase ("This is a test alert message")]
        public void TC4(string alertMsg)
        {
            IWebElement jsAlert = driver.FindElement(By.XPath("//button[contains(.,'Click for JS Prompt')]"));
            jsAlert.Click();

            var alert_win = driver.SwitchTo().Alert();
            alert_win.SendKeys(alertMsg);
            alert_win.Accept();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement resultTag = wait.Until(e => e.FindElement(By.Id("result")));
            Assert.IsTrue(resultTag.Text.Contains("You entered: " + alertMsg));
        }

        [Test]
        public void TC5()
        {
            IWebElement jsAlert = driver.FindElement(By.XPath("//button[contains(.,'Click for JS Prompt')]"));
            jsAlert.Click();

            var alert_win = driver.SwitchTo().Alert();
            alert_win.Dismiss();

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            IWebElement resultTag = wait.Until(e => e.FindElement(By.Id("result")));
            Assert.IsTrue(resultTag.Text.Contains("You entered: null"));
        }

        [OneTimeTearDown]
        public void Close()
        {
           driver.Quit();
        }
    }

    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }
    }
}