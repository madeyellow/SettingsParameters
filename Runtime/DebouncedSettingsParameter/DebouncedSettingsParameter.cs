using MadeYellow.SettingsParameters.Abstractions;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace MadeYellow.SettingsParameters
{
    /// <summary>
    /// Allows to write parameter's value only after a particular time span has passed without attempts to change value
    /// </summary>
    /// <remarks>
    /// Warning! This is an experimental feature
    /// </remarks>
    /// <typeparam name="TValue"></typeparam>
    public class DebouncedSettingsParameter<TValue> : ISettingsParameter<TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Key => _parameter.Key;

        /// <summary>
        /// Internal parameter that needs to be debounced
        /// </summary>
        private readonly SettingsParameterBase<TValue> _parameter;

        /// <summary>
        /// Amount of seconds to debounce for
        /// </summary>
        private readonly double _debounceTimeout;

        /// <summary>
        /// A timestamp (in terms of game)
        /// </summary>
        private double _debounceTimestamp;
        private Task _debounceTask;

        /// <summary>
        /// Currently stored value of that particular parameter
        /// </summary>
        public TValue Value { get => _parameter.Value; set => UpdateValue(value); }

        /// <summary>
        /// Invokes when <see cref="Value"/> changes (but not yet saved)
        /// </summary>
        public UnityEvent OnValueChanged => _parameter.OnValueChanged;

        /// <summary>
        /// Invokes when a <see cref="Value"/> is saved
        /// </summary>
        public UnityEvent OnValueCommited => _parameter.OnValueCommited;

        /// <summary>
        /// This is a thing required to sync to the main thread
        /// </summary>
        private SynchronizationContext _mainThreadContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter">A settings parameter to debounce</param>
        /// <param name="debounceTimeout">How many milliseconds must pass from last <see cref="Value"/> update before <see cref="Commit"/> will be triggered</param>
        public DebouncedSettingsParameter(SettingsParameterBase<TValue> parameter, int debounceTimeout)
        {
            _parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));

            if (_parameter.SelectedCommitStrategy != CommitStrategy.ManualCommit)
                throw new ArgumentException(string.Format("Parameter's commit strategy must be set to {0}!", _parameter.SelectedCommitStrategy));

            _debounceTimeout = 1000d / debounceTimeout;

            if (_debounceTimeout <= 0)
                throw new ArgumentOutOfRangeException(string.Format("Debounce timeout must be greater than zero!"));

            _mainThreadContext = SynchronizationContext.Current;
        }

        /// <summary>
        /// Same as calling setter of <see cref="Value"/>. Updates parameter's value and invokes <see cref="OnValueChanged"/> if value was changed.
        /// </summary>
        /// <param name="value"></param>
        public void UpdateValue(TValue value)
        {
            if (_parameter.CheckIfValueIsSame(value))
            {
                return;
            }

            _parameter.Value = value;
            _debounceTimestamp = Time.unscaledTimeAsDouble;

            // Check if debounce task already started. If not - start it

            if (_debounceTask == null || _debounceTask.IsCompleted || _debounceTask.IsCanceled || _debounceTask.IsFaulted)
            {
                _debounceTask = Task.Run(async () => await DebounceHandleAsync(_debounceTimestamp));
            }
        }

        private async Task DebounceHandleAsync(double startTimestamp)
        {
            var currentTimestamp = startTimestamp;

            do
            {
                // Check if deadline (a moment after which we can save value) is already happened
                var deadline = _debounceTimestamp + _debounceTimeout;

                if (currentTimestamp >= deadline)
                {
                    break;
                }

                // Wait until deadline is reached
                var diff = deadline - currentTimestamp;

                await Task.Delay(TimeSpan.FromSeconds(diff));

                currentTimestamp += diff;
            } while (true);

            // When debounce completed 
            DebouncedValue();
        }

        private void DebouncedValue()
        {
            // Sync to the main thread ('cause save methods will work only there)
            _mainThreadContext.Post(new SendOrPostCallback((o) => {
                _parameter.Commit();
            }), null);
        }
    }
}