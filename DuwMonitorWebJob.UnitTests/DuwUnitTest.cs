using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DuwMonitorWebJob.UnitTests
{
    [TestClass]
    public class DuwUnitTest
    {
        [TestMethod]
        public void GetInfoForNext15Days_ReturnsResultForNext15Days()
        {
            var duwStorageStub = new Mock<IDuwStorage>();
            var mailSenderStub = new Mock<IMailSender>();
            var startDate = DateTime.Today;
            var duw = new Duw(duwStorageStub.Object, mailSenderStub.Object);
            var result = duw.GetInfoForNext15Days();
            Assert.AreEqual(15, result.Length, $"Duw returned result for invalid count of days: {result.Length}");
            var onlyDates = result.Select(it => it.Key);
            var orderedDates = onlyDates.OrderBy(it => it).ToArray();

            for (var i = 0; i < orderedDates.Length; i++)
            {
                var expectedDate = startDate.AddDays(i);
                var orderedDate = orderedDates[i];
                Assert.AreEqual(expectedDate, orderedDate, $"Invalid date is present in returned date list: {orderedDate}");
            }
        }

        [TestMethod]
        public void GetInfoForNext15Days_ReturnsOneFreeDate()
        {
            var duwStorageStub = new Mock<IDuwStorage>();
            var mailSenderStub = new Mock<IMailSender>();
            var validDate = DateTime.Today;
            duwStorageStub.Setup(it => it.ReadDataByDate(validDate)).Returns($"has free slot for this date {validDate}");
            var duw = new Duw(duwStorageStub.Object, mailSenderStub.Object);
            var info = duw.GetInfoForNext15Days();
            Assert.IsTrue(info.Single(it => it.Key == validDate).Value, "Duw said that it has no free dates, but I know - it has.");
        }

        [TestMethod]
        public void GetInfoForNext15Days_ReturnsOneDateIsUsedAllOtherAreFree()
        {
            var duwStorageStub = new Mock<IDuwStorage>();
            var mailSenderStub = new Mock<IMailSender>();
            var startDate = DateTime.Today;
            for (var i = 0; i < 15; i++)
            {
                var currentDate = startDate.AddDays(i);
                var returnData = $"has free slot for this date {currentDate}";
                if (i == 14) //The last day. Set invalid data for it.
                {
                    returnData = "brak wolnych biletów"; //PL: it means there is no free tickets. 
                }
                duwStorageStub.Setup(it => it.ReadDataByDate(currentDate)).Returns(returnData);
            }
            var duw = new Duw(duwStorageStub.Object, mailSenderStub.Object);
            var info = duw.GetInfoForNext15Days();
            Assert.IsFalse(info.Single(it => it.Key == startDate.AddDays(14)).Value, "Duw said that it has a free date for me, but this date is not free.");
        }

        [TestMethod]
        public void GetInfoForNext15Days_DuwStorageThrowsException_MailSenderSendsEmail()
        {
            var duwStorageStub = new Mock<IDuwStorage>();
            var mailSenderMock = new Mock<IMailSender>();
            var targetDate = DateTime.Today;
            var exception = new DuwStorageException($"Cannot get data for this date {targetDate}");
            duwStorageStub.Setup(it => it.ReadDataByDate(targetDate)).Throws(exception);
            var duw = new Duw(duwStorageStub.Object, mailSenderMock.Object);
            duw.GetInfoForNext15Days();

            mailSenderMock.Verify(it => it.SendEmail(new Message
            {
                Body = $"Exception occured during getting info for {targetDate}. Exception: {exception}",
                Recepient = "dima.oleshchenko@outlook.com",
                Sender = "duwmonitor@monitoring.com",
                Subject = $"EXCEPTION OCCURED {DateTime.Now.ToString("g")}"
            }), Times.Once);
        }
    }
}
