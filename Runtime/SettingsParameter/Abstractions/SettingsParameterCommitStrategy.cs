namespace MadeYellow.SettingsParameters.Abstractions
{
    public enum CommitStrategy
    {
        /// <summary>
        /// Writes as soon as values changes. It's best to use it when changes of parameter aren't intense (e.g. not each frame)
        /// </summary>
        AutoCommit,

        /// <summary>
        /// Writes only when Commit() method triggered manually
        /// </summary>
        ManualCommit,
    }
}