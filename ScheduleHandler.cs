using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Cod4RconConsoleTool
{
    class ScheduleHandler
    {
        public static List<string> ReadScheduleFile(string file = "schedule.config")
        {
            if (File.Exists(file))
            {
                var filelines = new List<string>();
                var schedulefile = new StreamReader(file);
                while (!schedulefile.EndOfStream)
                {
                    filelines.Add(schedulefile.ReadLine());
                }
                return filelines;
            }
            else
            {
                File.Create("schedule.config");
                throw new FileNotFoundException(
                    "Error schedule.config file is missing! -- New file generated, please edit and try again");
            }
        }
    }
}
