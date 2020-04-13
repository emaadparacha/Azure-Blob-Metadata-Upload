using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Xml;
using System.Net;
using System.IO;

namespace AzureStorageBlobDemo
{
    class Program
    {
        static void Main(string[] args)
        {

            // Create Reference to Azure Storage Account
            String strorageconn = System.Configuration.ConfigurationSettings.AppSettings.Get("StorageConnectionString");
            CloudStorageAccount storageacc = CloudStorageAccount.Parse(strorageconn);

            //Create Reference to Azure Blob
            CloudBlobClient blobClient = storageacc.CreateCloudBlobClient();

            //The next 2 lines create if not exists a container named "democontainer"
            CloudBlobContainer container = blobClient.GetContainerReference("democontainer");

            container.CreateIfNotExists();


            //The next lines extract metadata from the file and upload it with the name DemoBlob on the container "democontainer"
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("DemoUpload");

            using (var filestream = System.IO.File.OpenRead(@"/Users/DemoUser/Desktop/sample_srt.srt"))
            {
                //Extracts the 4th line of the SRT file and extracts the date and time
                string datetime = System.IO.File.ReadLines("/Users/DemoUser/Desktop/sample_srt.srt").Skip(3).Take(1).First();
                string[] datetimewords = datetime.Split(' ');
                string[] timewords = datetimewords[1].Split(',');

                //Changes the date from numeric form to readable form (2019-02-28 to February 28, 2019)
                string[] wordmonth = datetimewords[0].Split('-');
                string month = wordmonth[1];
                int monthnum = int.Parse(month);
                System.Globalization.DateTimeFormatInfo mfi = new System.Globalization.DateTimeFormatInfo();
                string monthname = mfi.GetMonthName(monthnum).ToString();

                string datewords = monthname + " " + wordmonth[2] + ", " + wordmonth[0];

                //Extracts the 5th line of the SRT file
                string latlong = System.IO.File.ReadLines("/Users/DemoUser/Desktop/sample_srt.srt").Skip(4).Take(1).First();

                //Extracts the latitude and longitude from the 5th line
                string[] latlongspec = latlong.Split(']');
                string[] lat = latlongspec[7].Split(' ');
                string[] log = latlongspec[8].Split(' ');

                //Sets up coordinate string
                string coord = "(" + lat[2] + "," + log[2] + ")";
                string coordnopar = lat[2] + "," + log[2];

                //Creates a link that uses Azure Maps API to get cross street information from the given coordinates
                string crossurl = "https://atlas.microsoft.com/search/address/reverse/crossStreet/json?subscription-key=<ADD-AZURE-MAPS-CONNECTION-STRING-HERE>&api-version=1.0&query=" + coordnopar;

                //Reads the JSON file from the link and converts to it to string
                var jsoncross = new WebClient().DownloadString(crossurl);
                string jsoncrossstr = jsoncross.ToString();

                //Extracts text from the JSON file
                string[] crosss = jsoncross.Split('"');

                string crossstreet = "";

                //If cross street data was found, it extracts it. If not, the variable is assigned "N/A"
                if (jsoncrossstr.Contains("streetName"))
                {
                    string[] streetnamesplit = jsoncrossstr.Split(new string[] { "streetName" }, StringSplitOptions.None);
                    string[] streetnamesplit2 = streetnamesplit[1].Split('"');
                    crossstreet = streetnamesplit2[2];
                }

                else
                {
                    crossstreet = "N/A";
                }

                //Metadata being added to the uploaded blobs
                blockBlob.Metadata.Add("Date", datetimewords[0]);
                blockBlob.Metadata.Add("Time", timewords[0]);
                blockBlob.Metadata.Add("Coordinates", coord);
                blockBlob.Metadata.Add("CrossStreet", crossstreet);
                blockBlob.Metadata.Add("FullDate", datewords);

                //Blob uploaded to Blob Storage
                blockBlob.UploadFromStream(filestream);

            }

        }
    }
}
