using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordScheduleSender.ViewModels
{
    public partial class Appointment : ObservableObject
    {
        public Appointment(Action parentChangedNotification, DateOnly date, TimeOnly startTime = default, TimeOnly? endTime = default, string? objective = default, string? note = default, TimeSlot timeSlotOfDay = default, bool isScheduled = false)
        {
            _parentChangedNotification = parentChangedNotification;
            this.date = date;
            this.startTime = startTime;
            this.endTime = endTime;
            this.objective = objective;
            this.note = note;
            this.timeSlotOfDay = timeSlotOfDay;
            this.isScheduled = isScheduled;
        }

        private Action _parentChangedNotification;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParentNotification))]
        public DateOnly date;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParentNotification))]
        public TimeOnly startTime;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParentNotification))]
        public TimeOnly? endTime;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParentNotification))]
        public string? objective;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParentNotification))]
        public string? note;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParentNotification))]
        public TimeSlot timeSlotOfDay;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParentNotification))]
        public bool isScheduled;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ParentNotification))]
        public bool isCanceled;

        //Hack: notify the parent ViewModel that content modification occured.
        public bool ParentNotification
        {
            get
            {
                _parentChangedNotification.Invoke();

                return true;
            }
        }
    }

    public enum TimeSlot
    {
        朝,
        午前,
        昼,
        午後,
        夕方,
        夜,
        深夜
    }
}
