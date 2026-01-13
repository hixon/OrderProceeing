using System.Collections;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder()
    .SetBasePath("/home/ry/Documents/Projects/OrderProceeing")  //path needs to be set to something else, maybe env var?
    .AddJsonFile("appsettings.json");
    
var configuration = builder.Build();

// See https://aka.ms/new-console-template for more information
//Console.WriteLine("App Started: " + DateTime.Now.ToLocalTime());

var logFilePath = @"../../../Log_" + 
    DateTime.UtcNow.Month.ToString("##") + DateTime.UtcNow.Day.ToString("##") + DateTime.UtcNow.Year.ToString("##");

var rootDIR = @"../../../Files";
var closedFILE = configuration.GetSection("Closed_File").Value;
var waitingForPaymentFILE = configuration.GetSection("WaitingForPayment_File").Value;

List<String> companyList = new List<string>(Directory.EnumerateDirectories(rootDIR));

List<string> directoriesToClose = new List<string>();
List<string> directoriesWaitingForPayment = new List<string>();

//create log file
if (!File.Exists(logFilePath))
{
    using (StreamWriter sw = File.CreateText(logFilePath))
    {
        sw.WriteLine("Log Started: " + DateTime.Now.ToShortDateString() + "-" + DateTime.Now.ToShortTimeString());        
    }
}

foreach(var company in companyList)
{
    List<string> openCases = new List<string>(Directory.EnumerateDirectories(company+"/Open"));
    
    foreach(var opencase in openCases)
    {
        //check to see if we should move one of the folders somewhere
        if (File.Exists(opencase + closedFILE))
        {
            //place whole directory up for movement
            directoriesToClose.Add(opencase);
        }
        else if (File.Exists(opencase + waitingForPaymentFILE))
        {
            directoriesWaitingForPayment.Add(opencase);
        }
    }
}

if(directoriesToClose.Count() > 0)
{
    using (StreamWriter sw = File.AppendText(logFilePath))
    {
        //Console.WriteLine("\n################################");
        //Console.WriteLine("Directories that should be closed:");
        sw.WriteLine("\n################################");
        sw.WriteLine("Directories that should be closed:");
        foreach(var item in directoriesToClose)
        {
            //Console.WriteLine(item);
            sw.WriteLine(item);
        } 
        sw.WriteLine("################################\n");
        //Console.WriteLine("################################\n");
    }
}

if(directoriesWaitingForPayment.Count() > 0)
{
    using(StreamWriter sw = File.AppendText(logFilePath))
    {
        //Console.WriteLine("\n################################");
        //Console.WriteLine("Directories that should be placed in hold for payment:");
        
        sw.WriteLine("\n################################");
        sw.WriteLine("Directories that should be placed in hold for payment:");
        foreach(var item in directoriesWaitingForPayment)
        {
            //Console.WriteLine(item);
            sw.WriteLine(item);
        }
        sw.WriteLine("################################\n");
        //Console.WriteLine("################################\n");   
    }
}

//Section to move files if in processing or complete
//move full directory into ../../Closed
foreach(var closed in directoriesToClose)
{
    using (StreamWriter sw = File.AppendText(logFilePath))
    {
        sw.WriteLine("\n################################");
        sw.WriteLine("Directories");
        //Console.WriteLine("\n################################");
        //Console.WriteLine("Directories");
        string destinationPath = closed.Replace("Open", "Closed");
        if (Path.Exists(destinationPath))
        {
            //This will throw an error if the thing already exists
            //what's our workaround? just copy all files out to the directory that are missing currently
            Directory.Move(closed, closed.Replace("Open", "Closed"));    
        }
        else
        {
            //need to create the destination path
            Directory.Move(closed, closed.Replace("Open", "Closed"));   
            sw.WriteLine("Moved " + closed + " to " + closed.Replace("Open", "Closed"));
        }

        sw.WriteLine("################################\n");
    }
}

using(StreamWriter sw = File.AppendText(logFilePath))
{
    //Console.WriteLine("App Completed: " + DateTime.UtcNow.ToLocalTime());
    sw.WriteLine("App Completed: " + DateTime.Now.ToLocalTime());
}