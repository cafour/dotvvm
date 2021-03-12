﻿using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using DotVVM.Samples.Tests.Base;
using DotVVM.Testing.Abstractions;
using Riganti.Selenium.Core;
using Riganti.Selenium.Core.Abstractions;
using Xunit;
using Xunit.Abstractions;

namespace DotVVM.Samples.Tests.Feature
{
    public class ViewModuleTests : AppSeleniumTest
    {
        public ViewModuleTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void Feature_ViewModules_ModuleInMarkupControl()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ViewModules_ModuleInMarkupControl);
                browser.Wait();

                var log = browser.Single("#log");
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule: init"), 8000);

                var moduleButtons = browser.FindElements("input[type=button]");
                var incrementValue = browser.First(".increment-value");
                var result = browser.First(".named-command-result");
                TestModule(browser, log, moduleButtons, incrementValue, result, "testViewModule");
            });
        }

        [Fact]
        public void Feature_ViewModules_ModuleInMarkupControlTwice()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ViewModules_ModuleInMarkupControlTwice);
                browser.Wait();

                var log = browser.Single("#log");
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule: init"), 8000);

                var toggleButton = browser.Single(".toggle input[type=button]");

                // test first instance
                var moduleButtons = browser.FindElements(".control1 input[type=button]");
                var incrementValue = browser.First(".control1 .increment-value");
                var result = browser.First(".control1 .named-command-result");
                TestModule(browser, log, moduleButtons, incrementValue, result, "testViewModule");

                // show second instance
                toggleButton.Click();
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule: init"), 8000);

                // test second instance
                moduleButtons = browser.FindElements(".control2 input[type=button]");
                incrementValue = browser.First(".control2 .increment-value");
                result = browser.First(".control2 .named-command-result");
                TestModule(browser, log, moduleButtons, incrementValue, result, "testViewModule");

                // hide second instance
                toggleButton.Click();
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule: dispose"), 8000);

                // show second instance
                toggleButton.Click();
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule: init"), 8000);
            });
        }

        [Fact]
        public void Feature_ViewModules_ModuleInPage()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ViewModules_ModuleInPage);
                browser.Wait();

                var log = browser.Single("#log");
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule: init"), 8000);

                var moduleButtons = browser.FindElements("input[type=button]");
                var incrementValue = browser.First(".increment-value");
                var result = browser.First(".named-command-result");
                TestModule(browser, log, moduleButtons, incrementValue, result, "testViewModule");
            });
        }

        [Fact]
        public void Feature_ViewModules_ModuleInPageCommandAmbiguous()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ViewModules_ModuleInPageCommandAmbiguous);
                browser.Wait();

                var log = browser.Single("#log");
                browser.WaitFor(() => AssertLogEntry(log, "testViewModule: init"), 8000);
                browser.WaitFor(() => AssertLogEntry(log, "testViewModule2: init"), 8000);

                browser.First("input[type=button]").Click();
                browser.Wait(8000);
                AssertUI.InnerText(log, t => !t.Contains("testViewModule: commands.noArgs()"));
                AssertUI.InnerText(log, t => !t.Contains("testViewModule2: commands.noArgs()"));
            });
        }

        [Fact]
        public void Feature_ViewModules_ModuleInPageMasterPage()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ViewModules_ModuleInPageMasterPage);
                browser.Wait();

                var log = browser.Single("#log");
                browser.WaitFor(() => AssertLogEntry(log, "testViewModule: init"), 8000);
                browser.WaitFor(() => AssertLogEntry(log, "testViewModule2: init"), 8000);

                var moduleButtons = browser.FindElements(".master input[type=button]");
                var incrementValue = browser.First(".master .increment-value");
                var result = browser.First(".master .named-command-result");
                TestModule(browser, log, moduleButtons, incrementValue, result, "testViewModule");

                moduleButtons = browser.FindElements(".page input[type=button]");
                incrementValue = browser.First(".page .increment-value");
                result = browser.First(".page .named-command-result");
                TestModule(browser, log, moduleButtons, incrementValue, result, "testViewModule2");
            });
        }

        [Fact]
        public void Feature_ViewModules_ModuleInPageSpaMasterPage()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ViewModules_ModuleInPageSpaMasterPage2);
                browser.Wait();

                var links = browser.FindElements("a");

                var log = browser.Single("#log");
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule: init"), 8000);
                browser.Wait(8000);
                AssertUI.InnerText(log, t => !t.Contains("testViewModule2: init"));

                links[0].Click();
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule2: init"), 8000);

                var moduleButtons = browser.FindElements(".master input[type=button]");
                var incrementValue = browser.First(".master .increment-value");
                var result = browser.First(".master .named-command-result");
                TestModule(browser, log, moduleButtons, incrementValue, result, "testViewModule");

                moduleButtons = browser.FindElements(".page input[type=button]");
                incrementValue = browser.First(".page .increment-value");
                result = browser.First(".page .named-command-result");
                TestModule(browser, log, moduleButtons, incrementValue, result, "testViewModule2");

                links[1].Click();
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule2: dispose"), 8000);

                links[0].Click();
                browser.WaitFor(() => AssertLastLogEntry(log, "testViewModule2: init"), 8000);
            });
        }

        private void TestModule(IBrowserWrapper browser, IElementWrapper log, IElementWrapperCollection<IElementWrapper, IBrowserWrapper> moduleButtons, IElementWrapper incrementValue, IElementWrapper result, string prefix)
        {
            moduleButtons[0].Click();
            browser.WaitFor(() => AssertLastLogEntry(log, prefix + ": commands.noArgs()"), 8000);
            moduleButtons[1].Click();
            browser.WaitFor(() => AssertLastLogEntry(log, prefix + ": commands.oneArg(10)"), 8000);
            moduleButtons[2].Click();
            browser.WaitFor(() => AssertLastLogEntry(log, prefix + @": commands.twoArgs(10, {""Test"":""Hello"",""$type"":""4wHojaMvtyXNR6aMRsZ4cWanOvA=""})"), 8000);

            AssertUI.InnerTextEquals(incrementValue, "0");
            moduleButtons[3].Click();
            browser.WaitFor(() => AssertLastLogEntry(log, prefix + ": commands.syncIncrement(0)"), 8000);
            AssertUI.InnerTextEquals(incrementValue, "1");
            moduleButtons[4].Click();
            browser.WaitFor(() => AssertLastLogEntry(log, prefix + ": commands.asyncIncrement(1) begin"), 8000);
            browser.WaitFor(() => AssertLastLogEntry(log, prefix + ": commands.asyncIncrement(1) end"), 8000);
            AssertUI.InnerTextEquals(incrementValue, "2");
            moduleButtons[5].Click();
            browser.WaitFor(() => AssertLastLogEntry(log, prefix + ": commands.callIncrementCommand(2)"), 8000);
            AssertUI.InnerTextEquals(incrementValue, "3");

            moduleButtons[6].Click();
            browser.WaitFor(() => AssertLastLogEntry(log, prefix + ": commands.callSetResultCommand()"), 8000);
            AssertUI.InnerTextEquals(result, "1_test_abc");
        }


        private void AssertLastLogEntry(IElementWrapper log, string entry)
        {
            AssertUI.InnerText(log, t => t.Substring(t.LastIndexOf("\n") + 1).Contains(entry));
        }
        private void AssertLogEntry(IElementWrapper log, string entry)
        {
            AssertUI.InnerText(log, t => t.Contains(entry));
        }



        [Fact]
        public void Feature_ViewModules_IncrementerInRepeater()
        {
            RunInAllBrowsers(browser => {
                browser.NavigateToUrl(SamplesRouteUrls.FeatureSamples_ViewModules_IncrementerInRepeater);
                browser.Wait();

                var buttons = browser.FindElements("input[type=button]");

                void EnsureId(IElementWrapper inc, string id)
                {
                    browser.WaitFor(() => {
                        AssertUI.TextEquals(inc.Single(".id"), id);
                    }, 500);
                }
                void EnsureValue(IElementWrapper inc, string value)
                {
                    AssertUI.TextEquals(inc.Single(".value"), value);
                }

                // validate states
                var incrementers = browser.FindElements(".incrementer").ThrowIfDifferentCountThan(2);
                EnsureId(incrementers[0], "c23_0_incrementer");
                EnsureValue(incrementers[0], "0");
                EnsureId(incrementers[1], "c23_1_incrementer");
                EnsureValue(incrementers[1], "0");

                // increment
                incrementers[0].ElementAt("a", 0).Click();
                incrementers[1].ElementAt("a", 0).Click();
                incrementers[1].ElementAt("a", 0).Click();
                EnsureValue(incrementers[0], "1");
                EnsureValue(incrementers[1], "2");

                // report state
                incrementers[0].ElementAt("a", 1).Click();
                AssertUI.TextEquals(browser.Single(".reportedState"), "1");
                incrementers[1].ElementAt("a", 1).Click();
                AssertUI.TextEquals(browser.Single(".reportedState"), "2");

                // add incrementer
                buttons[0].Click();
                
                // state must be persisted, ids have changed
                incrementers = browser.FindElements(".incrementer").ThrowIfDifferentCountThan(3);
                EnsureId(incrementers[0], "c23_0_incrementer");
                EnsureValue(incrementers[0], "0");
                EnsureId(incrementers[1], "c23_1_incrementer");
                EnsureValue(incrementers[1], "1");
                EnsureId(incrementers[2], "c23_2_incrementer");
                EnsureValue(incrementers[2], "2");

                // report state
                incrementers[0].ElementAt("a", 1).Click();
                AssertUI.TextEquals(browser.Single(".reportedState"), "0");
                incrementers[1].ElementAt("a", 1).Click();
                AssertUI.TextEquals(browser.Single(".reportedState"), "1");
                incrementers[2].ElementAt("a", 1).Click();
                AssertUI.TextEquals(browser.Single(".reportedState"), "2");

                // remove incrementer
                buttons[1].Click();

                // state must be persisted, ids have changed
                incrementers = browser.FindElements(".incrementer").ThrowIfDifferentCountThan(2);
                EnsureId(incrementers[0], "c23_0_incrementer");
                EnsureValue(incrementers[0], "1");
                EnsureId(incrementers[1], "c23_1_incrementer");
                EnsureValue(incrementers[1], "2");

                // report state
                incrementers[0].ElementAt("a", 1).Click();
                AssertUI.TextEquals(browser.Single(".reportedState"), "1");
                incrementers[1].ElementAt("a", 1).Click();
                AssertUI.TextEquals(browser.Single(".reportedState"), "2");

                // remove incrementers
                buttons[1].Click();
                buttons[1].Click();
                
                browser.FindElements(".incrementer").ThrowIfDifferentCountThan(0);

                // re-add them and check that everything works
                buttons[0].Click();

                // validate id and state
                incrementers = browser.FindElements(".incrementer").ThrowIfDifferentCountThan(1);
                EnsureId(incrementers[0], "c23_0_incrementer");
                EnsureValue(incrementers[0], "0");

                // report state
                incrementers[0].ElementAt("a", 0).Click();
                EnsureValue(incrementers[0], "1");
            });

        }
    }
}
