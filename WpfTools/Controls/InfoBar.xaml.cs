using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfTools
{
    public enum InfoLevel
    {
        Message,
        Warning,
        Error,
    }

    public interface IInfoBar
    {
        void SetError(Exception exception, string text = null);
        void SetInfo(InfoLevel level, string text, string details = null, FrameworkElement extraContent = null);
        void Clear();
    }

    public sealed class MockInfoBar : IInfoBar
    {
        void IInfoBar.SetError(Exception exception, string text) { }
        void IInfoBar.SetInfo(InfoLevel level, string text, string details, FrameworkElement extraContent) { }
        void IInfoBar.Clear() { }
    }

    internal sealed class InfoLevelToBrushConverter : ValueConverter
    {
        public Brush MessageBrush { get; set; }
        public Brush WarningBrush { get; set; }
        public Brush ErrorBrush { get; set; }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is InfoLevel level)
            {
                switch (level)
                {
                    case InfoLevel.Message:
                        return this.MessageBrush;

                    case InfoLevel.Warning:
                        return this.WarningBrush;

                    case InfoLevel.Error:
                        return this.ErrorBrush;
                }
            }

            return null;
        }
    }

    public sealed class InfoBarModel : PropertyNotifier, IInfoBar
    {
        public bool HasText => !string.IsNullOrEmpty(this.Text);
        public bool HasDetails => !string.IsNullOrEmpty(this.Details);

        private string text;
        private string details;
        private InfoLevel errorLevel;
        private FrameworkElement extraContent;

        public InfoBarModel()
        {
            this.text = string.Empty;
            this.details = string.Empty;

            if (WpfUtility.DesignMode)
            {
                this.SetError(new Exception("Pretend error for designer"));
            }
        }

        public void SetError(Exception exception, string text = null)
        {
            if (exception is AggregateException ae)
            {
                ae = ae.Flatten();

                exception = (ae.InnerExceptions != null && ae.InnerExceptions.Count == 1)
                    ? ae.InnerExceptions[0]
                    : ae;
            }

            // If the user canceled a task, they don't need to know about it
            if (!(exception is TaskCanceledException) && !(exception is OperationCanceledException))
            {
                this.SetInfo(InfoLevel.Error,
                    string.IsNullOrEmpty(text) ? (exception?.Message ?? string.Empty) : text,
                    exception?.ToString() ?? string.Empty);
            }
        }

        public void SetInfo(InfoLevel level, string text, string details = null, FrameworkElement extraContent = null)
        {
            this.ErrorLevel = level;
            this.Text = text ?? string.Empty;
            this.Details = details ?? string.Empty;
            this.ExtraContent = extraContent;
        }

        public InfoLevel ErrorLevel
        {
            get => this.errorLevel;
            set
            {
                if (this.errorLevel != value)
                {
                    this.errorLevel = value;
                    this.OnPropertyChanged(nameof(this.ErrorLevel));
                }
            }
        }

        public FrameworkElement ExtraContent
        {
            get => this.extraContent;
            set
            {
                if (this.extraContent != value)
                {
                    this.extraContent = value;
                    this.OnPropertyChanged(nameof(this.ExtraContent));
                }
            }
        }

        public string Text
        {
            get => this.text;
            set
            {
                if (!string.Equals(this.text, value ?? string.Empty, StringComparison.Ordinal))
                {
                    this.text = value ?? string.Empty;
                    this.OnPropertyChanged(nameof(this.Text));
                    this.OnPropertyChanged(nameof(this.HasText));
                }
            }
        }

        public string Details
        {
            get => this.details;
            set
            {
                if (!string.Equals(this.details, value ?? string.Empty, StringComparison.Ordinal))
                {
                    this.details = value ?? string.Empty;
                    this.OnPropertyChanged(nameof(this.Details));
                    this.OnPropertyChanged(nameof(this.HasDetails));
                }
            }
        }

        public void Clear()
        {
            this.SetError(null);
        }
    }

    public sealed class InfoBarViewModel : PropertyNotifier, IInfoBar
    {
        private InfoBarModel model = new InfoBarModel();
        public InfoBarModel Model
        {
            get => this.model;
            set => this.SetProperty(ref this.model, value);
        }

        private ICommand clearCommand;
        public ICommand ClearCommand
        {
            get
            {
                if (this.clearCommand == null)
                {
                    this.clearCommand = new DelegateCommand(() => this.model.Clear());
                }

                return this.clearCommand;
            }
        }

        public void SetError(Exception exception, string text) => this.Model.SetError(exception, text);
        public void SetInfo(InfoLevel level, string text, string details = null, FrameworkElement extraContent = null) => this.Model.SetInfo(level, text, details, extraContent);
        public void Clear() => this.Model.Clear();
    }

    public sealed partial class InfoBar : UserControl
    {
        public InfoBarViewModel ViewModel { get; }

        public InfoBar()
        {
            this.ViewModel = new InfoBarViewModel();
            this.InitializeComponent();
        }
    }
}
