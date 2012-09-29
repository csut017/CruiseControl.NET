namespace CruiseControlNet.VisualStudio.Models
{
    using System.ComponentModel;

    /// <summary>
    /// Common functionality for all model classes.
    /// </summary>
    public abstract class ModelBase
        : INotifyPropertyChanged
    {
        #region Public events
        #region PropertyChanged
        /// <summary>
        /// Occurs when a property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
        #endregion

        #region Protected methods
        #region OnPropertyChanged()
        /// <summary>
        /// Called when a property has changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
        #endregion

    }
}