using MadeYellow.SettingsParameters.Abstractions;
using UnityEngine;

namespace MadeYellow.SettingsParameters
{
    /// <summary>
    /// Implements a <see cref="bool"/> parameter. Think of it as of on/off switch 
    /// </summary>
    /// <remarks>
    /// Will store values as <see cref="int"/> (e.g. 0 or 1)
    /// </remarks>
    [System.Serializable]
    public class BooleanSettingsParameter : SettingsParameterBase<bool>
    {
        public BooleanSettingsParameter(string prefrencesKey,
                                        bool defaultValue = false,
                                        CommitStrategy commitStrategy = CommitStrategy.AutoCommit) : base(prefrencesKey, defaultValue, commitStrategy)
        {
        }

        protected override bool ReadValue(bool defaultValue)
        {
            return Convert(PlayerPrefs.GetInt(Key, Convert(defaultValue)));
        }

        protected override void WriteValue()
        {
            PlayerPrefs.SetInt(Key, Convert(Value));
        }

        private int Convert(bool value)
        {
            return value ? 1 : 0;
        }

        private bool Convert(int value)
        {
            return value == 1 ? true : false;
        }
    }
}
