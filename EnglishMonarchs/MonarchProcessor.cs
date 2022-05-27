using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Newtonsoft.Json;

namespace EnglishMonarchs
{
    public interface IMonarchProcessor
    {
        void Run();
        void PrintNoOfMonarchs();
        void PrintLongestRuledMonarch();
        void PrintLongestRuledHouseWithYears();
        void PrintCommonMonarchFirstName();
        IEnumerable<Monarch> GetMonarchs(string monarchSourceUrl);
        void ShowErrorMessageAndExitApplication(string errorText);
    }

    /// <summary>
    /// MonarchProcessor process the monarch data and answers the pre defined questions.
    /// </summary>
    public class MonarchProcessor : IMonarchProcessor
    {
        public IEnumerable<Monarch> AllMonarchs;
        private readonly IConsoleManager _consoleManager;
        private readonly IConfigurationManager _configurationManager;

        public MonarchProcessor(IConsoleManager consoleManager, IConfigurationManager configurationManager)
        {
            _consoleManager = consoleManager;
            _configurationManager = configurationManager;
        }

        public virtual void Run()
        {
            _consoleManager.WriteLine("<------------------------Welcome to Monarch questionnaire app------------------------>\r\n");
            RunInternal();
        }

        private void RunInternal()
        {
            AllMonarchs = GetMonarchs(_configurationManager.GetMonarchSourceUrl());
            if (AllMonarchs == null || !AllMonarchs.Any())
            {
                _consoleManager.ErrorWriteLine("No monarchs found");
                return;
            }
            // Ask the user to choose an command.
            ShowQuestions();
            ProcessCommand();
        }

        private void ShowQuestions()
        {
            _consoleManager.WriteLine("Type a number to find the answer, for instance type 1 and press enter\r\n");
            _consoleManager.WriteLine("\t1. How many monarchs are there in the list?");
            _consoleManager.WriteLine("\t2. Which monarch ruled the longest (and for how long)?");
            _consoleManager.WriteLine("\t3. Which house ruled the longest (and for how long)?");
            _consoleManager.WriteLine("\t4. What was the most common first name?");
            _consoleManager.WriteLine("\t5. Clears the console");
            _consoleManager.WriteLine("\t6. exits the application");
        }

        private void ProcessCommand()
        {
            while (true)
            {
                switch (_consoleManager.ReadLine().ToLower())
                {
                    case "1":
                        PrintNoOfMonarchs();
                        break;
                    case "2":
                        PrintLongestRuledMonarch();
                        break;
                    case "3":
                        PrintLongestRuledHouseWithYears();
                        break;
                    case "4":
                        PrintCommonMonarchFirstName();
                        break;
                    case "0":
                        ShowQuestions();
                        break;
                    case "5":
                        _consoleManager.Clear();
                        RunInternal();
                        break;
                    case "6":
                        Environment.Exit(0);
                        break;
                    default:
                        _consoleManager.WriteLine("Invalid command\r\n");
                        RunInternal();
                        break;
                }
            }
        }

        /// <summary>
        /// Prints the no of monarchs fetched.
        /// </summary>
        public virtual void PrintNoOfMonarchs()
        {
            _consoleManager.WriteLine($"There are '{AllMonarchs.Count()}' monarchs in the list");
        }

        /// <summary>
        /// Prints the longest ruled monarch with years.
        /// </summary>
        public virtual void PrintLongestRuledMonarch()
        {
            var monarchsWithRuledYears = new Dictionary<string, int>();
            AllMonarchs.ToList().ForEach(m =>
            {
                if (monarchsWithRuledYears.ContainsKey(m.MonarchName) || m.RuledYears.Equals("N/A"))
                {
                    return;
                }

                if (!string.IsNullOrEmpty(m.RuledYearsFromAndTo.Item2))
                {
                    monarchsWithRuledYears.Add(m.MonarchName, Convert.ToInt32(m.RuledYearsFromAndTo.Item2) - Convert.ToInt32(m.RuledYearsFromAndTo.Item1));
                }
            });
            var monarchRuledMost = monarchsWithRuledYears.OrderByDescending(yrs => yrs.Value).First();
            _consoleManager.WriteLine($"The monarch that ruled longest is {monarchRuledMost.Key}, who ruled for {monarchRuledMost.Value} years");
        }

        /// <summary>
        /// Prints the longest ruled house with years
        /// </summary>
        public virtual void PrintLongestRuledHouseWithYears()
        {
            var ruledHouseWithYears = new Dictionary<string, int>();
            AllMonarchs.ToList().ForEach(m =>
            {
                if (ruledHouseWithYears.ContainsKey(m.RuledHouse) || m.RuledYears.Equals("N/A"))
                {
                    return;
                }
                var ruledHouses = AllMonarchs.Where(x => m.RuledHouse.Equals(x.RuledHouse, StringComparison.OrdinalIgnoreCase)).ToList();
                var ruledHouse = ruledHouses.First();
                if (ruledHouses.Count == 1)
                {
                    if (ruledHouse.RuledYearsFromAndTo != null)
                    {
                        ruledHouseWithYears.Add(ruledHouse.RuledHouse, Convert.ToInt32(ruledHouse.RuledYearsFromAndTo.Item2) - Convert.ToInt32(ruledHouse.RuledYearsFromAndTo.Item1));
                    }
                }
                else
                {
                    var ruledHouseName = ruledHouse.RuledHouse;
                    var ruledHouseMinYear = ruledHouses.Select(hse => hse.RuledYearsFromAndTo.Item1).Select(yr => Convert.ToInt32(yr)).OrderBy(yr => yr).Min();
                    var ruledHouseMaxYear = ruledHouses.Select(hse => string.IsNullOrEmpty(hse.RuledYearsFromAndTo.Item2) ? hse.RuledYearsFromAndTo.Item1 : hse.RuledYearsFromAndTo.Item2).Select(yr => Convert.ToInt32(yr)).OrderByDescending(yr => yr).Max();
                    ruledHouseWithYears.Add(ruledHouseName, ruledHouseMaxYear - ruledHouseMinYear);

                }

            });
            var houseThatRuledMost = ruledHouseWithYears.OrderByDescending(yrs => yrs.Value).First();
            _consoleManager.WriteLine($"The house that ruled the longest is {houseThatRuledMost.Key}, it ruled for {houseThatRuledMost.Value} years");
        }

        /// <summary>
        /// Prints the common monarch first name
        /// </summary>
        public virtual void PrintCommonMonarchFirstName()
        {
            var allFirstNames = AllMonarchs.Select(m => m.MonarchFirstNameLastName.Item1);
            var commonFirstName = allFirstNames.GroupBy(n => n).OrderByDescending(grp => grp.Count())
                .Select(grp => grp.Key).First();
            _consoleManager.WriteLine($"The most common monarch first name is '{commonFirstName}'");

        }

        /// <summary>
        /// Gets the monarch collection by contacting the online source
        /// </summary>
        /// <param name="monarchSourceUrl"></param>
        /// <returns>Monarchs collections</returns>
        public virtual IEnumerable<Monarch> GetMonarchs(string monarchSourceUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(monarchSourceUrl))
                {
                     ShowErrorMessageAndExitApplication("Cannot find 'MonarchSource' Url in the config file, please verify the Url and try again.\r\n Press enter to exit.");
                     return null;
                }

                var isUrlValid = Uri.TryCreate(monarchSourceUrl, UriKind.Absolute, out Uri uriResult) && uriResult.Scheme == Uri.UriSchemeHttps;
                if (!isUrlValid)
                {
                    ShowErrorMessageAndExitApplication("Invalid 'MonarchSource' Url in the config file, please verify the Url and try again.\r\n Press enter to exit.");
                    return null;
                }

                using WebClient wc = new WebClient();
                var monarchsJson = wc.DownloadString(monarchSourceUrl);
                return DeserializeMonarchJson(monarchsJson);
            }
            catch (Exception e)
            {
                _consoleManager.ErrorWriteLine($"Error occurred while fetching monarch data.\r\nAdditional Information: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deserializes the downloaded monarch json data
        /// </summary>
        /// <param name="monarchsJson"></param>
        /// <returns></returns>
        public virtual IEnumerable<Monarch> DeserializeMonarchJson(string monarchsJson)
        {
            return JsonConvert.DeserializeObject<IEnumerable<Monarch>>(monarchsJson);
        }

        /// <summary>
        /// Shows error message and exits the application.
        /// </summary>
        /// <param name="errorText"></param>
        public virtual void ShowErrorMessageAndExitApplication(string errorText)
        {
            _consoleManager.ErrorWriteLine(errorText);
        }
    }
}
