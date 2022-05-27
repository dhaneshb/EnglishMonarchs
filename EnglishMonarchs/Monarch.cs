using Newtonsoft.Json;

using System;

namespace EnglishMonarchs
{
    /// <summary>
    /// Each property is decorated with a json property, so it deserializes the member with the specified name.
    /// Since the ruled years and monarch name are in combined format, the properties MonarchName and Ruled years process them and stores the well formatted data into local fields so it will be
    /// beneficial when answering the monarch questionnaire
    /// </summary>
    public class Monarch
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        private string _monarchName;

        [JsonProperty("nm")]
        public string MonarchName
        {
            get => _monarchName;
            set
            {
                _monarchName = value;
                if (!string.IsNullOrEmpty(_monarchName))
                {
                    var monarchFullName = _monarchName.Split(' ', 2);
                    MonarchFirstNameLastName = monarchFullName.Length > 1 ? new Tuple<string, string>(monarchFullName[0], monarchFullName[1]) : new Tuple<string, string>(_monarchName, "");
                }
            }
        }

        public Tuple<string, string> MonarchFirstNameLastName;

        [JsonProperty("cty")]
        public string Country { get; set; }

        [JsonProperty("hse")]
        public string RuledHouse { get; set; }


        private string _ruledYears;

        [JsonProperty("yrs")]
        public string RuledYears
        {
            get => _ruledYears;
            set
            {
                _ruledYears = value;
                if (!string.IsNullOrEmpty(_ruledYears))
                {
                    var ruledYrsSplit = _ruledYears.Split('-', 2);
                    if (ruledYrsSplit.Length > 1)
                    {
                        if (string.IsNullOrEmpty(ruledYrsSplit[0]))
                        {
                            //Handle the rotten ruled years by setting N/A;
                            _ruledYears = "N/A";
                            return;
                        }
                        var ruledYearUntil = string.IsNullOrEmpty(ruledYrsSplit[1]) ? DateTime.Now.Year.ToString() : ruledYrsSplit[1];
                        RuledYearsFromAndTo = new Tuple<string, string>(ruledYrsSplit[0], ruledYearUntil);
                    }
                    else
                    {
                        //The ruled period is within one year
                        RuledYearsFromAndTo = new Tuple<string, string>(_ruledYears, _ruledYears);
                    }
                }
            }
        }

        public Tuple<string, string> RuledYearsFromAndTo;
    }
}
