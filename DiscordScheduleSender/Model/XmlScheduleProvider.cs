using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DiscordScheduleSender.Model
{
    public class XmlScheduleProvider
    {
        private const string xmlFolder = "SavedSchedules";

        public XmlWeekScheduleData? LoadWeekSchedule(string firstDate)
        {
            CreateFolder(xmlFolder);

            if (!File.Exists(FilePath(firstDate)))
            {
                return null;
            }

            var serializer = new DataContractSerializer(typeof(XmlWeekScheduleData));
            using (var xmlReader = XmlReader.Create(FilePath(firstDate)))
            {
                return (XmlWeekScheduleData?)serializer.ReadObject(xmlReader);
            }
        }

        public void SaveWeekSchedule(XmlWeekScheduleData schedule)
        {
            CreateFolder(xmlFolder);

            var serializer = new DataContractSerializer(typeof(XmlWeekScheduleData));
            var settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;

            using (var xmlWriter = XmlWriter.Create(FilePath(schedule.FirstDay), settings))
            {
                serializer.WriteObject(xmlWriter, schedule);
            }
        }

        public string FileName(string startDate)
        {
            return $"Schedule_{startDate}.xml";
        }

        public string FilePath(string startDate) 
        {
            return Path.Combine(xmlFolder, FileName(startDate));
        }

        private void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
