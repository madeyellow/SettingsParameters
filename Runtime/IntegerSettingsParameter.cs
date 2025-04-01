using MadeYellow.SettingsParameters.Abstractions;
using UnityEngine;

namespace MadeYellow.SettingsParameters
{
    /// <summary>
    /// Implements a <see cref="int"/> parameter. Think of it as of sliderm but with fixed step
    /// </summary>
    [System.Serializable]
    public class IntegerSettingsParameter : SettingsParameterBase<int>
    {
        public IntegerSettingsParameter(string prefrencesKey, int defaultValue = 0) : base(prefrencesKey, defaultValue)
        {
        }

        protected override int ReadValue(int defaultValue)
        {
            return PlayerPrefs.GetInt(Key, defaultValue);
        }

        protected override void WriteValue()
        {
            PlayerPrefs.SetInt(Key, Value);
        }
    }
}
