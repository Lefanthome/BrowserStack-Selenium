using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace UnitTestBrowserStack
{
    [TestClass]
    public class UnitTest1
    {
        IWebDriver _driver;

        [TestInitialize]
        public void StartDriver()
        {
            //Configure Driver
            var capabilities = new DesiredCapabilities();

            capabilities.SetCapability("browser", "IE");
            capabilities.SetCapability("browser_version", "11.0");
            capabilities.SetCapability("os", "Windows");
            capabilities.SetCapability("os_version", "10");
            capabilities.SetCapability("resolution", "1600x1200");

            //Ajout de la capture
            capabilities.SetCapability("browserstack.debug", true);

            //Nom du projet
            capabilities.SetCapability("project", "Softfluent.fr");

            //Configuration BrowserStack
            capabilities.SetCapability("browserstack.user", "USER");
            capabilities.SetCapability("browserstack.key", "KEY");

            _driver = new RemoteWebDriver(new Uri("http://hub.browserstack.com/wd/hub/"), capabilities);
            _driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(2));
            // This will maximize your window to 100%
            _driver.Manage().Window.Maximize();
        }

        [TestCleanup]
        public void DriverCleanup()
        {
            if(_driver != null)
            {
                _driver.Quit();
            }
        }

        [TestMethod]
        public void TestScenarioNavigation()
        {
            //1 - Aller sur la page d’accueil: http://www.softfluent.fr/
            _driver.Navigate().GoToUrl("http://www.softfluent.fr/");
            
            //2 - Vérification de l'url
            Assert.AreEqual(_driver.Url, "http://www.softfluent.fr/", "Url is not the same");
            
            //3 - Ensuite clic sur le lien « Offres & outils » : 
            _driver.FindElement(By.XPath("//a[contains(@href, 'offres')]")).Click();
            WaitForPageToLoad(_driver);

            //4 - On sélectionne ensuite l’outil: RowShare
            _driver.FindElement(By.XPath("//a[contains(@href, 'offres/rowshare')]")).Click();
            WaitForPageToLoad(_driver);

            //5 - On va chercher dans la page RowShare : L’organisation à plusieurs devient simple !
            var text = _driver.FindElement(By.XPath("//div[@id='tools']/div/div/div/h2")).Text;
            Assert.AreEqual(text, "L’organisation à plusieurs devient simple !", "Value is not the same.");
        }

        public  void WaitForPageToLoad(IWebDriver driver)
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10.00));

            var javascript = driver as IJavaScriptExecutor;
            if (javascript == null)
                throw new ArgumentException("driver", "Driver must support javascript execution");

            wait.Until((d) =>
            {
                try
                {
                    string readyState = javascript.ExecuteScript("if (document.readyState) return document.readyState;").ToString();
                    return readyState.ToLower() == "complete";
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            });
        }

    }
}
