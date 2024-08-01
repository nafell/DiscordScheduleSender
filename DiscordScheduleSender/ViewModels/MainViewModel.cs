using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DiscordScheduleSender.Discord;
using DiscordScheduleSender.Model;
using DiscordScheduleSender.VIews.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace DiscordScheduleSender.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {

        [ObservableProperty]
        public ObservableCollection<WeekSchedule> weekSchedules;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentScheduleChanged))]
        public WeekSchedule currentWeekSchedule;

        [ObservableProperty]
        public TimeOnly preferredStartTime;

        [ObservableProperty]
        public TimeOnly? preferredEndTime;

        [ObservableProperty]
        public string? botName;

        public DiscordBot DiscordClient { get; }
        public bool IsDiscordBotReady { get; private set; }

        public MainViewModel() //IEnumerable<WeekSchedule> slice 2w prior, 1w after
        {
            var today = DateTime.Now;

            PreferredStartTime = new TimeOnly(21, 30);
            PreferredEndTime = default;

            var searchBackwards = true;
            if (today.DayOfWeek == DayOfWeek.Sunday || today.DayOfWeek == DayOfWeek.Monday)
                searchBackwards = false;

            var candidateWeekSchedule = GetOrCreateWeekSchedule(
                GetFirstDayOfWeek(
                    referenceDate: new DateOnly(today.Year, today.Month, today.Day),
                    targetDayOfWeek: DayOfWeek.Tuesday,
                    searchBackwards: searchBackwards), 
                searchBackwards
                );



            WeekSchedules = new ObservableCollection<WeekSchedule>
            {
                candidateWeekSchedule
            };
            CurrentWeekSchedule = candidateWeekSchedule;

            LoadScheduleFromXml( candidateWeekSchedule );

            DiscordClient = new DiscordBot();
            StartDiscordBot().GetAwaiter().GetResult();
        }

        private async Task StartDiscordBot()
        {
            await DiscordClient.MainAsync();
            Console.WriteLine("started");
            IsDiscordBotReady = true;
        }

        [RelayCommand]
        private async Task LastWeekAsync()
        {
            var today = DateTime.Now - new TimeSpan(7,0,0,0);
            var candidateWeekSchedule = GetOrCreateWeekSchedule(
                GetFirstDayOfWeek(
                    referenceDate: new DateOnly(today.Year, today.Month, today.Day),
                    targetDayOfWeek: DayOfWeek.Tuesday,
                    searchBackwards: false),
                false
                );



            WeekSchedules = new ObservableCollection<WeekSchedule>
            {
                candidateWeekSchedule
            };
            CurrentWeekSchedule = candidateWeekSchedule;

            LoadScheduleFromXml(candidateWeekSchedule);
        }
        [RelayCommand]
        private async Task NextWeekAsync()
        {
            var today = DateTime.Now;
            var candidateWeekSchedule = GetOrCreateWeekSchedule(
                GetFirstDayOfWeek(
                    referenceDate: new DateOnly(today.Year, today.Month, today.Day),
                    targetDayOfWeek: DayOfWeek.Tuesday,
                    searchBackwards: false),
                false
                );

            WeekSchedules = new ObservableCollection<WeekSchedule>
            {
                candidateWeekSchedule
            };
            CurrentWeekSchedule = candidateWeekSchedule;

            LoadScheduleFromXml(candidateWeekSchedule);
        }

        [RelayCommand]
        private async Task ThisWeekAsync()
        {

        }

        [RelayCommand]
        private async Task SendMessageAsync()
        {
            if (!IsDiscordBotReady) 
            {
                await Console.Out.WriteLineAsync("Bot not ready.");
                return;
            }
            var webhook = DiscordClient.DiscordToken?.WebhookUrls[1268547692850712607];

            if (webhook == null)
            {
                await Console.Out.WriteLineAsync("Webhook not found.");
                return;
            }

            var str = CurrentWeekSchedule.DiscordMessageText;

            ulong messageId = 0;
            try
            {
                messageId = CurrentWeekSchedule.DiscordChannelMessageIdPair[1248923247685402646];
            }
            catch { }

            if (messageId == 0)
            {
                messageId = await DiscordClient.SendDiscordWebhook(webhook, str);
            }
            else
            {
                await DiscordClient.ModifyDiscordWebhook(webhook, str, messageId);
            }

            CurrentWeekSchedule.DiscordChannelMessageIdPair[1248923247685402646] = messageId;

            //save to xml
            SaveScheduleToXml(CurrentWeekSchedule);
        }

        [RelayCommand]
        private async Task LoadScheduleAsync()
        {
            LoadScheduleFromXml(CurrentWeekSchedule);
        }

        private void SaveScheduleToXml(WeekSchedule schedule)
        {
            if (schedule == null)
                return;

            var xml = new XmlScheduleProvider();
            var xmlSchedule = new XmlWeekScheduleData();

            var timeConverter = new TimeOnlyConverter();
            var dateConverter = new DateOnlyConverter();

            xmlSchedule.FirstDay = dateConverter.XmlConvert(schedule.FirstDay);
            xmlSchedule.DiscordChannelMessageIdPair = schedule.DiscordChannelMessageIdPair;

            xmlSchedule.Appointments = new List<XmlAppointmentData>();

            foreach (var appointment in schedule.Appointments)
            {
                var xmlAppointment = new XmlAppointmentData();
                xmlAppointment.Date = dateConverter.XmlConvert(appointment.Date);
                xmlAppointment.StartTime = timeConverter.Convert(appointment.StartTime);
                xmlAppointment.EndTime = timeConverter.Convert(appointment.EndTime);
                xmlAppointment.Objective = appointment.Objective;
                xmlAppointment.Note = appointment.Note;
                xmlAppointment.TimeSlotOfDay = appointment.TimeSlotOfDay;
                xmlAppointment.IsScheduled = appointment.IsScheduled;
                xmlAppointment.IsCanceled = appointment.IsCanceled;
                xmlSchedule.Appointments.Add(xmlAppointment);
            }
            xml.SaveWeekSchedule(xmlSchedule);
        }

        private void LoadScheduleFromXml(WeekSchedule schedule)
        {
            var timeConverter = new TimeOnlyConverter();
            var dateConverter = new DateOnlyConverter();

            var xml = new XmlScheduleProvider();
            var xmlSchedule = xml.LoadWeekSchedule(dateConverter.XmlConvert(schedule.FirstDay));
            if (xmlSchedule == null)
                return;


            schedule.FirstDay = dateConverter.XmlConvertBack(xmlSchedule.FirstDay);
            schedule.DiscordChannelMessageIdPair = xmlSchedule.DiscordChannelMessageIdPair;

            schedule.Appointments = new ObservableCollection<Appointment>();
            foreach (var appointment in xmlSchedule.Appointments)
            {
                var appointmentViewModel = new Appointment(
                    schedule.changedNotification,
                     dateConverter.XmlConvertBack(appointment.Date),
                     timeConverter.ConvertBack(appointment.StartTime) ?? new TimeOnly(),
                     timeConverter.ConvertBack(appointment.EndTime),
                     appointment.Objective,
                     appointment.Note,
                     appointment.TimeSlotOfDay,
                     appointment.IsScheduled
                    );
                schedule.Appointments.Add(appointmentViewModel);
            }
        }

        private bool CurrentScheduleChanged
        {
            get
            {
                LoadScheduleFromXml(CurrentWeekSchedule);

                return true;
            }
        }

        private WeekSchedule GetOrCreateWeekSchedule(DateOnly firstDay, bool searchBackwards)
        {
            try
            {
                var existing = WeekSchedules.FirstOrDefault(s => s.FirstDay == firstDay);

                if (existing != null)
                {
                    return existing;
                }
            }
            catch (Exception ex) { }


            var weekSchedule = new WeekSchedule(
                firstDay,
                PreferredStartTime,
                PreferredEndTime
                );

            return weekSchedule;
        }

        private DateOnly GetFirstDayOfWeek(DateOnly referenceDate, DayOfWeek targetDayOfWeek, bool searchBackwards, bool includeReferenceDate = true)
        {
            var step = searchBackwards ? -1 : 1;

            if (!includeReferenceDate)
            {
                referenceDate = referenceDate.AddDays(step);
            }

            for (var day = 0; day < 7; day++) 
            {
                if (referenceDate.DayOfWeek == targetDayOfWeek)
                    return referenceDate;

                referenceDate = referenceDate.AddDays(step);
            }

            throw new InvalidOperationException("The fabric of timespace itself has been disrupted. Please call the Doctor.");
        }


    }
}
