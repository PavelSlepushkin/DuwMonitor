using System;
using System.Collections.Generic;

namespace DuwMonitorWebJob
{
    /// <summary>
    /// Duw - Dolnośląski Urząd Wojewódzki
    /// </summary>
    public class Duw
    {
        private readonly IDuwStorage _duwStorage;
        private readonly IMailSender _mailSender;

        public Duw(IDuwStorage duwStorage, IMailSender mailSender)
        {
            _duwStorage = duwStorage;
            _mailSender = mailSender;
        }

        public KeyValuePair<DateTime, bool>[] GetInfoForNext15Days()
        {
            var targetDates = GetDatesForVerification();
            var result = new KeyValuePair<DateTime, bool>[targetDates.Length];

            for (var i = 0; i < targetDates.Length; i++)
            {
                var date = targetDates[i];
                try
                {
                    var data = _duwStorage.ReadDataByDate(date);
                    result[i] = new KeyValuePair<DateTime, bool>(date, HasFreeSlots(data));
                }
                catch (Exception ex)
                {
                    _mailSender.SendEmail(new Message
                    {
                        Body = $"Exception occured during getting info for {date}. Exception: {ex}",
                        Recepient = "dima.oleshchenko@outlook.com",
                        Sender = "duwmonitor@monitoring.com",
                        Subject = $"EXCEPTION OCCURED {DateTime.Now.ToString("g")}"
                    });
                    break;
                }
            }
            return result;
        }

        private DateTime[] GetDatesForVerification()
        {
            var result = new DateTime[15]; //For the next 15 days
            var firstDate = DateTime.Today;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = firstDate.AddDays(i);
            }

            return result;
        }

        private static bool HasFreeSlots(string pageData)
        {
            return !string.IsNullOrEmpty(pageData) && !pageData.Contains("brak wolnych biletów");
        }

       
    }
}