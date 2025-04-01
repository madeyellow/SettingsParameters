using MadeYellow.SettingsParameters.Abstractions;
using UnityEngine;

namespace MadeYellow.SettingsParameters
{
    /// <summary>
    /// Implements a <see cref="float"/> parameter. Think of it as of slider
    /// </summary>
    [System.Serializable]
    public class FloatSettingsParameter : SettingsParameterBase<float>
    {
        public FloatSettingsParameter(string prefrencesKey, float defaultValue = 0f) : base(prefrencesKey, defaultValue)
        {
        }

        protected override float ReadValue(float defaultValue)
        {
            return PlayerPrefs.GetFloat(Key, defaultValue);
        }

        protected override void WriteValue()
        {
            PlayerPrefs.SetFloat(Key, Value);
        }
    }
}
