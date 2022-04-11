using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simpleHttpServer
{
    public class ScheduledRtdmDatabase
    {
        public int nextRtdmId = 0;
        private const int maxID = 5000;
        private Dictionary<int, ScheduledItem> theRtdms;

        public ScheduledRtdmDatabase()
        {
            theRtdms = new Dictionary<int, ScheduledItem>();
        }

        /// <summary>
        /// Adds RTDM to dictionary
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newRtdm"></param>
        public void AddRtdm(ScheduledItem newRtdm)
        {
            theRtdms.Add(newRtdm.GetId(), newRtdm);
        }

        /// <summary>
        /// Remove RTDM with specified ID from dictionary
        /// </summary>
        /// <param name="id"></param>
        public bool RemoveRtdm(int id, out string error) 
        {
            error = "";
            if (theRtdms.ContainsKey(id))
            {
                theRtdms.Remove(id);
                return true;
            }
            else
            {
                error = "ID not found";
                return false;
            }
        }

        /// <summary>
        /// Remove all RTDM messages
        /// </summary>
        public void RemoveAllRtdms()
        {
            theRtdms.Clear();
        }


        /// <summary>
        /// Return all RTDMs as ReadOnlyCollection
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<ScheduledItem> GetAllRtdms()
        {
            List<ScheduledItem> rtdmList = theRtdms.Values.OrderBy(i=>i.GetId()).ToList();
            //rtdmList.ForEach(i => Console.Write("RtdmList:\n{0}", i));
            rtdmList.ForEach(i => Console.Write(i));

            ReadOnlyCollection<ScheduledItem> theRtdmsList
                = new ReadOnlyCollection<ScheduledItem>(rtdmList);

            return theRtdmsList;
        }

        //public int nextRtdmId = 0;

        /// <summary>
        /// Increment RTDM ID and return
        /// </summary>
        /// <returns></returns>
        public int GetNextId()
        {
            nextRtdmId++;
            if (nextRtdmId > maxID) nextRtdmId = 1;
            return nextRtdmId;
        }

    }
}
