using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Xml;

namespace DiscordScheduleSender.Model
{
    [DataContract]
    public class XmlWeekScheduleData
    {
        [DataMember]
        public string FirstDay;

        [DataMember]
        public Dictionary<long, ulong> DiscordChannelMessageIdPair;

        [DataMember]
        public List<XmlAppointmentData> Appointments;
    }
}
