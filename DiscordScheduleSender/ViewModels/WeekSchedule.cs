using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordScheduleSender.ViewModels
{
    public partial class WeekSchedule : ObservableObject
    {
        static CultureInfo Japanese = new CultureInfo("ja-JP");


        public Dictionary<long, ulong>? DiscordChannelMessageIdPair { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(LastDay))]
        public DateOnly firstDay;
        public DateOnly LastDay => FirstDay.AddDays(6);

        //Hack: recieves notification from every appointment for content modification.
        //consider use of IMessanger (conventional solution)
        //[ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(DiscordMessageText))]
        //public bool appointmentContentChangedReciever;
        public void changedNotification()
        {
            //AppointmentContentChangedReciever = !appointmentContentChangedReciever;
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(DiscordMessageText)));
        }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(AppointmentCount))]
        [NotifyPropertyChangedFor(nameof(DiscordMessageText))]
        public ObservableCollection<Appointment>? appointments;
        public int AppointmentCount => Appointments?.Count(a => a.IsScheduled) ?? 0;
        public string DiscordMessageText
        { 
            get
            {
                if (Appointments == null)
                    return string.Empty;

                var message = new StringBuilder();
                foreach (var appointment in appointments)
                {
                    if (!appointment.IsScheduled)
                        continue;

                    if (appointment.IsCanceled)
                        message.Append("~~");

                    message.Append("▼");
                    message.Append(appointment.date.ToMonthDayJapaneseString());
                    message.Append(" ");
                    message.Append(appointment.startTime.ToHourMinuteString());
                    message.Append("～");
                    if (appointment.EndTime.HasValue)
                    {
                        message.Append(appointment.endTime.Value.ToHourMinuteString());
                    }
                    message.Append("　");
                    message.AppendLine(appointment.note);

                    if (!string.IsNullOrEmpty(appointment.objective))
                    {
                        message.AppendLine(appointment.objective);
                    }

                    if (appointment.IsCanceled)
                        message.Append("~~");

                    message.AppendLine();
                }

                return message.ToString();
            } 
        }

        public WeekSchedule(DateOnly firstDayOfWeek, TimeOnly preferredStartTime = default, TimeOnly? preferredEndTime = default) 
        {
            FirstDay = firstDayOfWeek;

            DiscordChannelMessageIdPair = new Dictionary<long, ulong>();

            appointments = new ObservableCollection<Appointment>();
            FillMissingDays(firstDayOfWeek, preferredStartTime, preferredEndTime);

        }

        private void FillMissingDays(DateOnly firstDayOfWeek, TimeOnly preferredStartTime = default, TimeOnly? preferredEndTime = default)
        {
            for (var day = 0; day < 7; day++)
            {
                var date = firstDayOfWeek.AddDays(day);
                if (Appointments.Any(a => a.Date == date))
                {
                    continue;
                }
                Appointments.Add(new Appointment(changedNotification, firstDayOfWeek.AddDays(day), preferredStartTime, preferredEndTime));
            }
        }


        public override string ToString()
        {
            return $"{FirstDay.ToMonthDayJapaneseString()} ～ {LastDay.ToMonthDayJapaneseString()}";
        }
    }
}
