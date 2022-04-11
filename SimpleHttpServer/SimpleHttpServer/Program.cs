using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.Web;
//using static simpleHttpServerTwo.HandleIncomingConnections2;

namespace simpleHttpServer
{
    class MainClass
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static DvaReplyManager theDvaReplyManager = new DvaReplyManager();
        public static ScheduledRtdmDatabase theScheduledRtdmDatabase = new ScheduledRtdmDatabase();
        public static MainClass mainClass = new MainClass();
        public string userName = "omer";

        public static string pageData =
            "<!DOCTYPE>" +
            "<html>" +
            "  <head>" +
            "    <title>HttpListener Example</title>" +
            "  </head>" +
            "  <body>" +
            "    <p>Page Views: {0}</p>" +
            "    <form method=\"post\" action=\"shutdown\">" +
            "      <input type=\"submit\" value=\"Shutdown\" {1}>" +
            "    </form>" +
            "  </body>" +
            "</html>";


        public static string pageDataScheduled =
            "<ScheduleListItem>" +
                "<Days>Weekends,Sat,Sun</Days><br/>" +
                "<Duration>0</Duration> <br/>" +
                "<Priority>5</Priority> <br/>" +
                "<ScheduledBy>chris</ScheduledBy> <br/>" +
                "<fldClassifier>MRA</fldClassifier> <br/>" +
                "<fldEndTime>11:38</fldEndTime> <br/>" +
                "<fldIcon>text</fldIcon> <br/>" +
                "<fldName>Text Only Message</fldName> <br/>" +
                "<fldRepeatInterval>10</fldRepeatInterval> <br/>" +
                "<fldStartTime>10:38</fldStartTime> <br/>" +
                "<fldTarget>PAZ01</fldTarget>" +
                "<fldTargetDescription/> <br/>" +
                "<fldTargetLabel>Non Public</fldTargetLabel> <br/>" +
                "<fldid>381</fldid> <br/>" +
                "<fldmsgid>10100</fldmsgid> <br/>" +
            "</ScheduleListItem>";



        //-----For Page Counter-----//
        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                //req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine();
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                Console.WriteLine("absolute path: " + req.Url.AbsolutePath); //should be printing when making request.
                string response = "";
                //get scheduled message:
                if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath == "/scheduled"))
                {
                    Console.WriteLine("sched abs path: " + req.Url.AbsolutePath);

                    //This seperates the reply functions from the HTTP Listener.
                    response = theDvaReplyManager.GetScheduled();
                }
                //Add Scheduled Text:
                else if ((req.HttpMethod == "GET") && (req.Url.AbsolutePath.StartsWith("/AddScheduleTextOnly/")))
                {
                    //Console.WriteLine("req.URl: " + req.Url);
                    string[] splitUrl = req.Url.AbsolutePath.Split('/'); 
                    if (splitUrl.Length == 3)
                    {
                        string urlRtdmData = splitUrl[2];
                        urlRtdmData = HttpUtility.UrlDecode(urlRtdmData); //removes all characters not found in a URL.
                        Console.WriteLine("urlRtdmData: " + urlRtdmData);
                        response = theDvaReplyManager.AddScheduledTextOnly(urlRtdmData);
                    }
                }
                //delete single item:
                else if ((req.HttpMethod == "GET") && req.Url.AbsolutePath.StartsWith("/scheduled/") && req.Url.AbsolutePath.EndsWith("/omer"))
                {
                    string[] splitDeleteUrl = req.Url.AbsolutePath.Split('/');
                    string urlDeleteData = splitDeleteUrl[2]; //contains the id and username
                    response = theDvaReplyManager.DeleteRtdmMsg(urlDeleteData);
                }
                //delete ALL items in the dictionary:
                else if ((req.HttpMethod == "GET") && req.Url.AbsolutePath.StartsWith("/scheduled"))
                {
                    //string[] splitDeleteAllUrl = req.Url.AbsolutePath.Split('/');
                    //string urlUsername = splitDeleteAllUrl[2];

                    //if (req.Url.AbsolutePath.Contains(urlUsername))
                    //{
                    response = theDvaReplyManager.DeleteAllRtdmMessages();
                    //}
                }

                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                if (req.Url.AbsolutePath != "/favicon.ico")
                pageViews += 1;

                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(response, pageViews, disableSubmit));
                //resp.ContentType = "text/html"; //Without this, the webpage will be displayed in HTML/XML format. 
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                //Write out to the response stream(asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}

//The test software --> Add user input functionality on the Client.
//The user input defines the type of request made. Eg. Clinet starts --> prompts 's' or 'p' --> This then makes the request to server.
//



