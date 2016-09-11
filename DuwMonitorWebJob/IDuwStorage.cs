using System;

namespace DuwMonitorWebJob
{
    public interface IDuwStorage
    {
        string ReadDataByDate(DateTime targetDate);
    }
}