using System.Collections;
using System.IO;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("App Started: " + DateTime.Now.ToLocalTime());

var rootDIR = @"../../../Files/Open";
var closedFILE = "/closed.dat";
var waitingForPaymentFILE = "/payment.dat";

List<String> companyList = new List<string>(Directory.EnumerateDirectories(rootDIR));

List<string> directoriesToClose = new List<string>();
List<string> directoriesWaitingForPayment = new List<string>();
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
    Console.WriteLine("\n################################");
    Console.WriteLine("Directories that should be closed:");
    foreach(var item in directoriesToClose)
    {
        Console.WriteLine(item);
    }    
    Console.WriteLine("################################\n");
}

if(directoriesWaitingForPayment.Count() > 0)
{
    Console.WriteLine("\n################################");
    Console.WriteLine("Directories that should be placed in hold for payment:");
    foreach(var item in directoriesWaitingForPayment)
    {
        Console.WriteLine(item);
    }
    Console.WriteLine("################################\n");
}

Console.WriteLine("App Completed: " + DateTime.UtcNow.ToLocalTime());
