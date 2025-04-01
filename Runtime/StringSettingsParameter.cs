using MadeYellow.SettingsParameters.Abstractions;
using UnityEngine;

namespace MadeYellow.SettingsParameters
{
    /// <summary>
    /// Implements a <see cref="string"/> parameter. Think of it as of slider
    /// </summary>
    [System.Serializable]
    public class StringSettingsParameter : SettingsParameterBase<string>
    {
        public StringSettingsParameter(string prefrencesKey, string defaultValue = null) : base(prefrencesKey, defaultValue)
        {
        }

        protected override string ReadValue(string defaultValue)
        {
            return PlayerPrefs.GetString(Key, defaultValue);
        }

        protected override void WriteValue()
        {
            PlayerPrefs.SetString(Key, Value);
        }
    }
}
