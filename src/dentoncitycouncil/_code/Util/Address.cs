using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Util {
    public static class Address {
        private static Dictionary<string, string> _allStates = null;
        public static Dictionary<string, string> AllStates {
            get {
                if (_allStates == null) {
                    _allStates = new Dictionary<string, string>();
                    _allStates.Add("AL", "Alabama");
                    _allStates.Add("AK", "Alaska");
                    _allStates.Add("AZ", "Arizona");
                    _allStates.Add("AR", "Arkansas");
                    _allStates.Add("CA", "California");
                    _allStates.Add("CO", "Colorado");
                    _allStates.Add("CT", "Connecticut");
                    _allStates.Add("DE", "Delaware");
                    _allStates.Add("FL", "Florida");
                    _allStates.Add("GA", "Georgia");
                    _allStates.Add("HI", "Hawaii");
                    _allStates.Add("ID", "Idaho");
                    _allStates.Add("IL", "Illinois");
                    _allStates.Add("IN", "Indiana");
                    _allStates.Add("IA", "Iowa");
                    _allStates.Add("KS", "Kansas");
                    _allStates.Add("KY", "Kentucky");
                    _allStates.Add("LA", "Louisiana");
                    _allStates.Add("ME", "Maine");
                    _allStates.Add("MD", "Maryland");
                    _allStates.Add("MA", "Massachusetts");
                    _allStates.Add("MI", "Michigan");
                    _allStates.Add("MN", "Minnesota");
                    _allStates.Add("MS", "Mississippi");
                    _allStates.Add("MO", "Missouri");
                    _allStates.Add("MT", "Montana");
                    _allStates.Add("NE", "Nebraska");
                    _allStates.Add("NV", "Nevada");
                    _allStates.Add("NH", "New Hampshire");
                    _allStates.Add("NJ", "New Jersey");
                    _allStates.Add("NM", "New Mexico");
                    _allStates.Add("NY", "New York");
                    _allStates.Add("NC", "North Carolina");
                    _allStates.Add("ND", "North Dakota");
                    _allStates.Add("OH", "Ohio");
                    _allStates.Add("OK", "Oklahoma");
                    _allStates.Add("OR", "Oregon");
                    _allStates.Add("PA", "Pennsylvania");
                    _allStates.Add("RI", "Rhode Island");
                    _allStates.Add("SC", "South Carolina");
                    _allStates.Add("SD", "South Dakota");
                    _allStates.Add("TN", "Tennessee");
                    _allStates.Add("TX", "Texas");
                    _allStates.Add("UT", "Utah");
                    _allStates.Add("VT", "Vermont");
                    _allStates.Add("VA", "Virginia");
                    _allStates.Add("WA", "Washington");
                    _allStates.Add("WV", "West Virginia");
                    _allStates.Add("WI", "Wisconsin");
                    _allStates.Add("WY", "Wyoming");
                }
                return _allStates;
            }
        }
    }
}
