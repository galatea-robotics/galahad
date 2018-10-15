using System;
using System.Collections.Generic;
using System.Text;

namespace Galatea.AI.Language
{
    using Galatea.AI.Characterization;

    internal static class FeedbackTokenManager
    {
        private static UserFeedbackTemplates feedbackTemplates = UserFeedbackTemplates.Default;
        private static Dictionary<string, List<string>> feedbackTokens = GetFeedbackTokens();
        private static Dictionary<string, List<string>> GetFeedbackTokens()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            string[] feedbackResources = LanguageResources.Tokens_Feedback_Templates.Split(';');
            foreach (string resource in feedbackResources)
            {
                if (string.IsNullOrEmpty(resource)) continue;

                string[] tokens = resource.Trim().Split(':');
                string key = tokens[0].Trim();
                string[] valueTokens = tokens[1].Trim().Split(',');
                List<string> value = new List<string>(valueTokens);
                result.Add(key, value);
            }

            return result;
        }

        public static bool CheckInputTokensForUserFeedback(List<string> inputTokens, ref UserFeedback userFeedback)
        {
            bool result = false;

            if (inputTokens.Contains("!")) userFeedback = UserFeedbackTemplates.Meh;    // Slightly positive
            // TODO:  Adjust feedback if ALL CAPS lol
            else userFeedback = UserFeedbackTemplates.Neutral;

            for (int i = 0; i < inputTokens.Count; i++)
            {
                string it = inputTokens[i];

                // Does the Input contain Feedback (positive or negative response)?
                foreach (UserFeedback feedback in feedbackTemplates)
                {
                    // Compare to UserFeedbackTemplates Collection
                    if (string.Equals(feedback.Name, it, StringComparison.CurrentCultureIgnoreCase))
                    {
                        result = true;
                    }
                    else
                    {
                        // Compare to Language Resources property
                        if (feedbackTokens.ContainsKey(feedback.Name))
                        {
                            // TODO:  Implement within TemplateRelationship design framework
                            foreach(string fb in feedbackTokens[feedback.Name])
                            {
                                if (string.Equals(it, fb.Trim(), StringComparison.CurrentCultureIgnoreCase))
                                {
                                    result = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (result)
                    {
                        userFeedback = feedback;
                        inputTokens.Remove(it);
                        return true;
                    }
                }
            }

            return result;
        }
    }
}