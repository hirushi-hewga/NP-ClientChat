using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAppChat
{
    public class MessageInfo
    {
        public string Username { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public MessageInfo(string Username, string Text, DateTime Date)
        {
            this.Username = Username;
            this.Text = Text;
            this.Date = Date;
        }
        public override string ToString()
        {
            return $"{Username} : {Text}\t{Date.ToShortDateString()}";
        }
    }
}
