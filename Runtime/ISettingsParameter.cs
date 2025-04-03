using UnityEngine.Events;

namespace MadeYellow.SettingsParameters
{
    /// <summary>
    /// Most important features of settings parameter
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public interface ISettingsParameter<TValue>
    {
        /// <summary>
        /// Current cached value of a parameter
        /// </summary>
        TValue Value { get; set; }

        /// <summary>
        /// Invokes when parameter's value is saved
        /// </summary>
        UnityEvent OnValueCommited { get; }
    }
}