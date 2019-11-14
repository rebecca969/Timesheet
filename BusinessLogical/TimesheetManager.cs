using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.BusinessLogical
{
    public class TimesheetManager
    {
        private decimal defaultRate;
        private List<Item> items = new List<Item>();

        public decimal DefaultRate
        {
            get { return defaultRate; }
            set { defaultRate = value; }
        }

        public TimesheetManager()
        {
            defaultRate = decimal.Parse("250.50");
        }
        public Item AddNewTimesheet()
        {
            Item item = new Item(defaultRate);
            return item;
        }

        public decimal CalculateTotal(string duration, decimal rate)
        {
            decimal total=0;
            String[] parts = duration.Split(':');
            int hours = Int32.Parse(parts[0].TrimStart(new char[] {'0'}));
            int minutes = Int32.Parse(parts[1].TrimStart(new char[] { '0' }));
            int roundingMinutes = TimeRoundUp(minutes);

            total = (hours + Convert.ToDecimal(roundingMinutes)/60) * rate;
            return total;
        }

        private int TimeRoundUp(int minutes)
        {
            return minutes + (minutes % 15 == 0 ? 0 : 15 - minutes % 15);
        }

        public int Add(string title, decimal rate, TimeSpan duration, WorkType type)
        {
            int retVal;
            try
            {
                int id = items.Count > 0 ? items.Max(x => x.Id) + 1 : 1;
                items.Add(new Item(id, Status.Active, title, rate, duration, type));
                retVal = id;
            }
            catch (Exception ex)
            {
                retVal = 0;
            }
            return retVal;
        }

        public bool Edit(int id, Status state, string title, decimal rate, TimeSpan duration, WorkType type)
        {
            bool retVal;
            try
            {
                var item = items.SingleOrDefault(x => x.Id == id);
                item.State = state;
                item.Title = title;
                item.Rate = rate;
                item.Duration = duration;
                item.Type = type;
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public bool Delete(int id)
        {
            bool retVal;
            try
            {
                var item = items.SingleOrDefault(x => x.Id == id);
                items.Remove(item);
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public List<Item> Read()
        {
            return items;
        }

        public bool Submit(int id)
        {
            bool retVal;
            try
            {
                var item = items.SingleOrDefault(x => x.Id == id);
                item.State = Status.Submitted;
                retVal = true;
            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }
    }
}
