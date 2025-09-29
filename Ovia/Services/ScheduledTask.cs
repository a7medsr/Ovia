using System;

namespace Ovia.Services
{
    public class ScheduledTask
    {
        private readonly ProfitToCustomerAccountConverter _converter;
        private Timer _timer;

        public ScheduledTask(ProfitToCustomerAccountConverter converter)
        {
            _converter = converter;
        }

        public void Start()
        {
            // Calculate time until next Thursday
            DateTime now = DateTime.Now;
            DayOfWeek today = now.DayOfWeek;
            int daysUntilThursday = ((int)DayOfWeek.Thursday - (int)today + 7) % 7;
            DateTime nextThursday = now.AddDays(daysUntilThursday).Date;

            // Set the time to 11:59 PM
            nextThursday = nextThursday.AddHours(23).AddMinutes(59).AddSeconds(59);

            // Calculate time interval to repeat every Thursday
            TimeSpan timeUntilThursday = nextThursday - now;
            TimeSpan oneWeek = TimeSpan.FromDays(7);
            TimeSpan timeUntilNextExecution = timeUntilThursday;

            // Set up timer to run the service every Thursday at 11:59 PM
            _timer = new Timer(
                _ => _converter.ConvertProfitToCustomerAccount(),
                null,
                timeUntilNextExecution,
                oneWeek);
        }

        public void Stop()
        {
            _timer?.Change(Timeout.Infinite, 0);
        }
    }
}
