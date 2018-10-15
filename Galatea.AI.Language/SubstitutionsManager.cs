using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Galatea.AI.Abstract;
using Galatea.AI.Characterization;
using Galatea.IO;
using Galatea.Runtime;

namespace Galatea.AI.Language
{
    /// <summary>
    /// Contains all the good algorithms for Language Response.
    /// </summary>
    internal sealed class SubstitutionsManager : RuntimeComponent, IResponseManager
    {
        public SubstitutionsManager(ILanguageAnalyzer languageModel)
        {
            languageModel.Add(this);
        }

        internal void LoadSubstitutions(System.Collections.Specialized.StringCollection substitutionsSettings)
        {
            _substitutions = SubstitutionList.FromSettings(substitutionsSettings);
        }

        #region IResponseManager

        /// <summary>
        /// Gets the system name of the AI Self-Awareness.
        /// </summary>
        string ITemplate.Name
        {
            get { return this.ProviderID; }
        }
        /// <summary>
        /// Gets or sets a string value representing a user-friendly definition 
        /// for the AI.
        /// </summary>
        string ITemplate.FriendlyName
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }
        string IResponseManager.GetResponse(IResponseBag responseBag, ContextNode inputContext)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal string GetResponse(ref string response)
        {
            string globalizedResponse = response;

            foreach (Substitution subst in _substitutions)
            {
                globalizedResponse = globalizedResponse.Replace(subst.ValueToReplace, subst.ReplaceWithValue);
                if (subst.ApplyToOriginalResponse) response.Replace(subst.ValueToReplace, subst.ReplaceWithValue);
            }

            return globalizedResponse;
        }


        //[System.Runtime.InteropServices.ComVisible(false)]
        private class SubstitutionList : KeyedCollection<string, Substitution>
        {
            private SubstitutionList() { }

            internal static SubstitutionList FromSettings(System.Collections.Specialized.StringCollection substitutionsSettings)
            {
                SubstitutionList result = new SubstitutionList();

                foreach (string setting in substitutionsSettings)
                {
                    bool applyToOriginal = false;

                    // Determine apply to original 
                    string[] pair1 = setting.Split('|');
                    if (pair1.Length == 2)
                    {
                        try { applyToOriginal = bool.Parse(pair1[1]); }
                        catch (System.FormatException e) { throw new  SubstitutionsException(setting, 
                            LanguageResources.TextToSpeechSubstitionsError,e); }
                    }

                    string[] pair = pair1[0].Split(',');

                    // Validate
                    if (pair.Length != 2) throw new SubstitutionsException(setting);

                    // Add value pair to List
                    Substitution item = new Substitution(pair[0], pair[1], applyToOriginal);
                    result.Add(item);
                }

                // Finalize
                return result;
            }

            protected override string GetKeyForItem(Substitution item)
            {
                return item.ValueToReplace;
            }
        }
        private struct Substitution
        {
            public Substitution(string valueToReplace, string replaceWithValue, bool applyToOriginalResponse)
            {
                _valueToReplace = valueToReplace;
                _replaceWithValue = replaceWithValue;
                _applyToOriginalResponse = applyToOriginalResponse;
            }

            public string ValueToReplace { get { return _valueToReplace; } }
            public string ReplaceWithValue { get { return _replaceWithValue; } }
            public bool ApplyToOriginalResponse
            {
                get { return _applyToOriginalResponse; }
                set { _applyToOriginalResponse = value; }
            }

            private string _valueToReplace, _replaceWithValue;
            private bool _applyToOriginalResponse;
        }

        private SubstitutionList _substitutions;
    }
}
