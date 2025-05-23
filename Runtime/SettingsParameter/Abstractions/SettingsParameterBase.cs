﻿using System;
using UnityEngine;

namespace MadeYellow.SettingsParameters.Abstractions
{
    /// <summary>
    /// A base for generic settings paramters, that uses <see cref="PlayerPrefs"/> to read/write parameters
    /// </summary>
    /// <typeparam name="TValue">Type of value this parameter will hold</typeparam>
    public abstract class SettingsParameterBase<TValue> : ISettingsParameter<TValue>
    {
        /// <summary>
        /// A string that will be used to read&write values from/to <see cref="PlayerPrefs"/>
        /// </summary>
        public readonly string Key;

        /// <summary>
        /// Actual vale of that particular parameter
        /// </summary>
        private TValue _value;

        /// <summary>
        /// Currently cached value of that particular parameter
        /// </summary>
        public TValue Value { get => _value; set => UpdateValue(value); }

        /// <summary>
        /// Indicates if <see cref="Value"/> is changed (and not yet commited)
        /// </summary>
        protected bool IsValueChanged { get; private set; }

        /// <summary>
        /// Invokes when <see cref="Value"/> changes
        /// </summary>
        public event Action OnValueChanged;

        /// <summary>
        /// Invokes when a <see cref="Value"/> is saved via call of <see cref="Commit"/>
        /// </summary>
        /// <remarks>
        /// If you are using <see cref="CommitStrategy.ManualCommit"/> this event != to value being saved!
        /// </remarks>
        public event Action OnValueCommited;

        /// <summary>
        /// Current way of triggering a Write() method
        /// </summary>
        public CommitStrategy SelectedCommitStrategy { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefrencesKey">A key that will be used to read/write values with use of <see cref="PlayerPrefs"/></param>
        /// <param name="defaultValue">A value that will be considered as default for that parameter</param>
        /// <param name="commitStrategy">Dictates when writing must happen (may be usefull to change strategy in some cases)s</param>
        public SettingsParameterBase(string prefrencesKey,
                                     TValue defaultValue = default,
                                     CommitStrategy commitStrategy = CommitStrategy.AutoCommit)
        {
            if (string.IsNullOrWhiteSpace(prefrencesKey))
            {
                throw new ArgumentException($"\"{nameof(prefrencesKey)}\" couldn't be null or empty.", nameof(prefrencesKey));
            }

            Key = prefrencesKey.Trim();

            Value = ReadValue(defaultValue);

            SelectedCommitStrategy = commitStrategy;

            OnCommitStrategyChanged();
        }

        /// <summary>
        /// Same as calling setter of <see cref="Value"/>. Updates parameter's value and invokes <see cref="OnValueChanged"/> if value was changed.
        /// </summary>
        /// <param name="newValue"></param>
        /// <remarks>
        /// Note that calling this method will also call some of <see cref="PlayerPrefs"/> Set() methods to store value
        /// </remarks>
        public void UpdateValue(TValue newValue)
        {
            // If values are null or they are equal (comparing by Equals() function)
            if (CheckIfValueIsSame(newValue))
            {
                return;
            }

            // Update value and invoke OnValueChanged event
            _value = newValue;

            IsValueChanged = true;

            OnValueChanged?.Invoke();
        }

        /// <summary>
        /// Checks if <see cref="Value"/> is changed and triggers a Write() method
        /// </summary>
        public void Commit()
        {
            if (!IsValueChanged)
                return;

            // Save value to PlayerPrefs
            WriteValue();

            IsValueChanged = false;

            OnValueCommited?.Invoke();
        }

        /// <summary>
        /// Returns true if new value aren't different from the<see cref="Value"/>
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns></returns>
        public bool CheckIfValueIsSame(TValue newValue)
        {
            // If values are null or they are equal (comparing by Equals() function)
            return (newValue == null && Value == null) || newValue.Equals(Value);
        }

        /// <summary>
        /// Creates new <see cref="DebouncedSettingsParameter"/> and sends this parameter into it
        /// </summary>
        /// <param name="debounceTimeout">How many milliseconds must pass from last <see cref="Value"/> update before <see cref="Commit"/> will be triggered</param>
        /// <returns></returns>
        public DebouncedSettingsParameter<TValue> ToDebounced(int debounceTimeout)
        {
            return new DebouncedSettingsParameter<TValue>(this, debounceTimeout);
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


        /// <summary>
        /// This method rewires logic if the <see cref="SelectedCommitStrategy"/> get's changed
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        private void OnCommitStrategyChanged()
        {
            OnValueCommited -= Commit;

            switch (SelectedCommitStrategy)
            {
                case CommitStrategy.AutoCommit:
                    OnValueChanged += Commit;
                    break;

                case CommitStrategy.ManualCommit:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(SelectedCommitStrategy), SelectedCommitStrategy, null);
            }
        }
    }
}
