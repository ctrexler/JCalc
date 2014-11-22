using System;
using System.Collections.Generic;
using System.Text;

namespace JeopardyCalculator
{
    public class JCalcScores
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Score { get; set; }
        public DateTimeOffset Time { get; set; }
        public string DateFormatted { get { return Time.ToString("MM/dd/yy H:mm"); } }
    }
}
