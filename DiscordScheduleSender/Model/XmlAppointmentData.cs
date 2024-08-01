using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Xml;
using CommunityToolkit.Mvvm.ComponentModel;
using DiscordScheduleSender.ViewModels;

namespace DiscordScheduleSender.Model
{
    [DataContract]
    public class XmlAppointmentData
    {
        [DataMember]
        public string Date;

        [DataMember]
        public string StartTime;

        [DataMember]
        public string? EndTime;

        [DataMember]
        public string? Objective;

        [DataMember]
        public string? Note;

        [DataMember]
        public TimeSlot TimeSlotOfDay;

        [DataMember]
        public bool IsScheduled;

        [DataMember]
        public bool IsCanceled;
    }
}
