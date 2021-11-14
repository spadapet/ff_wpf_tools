using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfTools
{
    public interface IProgressBar
    {
        IDisposable Begin(TaskData taskData);
        void Cancel();
    }

    public class TaskData : PropertyNotifier
    {
        private string text;
        private readonly Action cancelAction;

        public TaskData(string text, Action cancelAction = null)
        {
            this.text = text ?? string.Empty;
            this.cancelAction = cancelAction;
        }

        public string Text
        {
            get => this.text;
            set => this.SetProperty(ref this.text, value);
        }

        public void Cancel()
        {
            this.cancelAction?.Invoke();
        }
    }

    public sealed class MockProgressBar : IProgressBar, IDisposable
    {
        IDisposable IProgressBar.Begin(TaskData taskData) => this;
        void IProgressBar.Cancel() { }
        void IDisposable.Dispose() { }
    }

    public sealed class TaskProgressBarModel : PropertyNotifier, IProgressBar
    {
        private readonly List<TaskDataWrapper> tasks;

        private sealed class TaskDataWrapper : IDisposable
        {
            public TaskProgressBarModel Owner { get; set; }
            public TaskData TaskData { get; set; }

            public void Dispose()
            {
                if (this.Owner is TaskProgressBarModel owner)
                {
                    this.Owner = null;
                    owner.RemoveTask(this, cancel: false);
                }
            }

            public void Cancel()
            {
                if (this.Owner is TaskProgressBarModel owner)
                {
                    this.Owner = null;
                    owner.RemoveTask(this, cancel: true);
                }
            }
        }

        public TaskProgressBarModel()
        {
            this.tasks = new List<TaskDataWrapper>();

            if (WpfUtility.DesignMode)
            {
                _ = this.Begin(new TaskData("Pretend loading for designer"));
            }
        }

        public bool IsLoading => this.tasks.Count > 0;
        public string LoadingText => this.IsLoading ? this.tasks[this.tasks.Count - 1].TaskData.Text : string.Empty;

        public IDisposable Begin(TaskData taskData)
        {
            TaskDataWrapper info = new TaskDataWrapper()
            {
                TaskData = taskData
            };

            this.PushTask(info);
            return info;
        }

        private void OnTaskPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (string.IsNullOrEmpty(args.PropertyName) || args.PropertyName == nameof(TaskData.Text))
            {
                this.OnPropertyChanged(nameof(this.LoadingText));
            }
        }

        private void PushTask(TaskDataWrapper info)
        {
            info.Owner = this;
            info.TaskData.PropertyChanged += this.OnTaskPropertyChanged;
            this.tasks.Add(info);
            this.OnPropertyChanged(nameof(this.LoadingText));

            if (this.tasks.Count == 1)
            {
                this.OnPropertyChanged(nameof(this.IsLoading));
            }
        }

        private void RemoveTask(TaskDataWrapper info, bool cancel)
        {
            int index = this.tasks.IndexOf(info);
            if (index >= 0)
            {
                this.tasks.RemoveAt(index);
                info.TaskData.PropertyChanged -= this.OnTaskPropertyChanged;

                if (index == this.tasks.Count)
                {
                    this.OnPropertyChanged(nameof(this.LoadingText));
                }

                if (this.tasks.Count == 0)
                {
                    this.OnPropertyChanged(nameof(this.IsLoading));
                }

                if (cancel)
                {
                    info.TaskData.Cancel();
                }
            }
        }

        public void Cancel()
        {
            foreach (TaskDataWrapper info in this.tasks.ToArray())
            {
                info.Cancel();
            }
        }
    }

    public sealed class TaskProgressBarViewModel : PropertyNotifier, IProgressBar
    {
        private TaskProgressBarModel model = new TaskProgressBarModel();
        public TaskProgressBarModel Model
        {
            get => this.model;
            set => this.SetProperty(ref this.model, value);
        }

        private ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get
            {
                if (this.cancelCommand == null)
                {
                    this.cancelCommand = new DelegateCommand(() => this.model.Cancel());
                }

                return this.cancelCommand;
            }
        }

        public IDisposable Begin(TaskData taskData) => this.Model.Begin(taskData);
        public void Cancel() => this.Model.Cancel();
    }

    public sealed partial class TaskProgressBar : UserControl
    {
        public TaskProgressBarViewModel ViewModel { get; }

        public TaskProgressBar()
        {
            this.ViewModel = new TaskProgressBarViewModel();
            this.InitializeComponent();
        }
    }
}
