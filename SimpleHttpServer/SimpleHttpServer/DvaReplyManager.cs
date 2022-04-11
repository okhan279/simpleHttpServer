using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace simpleHttpServer
{
    public class DvaReplyManager
    {
        ScheduledRtdmDatabase theScheduledRtdmDatabase = new ScheduledRtdmDatabase();

        private string GetXMLSucessOrFail(bool result, string detail, string error)
        {
            string resultString = result ? "OK" : "Fail";
            string errorString = error == "" ? "<Error i:nil=\"true\"/>" : $"<Error>{error}</Error>"; //if there is an error message, then run 2nd statement "$"<Error>{error}</Error>""
            string detailString = detail == "" ? "<Detail i:nil =\"true\"/>" : $"{detail}";

            return $"{errorString}<Result>{resultString}</Result><content i:nil=\"true\"/><Detail>{detailString}</Detail></DVAInterfaceResult>"; 
        }

        //Used for the '/scheduled' url
        public string GetScheduled()
        {
            ReadOnlyCollection<ScheduledItem> theRtdms = theScheduledRtdmDatabase.GetAllRtdms();

            string scheduleXml = "<XML Header> \n";

            foreach (var item in theRtdms)
            {
                scheduleXml += (item.AsXml);
            }

            scheduleXml += "</XML Footer>";

            return scheduleXml;
        }

        private List<string> ValidUsernames = new List<string>()
        {
            "MICA", "LIS", "chris", "omer"
        };
        //if(validUsername.Contains(inputUsername)...

        //Method to DELETE particular RTDM depending on ID:
        public string DeleteRtdmMsg(string input)
        {
           string[] delElements = input.Split(','); //[1,omer]
           string idElement = delElements[0]; //Stores only the ID 
           int intID = int.Parse(idElement);
           bool success = theScheduledRtdmDatabase.RemoveRtdm(intID, out string error);

           return GetXMLSucessOrFail(success, "", error);
        }

        //Method to DELETE all RTDM messages:
        public string DeleteAllRtdmMessages()
        {
            theScheduledRtdmDatabase.RemoveAllRtdms();
            theScheduledRtdmDatabase.nextRtdmId = 0;

            return GetXMLSucessOrFail(true, "", "");
        }

        //Method to add scheduled text:
        public string AddScheduledTextOnly(string rtdmDataString)
        {
            int rtdmID = theScheduledRtdmDatabase.GetNextId();
            try
            {
                //Console.WriteLine("rtdmDataString: " +rtdmDataString);
                ScheduledItem newScheduledItem = new ScheduledItem(rtdmDataString); //[There%20are%20delays,mon%20tues%20wed,10:00,13:30,10,omer]

                newScheduledItem.SetId(rtdmID);
                //Console.WriteLine("ID: " + newScheduledItem.Id);

                theScheduledRtdmDatabase.AddRtdm(newScheduledItem);

                Console.WriteLine(theScheduledRtdmDatabase.GetAllRtdms()); //Prints all the scheduled messages to the terminal.

                return GetXMLSucessOrFail(true, rtdmID.ToString(), "");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message); //This is where the exception messages in the ScheduledItem class will be printed.
                                               //If I had multiple exceptions, I would have multiple catch statements.
                return GetXMLSucessOrFail(false, rtdmID.ToString(), ex.Message);
            }
        }

        //Method to delete single item from the database:
        //    internal string deleteItem() //When this runs, the scheduled item (pageDataSchedule) should be deleted.
        //    {
        //        //code that deletes the getScheduled() class. The class should be deleted depending on the 'fldid' number of the
        //        //getScheduled() class.

        //        //return pageDataDelete;
        //    }

        //Method to delete all items:
        //    internal string deleteAllItems()
        //    {
        //        //code that will delete all entries in the XML database.
        //    }

        //Method to add composed scheduled message:
        //    internal string AddComposedSchedule()
        //    {
        //        //Add composed message to the database.
        //        return addComposedScheduled;
        //    }

        //Method to add text only scheduled message:
        //    internal string addTextOnlyScheduledMsg()
        //    {
        //        //Code that will take in a number of values and will print that in XML format and send to the database.
        //        return addTextOnlyMsg;
        //    }

        //Method to add one shot play annoucement:
        //    internal string oneShotPlayAnnouncement()
        //    {
        //        //Has 2 extra URLs:
        //        //  For message ID
        //        //  Registered User ID
        //    }
    }
}

//Rather than having that in the DvaReplyManager, have a class called scheduled item. That RTDM will have all of the properties. It will also have the
//get XML property.
//The scheduled item class should contain all the RTDM logic within the class the RTDM properties.
//The database will be alist of these scheduled items. When recieveing the schdueled request, for each scheduled item --> scheduleItem.getXML().

//-----Add Text Only Message XML Display-----//
//public static string addTextOnlyMsg =
//     "<Days> Today only</Days>" +
//     "<Duration>0</Duration>" +
//     "<Priority>5</Priority>" +
//     "<ScheduledBy>chris</ScheduledBy>" +
//     "<fldClassifier>MRA</fldClassifier>" +
//     "<fldEndTime>02:00</fldEndTime>" +
//     "<fldIcon>text</fldIcon>" +
//     "<fldName>Text Only Message</fldName>" +
//     "<fldRepeatInterval>10</fldRepeatInterval>" +
//     "<fldStartTime>13:00</fldStartTime>" +
//     "<fldTarget>PAZ01</fldTarget>" +
//     "<fldTargetDescription/>" +
//     "<fldTargetLabel>Non Public</fldTargetLabel>" +
//     "<fldid>386</fldid>" +
//     "<fldmsgid>10104</fldmsgid>";


//Notes from meeting with Alex:
//Rather than having that in the DvaReplyManager, have a class called scheduled item. That RTDM will have all of the properties. It will also have the
//get XML property.
//The scheduled item class should contain all the RTDM logic within the class the RTDM properties.
//The database will be alist of these scheduled items. When recieveing the schdueled request, for each scheduled item --> scheduleItem.getXML().