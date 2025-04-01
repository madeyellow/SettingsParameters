using UnityEngine.Events;
using UnityEngine;

namespace MadeYellow.SettingsParameters.Abstractions
{
    /// <summary>
    /// A base for generic settings paramters, that uses <see cref="PlayerPrefs"/> to read/write parameters
    /// </summary>
    /// <typeparam name="TValue">Type of value this parameter will hold</typeparam>
    public abstract class SettingsParameterBase<TValue>
    {
        /// <summary>
        /// A string that will be used to read&write values from/to <see cref="PlayerPrefs"/>
        /// </summary>
        protected readonly string Key;

        /// <summary>
        /// Actual vale of this particular parameter
        /// </summary>
        private TValue _value;

        /// <summary>
        /// Currently stored value of that particular settings
        /// </summary>
        public TValue Value { get => _value; set => UpdateValue(value); }

        /// <summary>
        /// Invokes when <see cref="Value"/> changes
        /// </summary>
        public UnityEvent OnValueChanged => _onValueChanged;
        private readonly UnityEvent _onValueChanged = new UnityEvent();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefrencesKey">A key that will be used to read/write values with use of <see cref="PlayerPrefs"/></param>
        /// <param name="defaultValue">A value that will be considered as default for that parameter</param>
        public SettingsParameterBase(string prefrencesKey, TValue defaultValue = default)
        {
            if (string.IsNullOrWhiteSpace(prefrencesKey))
            {
                throw new System.ArgumentException($"\"{nameof(prefrencesKey)}\" couldn't be null or empty.", nameof(prefrencesKey));
            }

            Key = prefrencesKey.Trim();

            Value = ReadValue(defaultValue);
        }

        /// <summary>
        /// Same as calling setter of <see cref="Value"/>. Updates parameter's value and invokes event.
        /// </summary>
        /// <param name="newValue"></param>
        /// <remarks>
        /// Note that calling this method will also call some of <see cref="PlayerPrefs"/> Set() methods to store value
        /// </remarks>
        public void UpdateValue(TValue newValue)
        {
            // If values are null or they are equal (comparing by Equals() function)
            if ((newValue == null && Value == null) ||newValue.Equals(Value))
            {
                return;
            }

            // Update value and invoke OnValueChanged event
            _value = newValue;

            _onValueChanged.Invoke();

            // Save value to PlayerPrefs
            WriteValue();
        }

        /// <summary>
        /// Reads value with use of <see cref="PlayerPrefs"/>'s get method or returns provided default value
        /// </summary>
        /// <param name="defaultValue">That value will be returned if there won't be found any stored value for that paramater</param>
        /// <returns></returns>
        protected abstract TValue ReadValue(TValue defaultValue);

        /// <summary>
        /// Writes value with use of <see cref="PlayerPrefs"/>'s set method
        /// </summary>
        protected abstract void WriteValue();
    }
}
