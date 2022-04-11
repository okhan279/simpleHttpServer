using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace simpleHttpServer
{
    public class ScheduledItem
    {

        //Add properties:
        public string Text { get; }
        public string StartTime { get; } //DateTime --> Timestamp.
        public string EndTime { get; }
        public string Days { get; }
        public string User { get; }
        public string repeatInterval { get; }

        public Boolean PlayMonday { get; }
        public Boolean PlayTuesday { get; }
        public Boolean PlayWednesday { get; }
        public Boolean PlayThursday { get; }
        public Boolean PlayFriday { get; }
        public Boolean PlaySaturday { get; }
        public Boolean PlaySunday { get; }
        public Boolean TodayOnly { get; }

        public string AsXml { get; private set; }
        public TimeSpan RepeatInterval { get; } //int.Parse(...);

        private int id;

        public int GetId()
        {
            return id;
        }

        public void SetId(int value)
        {
            id = value;
            AsXml = GetXML();
        }

        //Regex rgx = new Regex("[^a-zA-Z0-9]");

        public enum days
        {
            monday = 1,     //0000 0001
            tuesday = 2,    //0000 0010
            wednesday = 4,  //0000 0100
            thursday = 8,   //0000 1000
            friday = 16,    //0001 0000
            saturday = 32,  //0010 0000
            sunday = 64,    //0100 0000
            todayOnly = 128 //1000 0000
        }

        public string GetXML()
        {
            return $"\t<ScheduledListItem>\n" +
                        $"\t\t<Text> {Text}  </Text>\n" +
                        $"\t\t<Days> {daysAnnouncement} </Days>\n" +
                        $"\t\t<ScheduledBy> {User} </ScheduledBy>\n" +
                        $"\t\t<fldEndTime> {EndTime} </fldEndTime>\n" +
                        $"\t\t<fldRepeatInterval> {repeatInterval} </fldRepeatInterval>\n" +
                        $"\t\t<fldStartTime> {StartTime} </fldStartTime>\n" +
                        $"\t\t<fldid> {GetId()} </fldid> \n" +
                    $"\t</ScheduledListItem>\n";
        }

        //Used in the daysXML() method:
        StringBuilder daysAnnouncement = new StringBuilder();


        public ScheduledItem(string input)
        {

            //Console.WriteLine(input);
            string[] elements = input.Split(','); //[There%20are%20delays,7,10:00,13:30,10,omer]

            string startTimeTrim = elements[2].Substring(0, 5); //limits the input start time string to first 5 characters.
            string endTimeTrim = elements[3].Substring(0, 5);

            //Sets first character of element[0] to uppercase. Eg. if client enters 'there are delays', server will read 'There are delays'
            string elementsZeroUpper = elements[0].Substring(0, 1).ToUpper();

            //Class that contains all the Guard Clauses:
            byte daysByte;
            guardClauses();

            PlayMonday = (daysByte & (byte)days.monday) == (byte)days.monday;
            PlayTuesday = (daysByte & (byte)days.tuesday) == (byte)days.tuesday;
            PlayWednesday = (daysByte & (byte)days.wednesday) == (byte)days.wednesday;
            PlayThursday = (daysByte & (byte)days.thursday) == (byte)days.thursday;
            PlayFriday = (daysByte & (byte)days.friday) == (byte)days.friday;
            PlaySaturday = (daysByte & (byte)days.saturday) == (byte)days.saturday;
            PlaySunday = (daysByte & (byte)days.sunday) == (byte)days.sunday;
            TodayOnly = (daysByte & (byte)days.todayOnly) == (byte)days.todayOnly;

            //Class containing certain Console.WriteLine() statements:
            stringForScheduledItem();

            Text = elements[0]; //output: There%20are%20delays
            daysXML(); //Method that prints the days of announcement to the XML tag <days>.
            StartTime = elements[2];
            EndTime = elements[3];
            repeatInterval = elements[4];
            User = elements[5];

            AsXml = GetXML();
            Console.WriteLine($"AsXML: {AsXml}");

            //Contains all the Console.WriteLine statements.
            void stringForScheduledItem()
            {
                Console.WriteLine($"Play Mondays: {PlayMonday}, \n" +
                $"Play Tuesday: {PlayTuesday}, \n" +
                $"Play Wednesday: {PlayWednesday} \n" +
                $"Play Thursday: {PlayThursday} \n" +
                $"Play Friday: {PlayFriday} \n" +
                $"Play Saturday: {PlaySaturday} \n" +
                $"Play Sunday: {PlaySunday} \n" +
                $"Play Today Only: {TodayOnly}");

                //Console.WriteLine("startTime: " + StartTime);
                //Console.WriteLine("endTime: " + EndTime);
            }

            //Method that will print the days that are true in the <Days>...</Days> tag in the getXML() method.
            void daysXML()
            {
                if (PlayMonday) daysAnnouncement.Append("Mon ");
                if (PlayTuesday) daysAnnouncement.Append("Tue ");
                if (PlayWednesday) daysAnnouncement.Append("Wed ");
                if (PlayThursday) daysAnnouncement.Append("Thurs ");
                if (PlayFriday) daysAnnouncement.Append("Fri ");
                if (PlaySaturday) daysAnnouncement.Append("Sat ");
                if (PlaySunday) daysAnnouncement.Append("Sun");
                if (TodayOnly) daysAnnouncement.Append("Today Only");
            }

            void guardClauses()
            {
                //-----Exceptions-----//
                //if(!HasNonASCIIChars(elements[0])) throw new ArgumentException("Text contains non-alphanumeric characters");

                if (elements.Length > 6) throw new ArgumentException("Absolute URL exceeded limit of 6 values.");

                //elements[0] is the text message to the customers
                if (elements[0].Length > 255) throw new ArgumentException("Text length exceeds maximum limit of 255 characters.");

                //Checks to confirm that the text contains alphanumeric characters using the RegEx method.
                //if (!Regex.IsMatch(elements[0], "^[a-zA-Z0-9]*$")) throw new ArgumentException("Text contains non-alphanumeric characters");

                if (!Byte.TryParse(elements[1], out daysByte)) throw new ArgumentException("Could not parse days.");

                if (daysByte > 128 || daysByte < 1) throw new ArgumentException("Element length is beyond set limits.");

                if (!DateTime.TryParse(startTimeTrim, out DateTime StartTime)) throw new ArgumentException("Could not parse start time.");

                if (!DateTime.TryParse(endTimeTrim, out DateTime EndTime)) throw new ArgumentException("Could not parse end time.");

                if (StartTime > EndTime || StartTime == EndTime) throw new ArgumentException("Start Time is after or same as end time.");

                if (int.Parse(elements[4]) > 60) throw new ArgumentException("Time interval is more than 60 minutes");

                //elements[5] is the username
                if (elements[5].Length > 20) throw new ArgumentException("User has exceeded character limit for user input of 20 characters.");

                //loop over each element. if element is empty throw exception
                foreach (string element in elements)
                {
                    if (element == "")
                    {
                        throw new ArgumentException("element is empty");
                    }

                    string elementsArray = element.ToString();
                    Console.WriteLine("elements array: " + elementsArray);
                }
            }
        }

        public override string ToString()
        {
            //returns string that states what is in the scheduled item:
            //This could be used to give a print out of all the messages over a certain time period.
            //Or when a certain message is about to be played...
            return "messageText: " +Text+ "\n"
                    + "messageStartTime: " +StartTime + "\n"
                    + "messageEndTime: " +EndTime+ "\n"
                    + "repeatInterval: " +repeatInterval+ "\n"
                    + "User: " +User+ "\n"
                    + "ID: " + GetId() + "\n";
        }
    }
}



//Notes - 27/01/22:
//1. Build a full input on the client side.
//2. Under 'StartsWith(...)' pull out the URL on line 28 and pass it to the reply manager.
//3. Attempt to build a scheduled item type, using constructor in line 25 --> 
//4. If successful --> check if the start time before the end time, etc.
//5. Build XML Dva Interface Response.
//6. If successful --> Add to database which will be a dictionary.
//7. Any future call to scheduled, use the GetXml() method to build the response to the scheduled request.
//

//byte daysByte = Convert.ToByte(elements[1]);
//Console.WriteLine("daysByte: " +daysByte);

//int testing = int.Parse(elements[1]);
//byte value = (byte)testing;
//Console.WriteLine("value: " +value.GetType());

//if((sevenInBinary & (byte)daysValue) == (byte)days.monday)
//{
//    Console.WriteLine("Announcement will play on Monday");
//}


//foreach (byte b in daysByte)
//{
//    Console.WriteLine("daysByte: " + b); //55 if input is 7.
//    Console.WriteLine($"{Convert.ToString(b, 2).PadLeft(8, '0')}"); //0011 0111 if input is 7.
//}
//There are 2 bytes representation of 7:
//  1. First is 0000 0111
//  2. ASCII encoded value for character 7.

//Notes from 17/02/2022:
//    if elements.length != expected length then throw an exception.
//      Throw newArguementException with the message 'there were not 6 elements'.
//    As I am throwing exceptions in this contructor, I would have to use try/catch.
//      Could use regular try/catch, but that is poor form.
//      Where I
//    Make checks that all invalidity of messages are captured.
//    Perhaps there is a HTTP library that allows us to check that there are no invalid entries.
//    When I call this constructor(scheduledItem) use try/catch.
//    With username, limit username to a small number of charters(Eg. 30 characters).
//    When throwing exceptions, any code that contains exceptions should be at the very top.
//    Check al inputl values are valid, guard clauses.
//    Do not have console.writeline.Have a new method inside the schduledItem class called ToString(). In that we generate string that has
//   all of these values in.
//      scheduledItem newItem = new scheduledItem(..);
//      console.writeline(newItem.ToString());


//Use TryParse. Do same as if tryparse is false thrown new arguement exception.
//StartTime = DateTime.Parse(startTimeTrim);
//EndTime = DateTime.Parse(endTimeTrim);

//Console.WriteLine("startTime: " +StartTime);
//Console.WriteLine("endTime: " + EndTime);