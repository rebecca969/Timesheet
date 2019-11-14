using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Timesheet.BusinessLogical
{
    public enum Status
    {
        Active,
        Submitted
    };

    public enum WorkType
    {
        TelephoneCall,
        Research,
        DraftingDocument
    }

    public class Item
    {
        private int id;
        private string title;
        private decimal rate;
        private TimeSpan duration;
        private Status state;
        private WorkType type;

        public Item(decimal rate)
        {
            this.rate = rate;
        }

        public Item(int id, Status state, string title, decimal rate, TimeSpan duration, WorkType type)
        {
            this.id = id;
            this.state = state;
            this.title = title;
            this.rate = rate;
            this.duration = duration;
            this.type = type;
        }

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                title = value;
            }
        }

        public decimal Rate
        {
            get
            {
                return rate;
            }
            set
            {
                rate = value;
            }
        }

        public TimeSpan Duration
        {
            get
            {
                return duration;
            }
            set
            {
                duration = value;
            }
        }

        public Status State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
            }
        }

        public WorkType Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
    }


}
