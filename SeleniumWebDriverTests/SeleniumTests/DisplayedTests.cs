using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.Extensions;
using Selenium.WebDriver.WaitExtensions.WaitConditions;
using SeleniumExtras.WaitHelpers;

namespace SeleniumTests
{
    /// <summary>
    /// Tests related to element displayedness.
    /// https://www.w3.org/TR/webdriver1/#element-displayedness
    /// </summary>
    public class DisplayedTests
    {
        private IWebDriver driver;

        /// <summary>
        /// Using Displayed element property
        /// </summary>
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void Test0_SingleElementObject(string driverType)
        {
            InitDriver(driverType);
            SearchFor("Selenium displayed element property");

            IWebElement pageNavigationElement = driver.FindElement(By.CssSelector("table#nav"));

            // not in viewport - should be false
            bool beforeScroll = pageNavigationElement.Displayed;

            driver.ExecuteJavaScript("window.scrollTo(0, document.body.scrollHeight)");

            // in viewport - should be true
            bool afterScroll = pageNavigationElement.Displayed;

            // should NOT be equal
            Assert.AreNotEqual(beforeScroll, afterScroll);
        }

        /// <summary>
        /// Using Displayed element property
        /// </summary>
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void Test0_TwoElementObjects(string driverType)
        {
            InitDriver(driverType);
            SearchFor("Selenium displayed element property");

            IWebElement pageNavigationElementBeforeScroll = driver.FindElement(By.CssSelector("table#nav"));

            // not in viewport - should be false
            bool beforeScroll = pageNavigationElementBeforeScroll.Displayed;

            driver.ExecuteJavaScript("window.scrollTo(0, document.body.scrollHeight)");

            // in viewport - should be true
            IWebElement pageNavigationElementAfterScroll = driver.FindElement(By.CssSelector("table#nav"));

            bool afterScroll = pageNavigationElementAfterScroll.Displayed;

            // should NOT be equal
            Assert.AreNotEqual(beforeScroll, afterScroll);
        }

        /// <summary>
        /// Using library Selenium.WebDriver.WaitExtensions: <see cref="ElementWaitConditions"/>
        /// </summary>
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void Test1(string driverType)
        {
            InitDriver(driverType);
            SearchFor("Selenium displayed element property");

            IWebElement pageNavigationElementBeforeScroll = driver.FindElement(By.CssSelector("table#nav"));

            ElementWaitConditions waitConditions = new ElementWaitConditions(pageNavigationElementBeforeScroll, 3);

            // should throw timeout exception
            Assert.Throws<WebDriverTimeoutException>(() => waitConditions.ToBeVisible());
        }

        /// <summary>
        /// Using library DotNetSeleniumExtras.WaitHelpers: <see cref="ExpectedConditions"/>
        /// </summary>
        [Test]
        [TestCase("Chrome")]
        [TestCase("Firefox")]
        public void Test2(string driverType)
        {
            InitDriver(driverType);
            SearchFor("Selenium displayed element property");

            IWebElement pageNavigationElement = ExpectedConditions.ElementIsVisible(By.CssSelector("table#nav")).Invoke(driver);

            // no scrolling performed - element should not be displayed
            Assert.That(pageNavigationElement, Is.Null);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }

        private void InitDriver(string driverType)
        {
            switch (driverType)
            {
                case "Chrome":
                    driver = new ChromeDriver();
                    break;

                case "Firefox":
                    driver = new FirefoxDriver();
                    break;

                default:
                    throw new NotImplementedException("driver not supported for test's purpose");
            }

            driver.Url = "http://www.google.com";

            driver.Manage().Window.Maximize();
        }

        private void SearchFor(string input)
        {
            IWebElement searchInputElement = driver.FindElement(By.Name("q"));

            searchInputElement.SendKeys(input);
            searchInputElement.Submit();
        }
    }
}
