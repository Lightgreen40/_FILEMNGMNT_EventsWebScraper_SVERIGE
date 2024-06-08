using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Globalization;

namespace _FILEMNGMNT_EventsWebScraper
{
    class Program
    {
        static List<EventData> eventsList = new List<EventData>();
        static List<string> externalDescriptionLinks = new List<string>();
        static List<string> externalDescriptions = new List<string>();


        static async System.Threading.Tasks.Task Main(string[] args)
        {
            List<string> unityJazz_urlsList = new List<string>
            {
                "https://www.unityjazz.se/program"
            };

            List<string> nefertiti_urlsList = new List<string>
            {
                "https://www.nefertiti.se/kalendarium/"
            };

            List<string> gso_urlsList = new List<string>
            {
                "https://www.gso.se/program/konserter/?show_dates=1"
            };

            List<string> musikensHus_urlsList = new List<string>
            {
                "https://www.musikenshus.se/kalender/"
            };


            Console.WriteLine("Hämtar data för eventer i Göteborg – var snäll och vänta ...");
            Console.WriteLine();


            //string eventsFile = @"C:\Users\oelll\Dropbox\_WeeklyEventsGothenburg_4BCAL.txt";
            string eventsFile = @"C:\Users\Bernd\Downloads\Csharp\_FILEMNGMNT_EventsWebScraper_SVERIGE\testfiles\_WeeklyEventsGothenburg_4BCAL.txt";
            if (System.IO.File.Exists(eventsFile))
            {
                System.IO.File.Delete(eventsFile);
            }


            //foreach (string url in unityJazz_urlsList)
            //{
            //    try
            //    {
            //        string htmlContent = await GetHtmlContentAsync(url);

            //        List<EventData> eventsList = ParseHtml_UnityJazz(htmlContent);

            //        Console.ForegroundColor = ConsoleColor.Green;
            //        Console.WriteLine("Scraping av Unity Jazz websidan avslutat");
            //        Console.WriteLine();
            //        Console.ResetColor();
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine(">>> ERROR <<<:" + exception);
            //        Console.WriteLine();
            //        Console.ResetColor();
            //        Console.ReadLine();
            //    }
            //}

            //foreach (string url in nefertiti_urlsList)
            //{
            //    try
            //    {
            //        string htmlContent = await GetHtmlContentAsync(url);

            //        List<string> externalDescriptionLinks = PreParseHtml_Nefertiti(htmlContent);

            //        //Console.WriteLine("pre-fetched linklist:");   //debug
            //        //foreach (string link in externalDescriptionLinks)
            //        //{
            //        //    Console.WriteLine(link);
            //        //}

            //        Console.ForegroundColor = ConsoleColor.Green;
            //        Console.WriteLine("Pre-collecting av Nefertiti länkar avslutat");
            //        Console.WriteLine();
            //        Console.ResetColor();
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine(">>> ERROR <<<:" + exception);
            //        Console.ResetColor();
            //        Console.ReadLine();
            //    }
            //}

            //foreach (string externalUrl in externalDescriptionLinks)
            //{
            //    try
            //    {
            //        string externalHtmlContent = await GetHtmlContentAsync(externalUrl);

            //        externalDescriptions = ParseHtml_NefertitiDescriptions(externalHtmlContent);

            //        Console.ForegroundColor = ConsoleColor.Green;
            //        Console.WriteLine("Pre-scraping av Nefertiti länkar avslutat");
            //        Console.WriteLine();
            //        Console.ResetColor();
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine(">>> ERROR <<<:" + exception);
            //        Console.WriteLine();
            //        Console.ResetColor();
            //        Console.ReadLine();
            //    }
            //}

            //////Console.ReadLine();   //debug

            //foreach (string url in nefertiti_urlsList)
            //{
            //    try
            //    {
            //        string htmlContent = await GetHtmlContentAsync(url);

            //        List<EventData> eventsList = ParseHtml_Nefertiti(htmlContent);

            //        Console.ForegroundColor = ConsoleColor.Green;
            //        Console.WriteLine("Scraping av Nefertiti websidan avslutat");
            //        Console.WriteLine();
            //        Console.ResetColor();
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine(">>> ERROR <<<:" + exception);
            //        Console.WriteLine();
            //        Console.ResetColor();
            //        Console.ReadLine();
            //    }
            //}

            //foreach (string url in gso_urlsList)
            //{
            //    try
            //    {
            //        string htmlContent = await GetHtmlContentAsync(url);

            //        List<EventData> eventsList = ParseHtml_GSO(htmlContent);

            //        Console.ForegroundColor = ConsoleColor.Green;
            //        Console.WriteLine("Scraping av GSO websidan avslutat");
            //        Console.WriteLine();
            //        Console.ResetColor();
            //    }
            //    catch (Exception exception)
            //    {
            //        Console.ForegroundColor = ConsoleColor.Red;
            //        Console.WriteLine(">>> ERROR <<<:" + exception);
            //        Console.WriteLine();
            //        Console.ResetColor();
            //        Console.ReadLine();
            //    }
            //}

            foreach (string url in musikensHus_urlsList)
            {
                try
                {
                    string htmlContent = await GetHtmlContentAsync(url);

                    List<EventData> eventsList = ParseHtml_MusikensHus(htmlContent);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Scraping av Musikens Hus websidan avslutat");
                    Console.WriteLine();
                    Console.ResetColor();
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(">>> ERROR <<<:" + exception);
                    Console.WriteLine();
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }


            List<EventData> sortedEventsList = eventsList.OrderBy(e => e.DateTimeObject).ToList();

            ExportToTextFile(sortedEventsList, eventsFile);

            Console.ForegroundColor= ConsoleColor.Green;
            Console.WriteLine("Done");
            Console.WriteLine();
            Console.ResetColor();

            Console.ReadLine();   //debug, should auto-run after bug-free
        }




        static async System.Threading.Tasks.Task<string> GetHtmlContentAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);

                return await response.Content.ReadAsStringAsync();
            }
        }



        //färdig:
        static List<EventData> ParseHtml_UnityJazz(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var eventNodes = document.DocumentNode.SelectNodes("//article[contains(@class, 'eventlist-event eventlist-event--upcoming')]");

            if (eventNodes != null)
            {
                foreach (var node in eventNodes)
                {
                    Console.WriteLine("Extracted data:");

                    string date = "";
                    var dateNode = node.SelectSingleNode(".//time[@class='event-date']");
                    if (dateNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        date = GetAttributeValue(node, ".//time[@class='event-date']", "datetime");
                        Console.ResetColor();
                    }
                    DateTime dateTimeObject;
                    string dateSubstring = Regex.Match(date, @"\d{4}-\d{2}-\d{2}").Value;   //necessary due to strings like "2024-06-15"
                    if (DateTime.TryParseExact(dateSubstring, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dateTimeObject))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        date = dateTimeObject.ToString("dd.MM.yyyy");   //to convert initial yyyy-MM-dd to dd.MM.yyyy; NOTE: this is only for DISPLAY of date (normally done above in "dateNode" – dateTimeObject is still in "yyyy-MM-dd", which is okay, as internal calculation and sorting via LINQ doesn't care which date format as long as it's a valid format!)
                        Console.WriteLine(date);
                        //Console.WriteLine("Parsing date string to DateTime object succeeded");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(">>> ERROR <<<: Failed to parse date");
                        Console.ResetColor();
                    }

                    string location = "Kyrkogatan 13";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(location);
                    Console.ResetColor();

                    string title = "";
                    var titleNode = node.SelectSingleNode(".//a[@class='eventlist-title-link']");
                    if (titleNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        title = HighlightInterestingKeywords(titleNode.InnerText.Trim());
                        Console.WriteLine(title);
                        Console.ResetColor();
                    }

                    string description = "";
                    var descriptionNode = node.SelectSingleNode(".//div[@class='sqs-html-content']/p");
                    if (descriptionNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        description = HighlightInterestingKeywords(descriptionNode.InnerText.Trim());
                        Console.WriteLine(description);
                        Console.ResetColor();
                    }

                    string link = "";
                    var linkNode = node.SelectSingleNode(".//a[@class='eventlist-title-link']");
                    if (linkNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        link = @"https://www.unityjazz.se" + GetAttributeValue(node, ".//a[@class='eventlist-title-link']", "href");
                        Console.WriteLine(link);
                        Console.ResetColor();
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    eventsList.Add(new EventData
                    {
                        DateTimeObject = dateTimeObject,
                        Date = dateTimeObject.ToString("dd.MM.yyyy"),
                        Location = location,
                        Title = title,
                        Description = description,
                        Link = link
                    });
                }
            }

            return eventsList;
        }



        //färdig:
        private static List<string> PreParseHtml_Nefertiti(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var eventNodes = document.DocumentNode.SelectNodes("//article[@class='spajder-post']");

            Console.WriteLine("PreParseHtml_Nefertiti:");

            if (eventNodes != null)
            {
                foreach (var node in eventNodes)
                {
                    string externalLink = "";
                    var linkNode = node.SelectSingleNode(".//a");
                    if (linkNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        externalLink = GetAttributeValue(node, ".//a", "href");
                        Console.WriteLine(externalLink);   //debug
                        Console.ResetColor();
                        externalDescriptionLinks.Add(externalLink);
                    }
                }
            }

            return externalDescriptionLinks;
        }



        //färdig:
        private static List<string> ParseHtml_NefertitiDescriptions(string externalHtmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(externalHtmlContent);

            var eventNodes = document.DocumentNode.SelectNodes("//div[@class='event-text']");

            if (eventNodes != null)
            {
                foreach (var eventNode in eventNodes)
                {
                    string description = "";
                    var descriptionNodes = eventNode.SelectNodes(".//p");
                    var descriptionNodesDiv = eventNode.SelectNodes(".//div");
                    List<string> paragraphTexts = new List<string>();

                    if (descriptionNodes != null && descriptionNodes.Count > 0)
                    {
                        foreach (var descriptionNode in descriptionNodes)
                        {
                            paragraphTexts.Add(descriptionNode.InnerText.Trim());
                        }
                        string completeDescription = string.Join(Environment.NewLine, paragraphTexts);
                        completeDescription = HighlightInterestingKeywords(completeDescription);
                        //Console.WriteLine(completeDescription);
                        externalDescriptions.Add(completeDescription);
                    }
                    else if(descriptionNodesDiv != null && descriptionNodesDiv.Count > 0)
                    {
                        foreach (var descriptionNodeDiv in descriptionNodesDiv)
                        {
                            paragraphTexts.Add(descriptionNodeDiv.InnerText.Trim());
                        }
                        string completeDescription = string.Join(Environment.NewLine, paragraphTexts);
                        completeDescription = HighlightInterestingKeywords(completeDescription);
                        //Console.WriteLine(completeDescription);
                        externalDescriptions.Add(completeDescription);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        description = "WARNING: No description text found, filling with this placeholder instead.";
                        Console.WriteLine("WARNING: No description text found, filling with this placeholder instead.");
                        Console.ResetColor();
                        externalDescriptions.Add(description);
                    }
                }
            }

            return externalDescriptions;
        }



        //färdig:
        static List<EventData> ParseHtml_Nefertiti(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var eventNodes = document.DocumentNode.SelectNodes("//article[@class='spajder-post']");

            int i = 0;

            if (eventNodes != null)
            {
                foreach (var node in eventNodes)
                {
                    Console.WriteLine("Extracted data:");

                    string date = "";
                    var dateNode = node.SelectSingleNode(".//span[@class='timestamp heading']");

                    if (dateNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        date = dateNode.InnerText.Trim();
                        //Console.WriteLine(dateNode.InnerText.Trim());
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(">>> ERROR <<<: attribute value seems to be empty, exact date could not be extracted");
                        Console.ResetColor();
                    }

                    DateTime dateTimeObject;
                    string dateSubstring = date;

                    //Console.WriteLine("before all the processing: " + date);   //debug


                    //Use a dictionary for efficient day name removal:
                    Dictionary<string, string> dayMap = new Dictionary<string, string>()
                    {
                        { "Måndag ", "" }, { "Tisdag ", "" }, { "Onsdag ", "" },
                        { "Torsdag ", "" }, { "Fredag ", "" }, { "Lördag ", "" },
                        { "Söndag ", "" }
                    };

                    //Remove days using a loop and dictionary:
                    foreach (var pair in dayMap)
                    {
                        dateSubstring = dateSubstring.Replace(pair.Key, pair.Value);
                    }

                    //Console.WriteLine("after daymap: " + dateSubstring);   //debug


                    //Use a dictionary for efficient month name replacements:
                    Dictionary<string, string> monthMap = new Dictionary<string, string>()
                    {
                        { " januari", "-01" }, { " februari", "-02" }, { " mars", "-03" },
                        { " april", "-04" }, { " maj", "-05" }, { " juni", "-06" },
                        { " juli", "-07" }, { " augusti", "-08" }, { " september", "-09" },
                        { " oktober", "-10" }, { " november", "-11" }, { " december", "-12" }
                    };

                    //Replace month using a loop and dictionary:
                    foreach (var pair in monthMap)
                    {
                        dateSubstring = dateSubstring.Replace(pair.Key, pair.Value);
                    }

                    //Console.WriteLine("after monthMap: " + dateSubstring);   //debug


                    //Use a dictionary for efficient day name removal:
                    Dictionary<string, string> dayMap2 = new Dictionary<string, string>()
                    {
                        { " 1-", "01-" }, { " 2-", "02-" }, { " 3-", "03-" },
                        { " 4-", "04-" }, { " 5-", "05-" }, { " 6-", "06-" },
                        { " 7-", "07-" }, { " 8-", "08-" }, { " 9-", "09-" }
                    };

                    //convert single digit days to two-digits using a loop and dictionary:
                    foreach (var pair in dayMap2)
                    {
                        dateSubstring = dateSubstring.Replace(pair.Key, pair.Value);
                    }

                    //Console.WriteLine("after dayMap2: " + dateSubstring);   //debug


                    //append current year to achieve format dd-MM-yyy:
                    dateSubstring = dateSubstring + "-" + DateTime.Now.ToString("yyyy");

                    //Console.WriteLine("doublechecking date conversion: " + dateSubstring);   //debug

                    //Use a regular expression to match the desired format:
                    dateSubstring = Regex.Match(dateSubstring, @"\d{2}-\d{2}-\d{4}").Value;

                    //Parse the date string:
                    if (DateTime.TryParseExact(dateSubstring, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out dateTimeObject))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(dateTimeObject.ToString("dd.MM.yyyy"));   //Display in desired format
                        //Console.WriteLine("Parsing date string to DateTime object succeeded");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(">>> ERROR <<<: Failed to parse date");
                        Console.ResetColor();
                    }

                    string location = "Hvitfeldsplatsen 6";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(location);
                    Console.ResetColor();

                    string title = "";
                    var titleNode = node.SelectSingleNode(".//a/h2");
                    if (titleNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        title = HighlightInterestingKeywords(titleNode.InnerText.Trim());
                        Console.WriteLine(title);
                    }

                    Console.WriteLine(externalDescriptions[i]);
                    string description = externalDescriptions[i];
                    i++;

                    string link = "";
                    var linkNode = node.SelectSingleNode(".//a");
                    if (linkNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        link = GetAttributeValue(node, ".//a", "href");
                        Console.WriteLine(link);
                        Console.ResetColor();
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    eventsList.Add(new EventData
                    {
                        DateTimeObject = dateTimeObject,
                        Date = dateTimeObject.ToString("dd.MM.yyyy"),
                        Location = location,
                        Title = title,
                        Description = description,
                        Link = link
                    });
                }
            }

            return eventsList;
        }



        //färdig:
        static List<EventData> ParseHtml_GSO(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var eventNodes = document.DocumentNode.SelectNodes("//a[@class='gso-block box-event-compact']");   //might only scrape current month, but that's okej

            if (eventNodes != null)
            {
                foreach (var node in eventNodes)
                {
                    Console.WriteLine("Extracted data:");
                    Console.WriteLine();

                    string date = "";
                    var dateNode = node.SelectSingleNode(".//div[@class='gso-tag color-scheme-gold']");

                    DateTime dateTimeObject = new DateTime();

                    if (dateNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        date = dateNode.InnerText.Trim();
                        Console.WriteLine(date);   //debug
                        Console.ResetColor();

                        try
                        {
                            //date time example from website: "6 jun 14.00", konvertera: "6 " => "06-", "jun " => "06-", "2024", "14.00" => "14:00:00"

                            //Define input/output format:
                            string inputFormat = "d MMM HH.mm";
                            string outputFormat = "yyyy-MM-dd, HH:mm";

                            //Create custom DateTimeFormatInfo, where you state in a special array the abbreviated names of the months (here i Swedish):
                            DateTimeFormatInfo dateTimeFormatInfo_ = new DateTimeFormatInfo
                            {
                                AbbreviatedMonthNames = new[] { "jan", "feb", "mar", "apr", "maj", "jun", "jul", "aug", "sep", "okt", "nov", "dec", "" },
                                AbbreviatedMonthGenitiveNames = new[] { "jan", "feb", "mar", "apr", "maj", "jun", "jul", "aug", "sep", "okt", "nov", "dec", "" }
                            };

                            //Parse the date string to a DateTime object:
                            dateTimeObject = DateTime.ParseExact(date, inputFormat, dateTimeFormatInfo_);

                            //Format the DateTime object to the desired output format:
                            string formattedDate = dateTimeObject.ToString(outputFormat);

                            //Console.WriteLine("formattedDate:" + formattedDate);   //debug
                        }
                        catch (Exception exception)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine(">>> ERROR <<<: Failed to parse date " + exception);
                            Console.ResetColor();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(">>> ERROR <<<: attribute value seems to be empty, exact date could not be extracted");
                        Console.ResetColor();
                    }


                    Console.ForegroundColor = ConsoleColor.Yellow;
                    string location = "Viktor Rydbergsgatan 4";   //generic, because always same location
                    Console.WriteLine(location);
                    Console.ResetColor();

                    string title = "";
                    var titleNode = node.SelectSingleNode(".//h5");
                    if (titleNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        title = HighlightInterestingKeywords(titleNode.InnerText.Trim());
                        Console.WriteLine(title);
                        Console.ResetColor();
                    }

                    string description = "";
                    var descriptionNode = node.SelectSingleNode(".//p");
                    if (descriptionNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        description = HighlightInterestingKeywords(descriptionNode.InnerText.Trim());
                        Console.WriteLine(description);
                        Console.ResetColor();
                    }

                    string link = "";
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    link = node.GetAttributeValue("href", string.Empty);   //this is how you "self-reference" the node document.DocumentNode.SelectNodes("//a[@class='gso-block box-event-compact']") itself
                    Console.WriteLine(link);
                    Console.ResetColor();

                    Console.WriteLine();
                    Console.WriteLine();

                    eventsList.Add(new EventData
                    {
                        DateTimeObject = dateTimeObject,
                        Date = date,
                        Location = location,
                        Title = title,
                        Description = description,
                        Link = link
                    });
                }
            }

            return eventsList;
        }




        static List<EventData> ParseHtml_MusikensHus(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var eventNodes = document.DocumentNode.SelectNodes("//div[@class='cmsnycontent-box vc_row wpb_row vc_row-fluid b-row vc_row-has-fill vc_row-o-equal-height vc_row-o-content-middle vc_row-flex']");

            if (eventNodes != null)
            {
                foreach (var node in eventNodes)
                {
                    Console.WriteLine("Extracted data:");
                    Console.WriteLine();

                    string date = "";
                    var dateNode1 = node.SelectSingleNode("..//div[@class='cmscontent-date']/h4");
                    var dateNode2 = node.SelectSingleNode(".//div[@class='cmscontent-date']/p");

                    //Use a dictionary for efficient month name replacements:
                    Dictionary<string, string> monthMap = new Dictionary<string, string>()
                    {
                        { "Jan", "-01" }, { "Feb", "-02" }, { "Mar", "-03" },
                        { "Apr", "-04" }, { "Maj", "-05" }, { "Jun", "-06" },
                        { "Jul", "-07" }, { "Aug", "-08" }, { "Sep", "-09" },
                        { "Okt", "-10" }, { "Nov", "-11" }, { "Dec", "-12" }
                    };

                    string dateMonthSubstring = dateNode2.InnerText.Trim().ToLower().Substring(0, dateNode2.InnerText.Trim().IndexOf('/'));

                    //Replace month using a loop and dictionary:
                    foreach (var pair in monthMap)
                    {
                        dateMonthSubstring = dateMonthSubstring.Replace(pair.Key, pair.Value);
                    }

                    string dayMonth = "2024" + "-" + dateMonthSubstring + "-" + dateNode1.InnerText.Trim();   //output as dateObject compatible format yyyy-MM-dd

                    Console.WriteLine(dayMonth);



                    //if (dateNode1 != null && dateNode2 != null && dateNode3 != null)
                    //{
                    //    date = dateNode1.InnerText.Trim() +
                    //        "." +
                    //        dateNode2.InnerText.Trim()
                    //        .Replace("Jan", "01.")
                    //        .Replace("Feb", "02.")
                    //        .Replace("Mär", "03.")
                    //        .Replace("Apr", "04.")
                    //        .Replace("Mai", "05.")
                    //        .Replace("Jun", "06.")
                    //        .Replace("Jul", "07.")
                    //        .Replace("Aug", "08.")
                    //        .Replace("Sep", "09.")
                    //        .Replace("Okt", "10.")
                    //        .Replace("Nov", "11.")
                    //        .Replace("Dez", "12.") +
                    //        ", " +
                    //        dateNode3.InnerText.Trim().Replace("&nbsp;","");

                    //    Console.ForegroundColor = ConsoleColor.Yellow;
                    //    Console.WriteLine(date);
                    //}
                    //else
                    //{
                    //    Console.ForegroundColor = ConsoleColor.Red;
                    //    Console.WriteLine(">>> ERROR <<<: attribute value seems to be empty, exact date could not be extracted");
                    //}
                    //DateTime dateTimeObject;
                    ////Extract the day and month components from the date string, accounting for variability that day and month can be either one or two digits:
                    //string dateSubstring = Regex.Match(date, @"\d+\.\d+\.").Value + DateTime.Now.Year.ToString();
                    //string[] dateParts = Regex.Match(date, @"\d+\.\d+\.").Value.Split('.');
                    //int dayLength = dateParts[0].Length;
                    //int monthLength = dateParts[1].Length;
                    ////Construct a custom format string based on the lengths of the day and month components:
                    //string customFormat = $"{new string('d', dayLength)}.{new string('M', monthLength)}.yyyy";
                    //if (DateTime.TryParseExact(dateSubstring, customFormat, null, System.Globalization.DateTimeStyles.None, out dateTimeObject))
                    //{
                    //    Console.WriteLine("Parsing date string to DateTime object succeeded");
                    //}
                    //else
                    //{
                    //    Console.ForegroundColor = ConsoleColor.Red;
                    //    Console.WriteLine(">>> ERROR <<<: Failed to parse date");
                    //    Console.ResetColor();
                    //}

                    string location = "Djurgårdsgatan 13";
                    Console.WriteLine(location);

                    string title = "";
                    var titleNode = node.SelectSingleNode(".//h2");
                    if (titleNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        title = HighlightInterestingKeywords(titleNode.InnerText.Trim());
                        Console.WriteLine(title);
                        Console.ResetColor();
                    }

                    string description = "";
                    var descriptionNode = node.SelectSingleNode(".//div[@class='wpb_wrapper']/p");
                    if (descriptionNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        description = HighlightInterestingKeywords(descriptionNode.InnerText.Trim());
                        Console.WriteLine(description);
                        Console.ResetColor();
                    }

                    string link = "";
                    var linkNode = node.SelectSingleNode(".//div[@class='wpb_wrapper']/p/a");
                    if (linkNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        link = GetAttributeValue(node, ".//div[@class='wpb_wrapper']/p/a", "href");
                        Console.WriteLine(link);
                        Console.ResetColor();
                    }

                    Console.WriteLine();
                    Console.WriteLine();

                    eventsList.Add(new EventData
                    {
                        //DateTimeObject = dateTimeObject,
                        Date = date,
                        Location = location,
                        Title = title,
                        Description = description,
                        Link = link
                    });
                }
            }
            else if (eventNodes == null)
            {
                Console.WriteLine("eventnodes NULL");
            }

            return eventsList;
        }




        static string HighlightInterestingKeywords(string inputString)
        {
            string outputString = inputString
                .Replace("&quot;", "'")
                .Replace("&amp;", "'")
                .Replace("\t", " ")
                .Replace("    ", " ")
                .Replace("   ", " ")
                .Replace("  ", " ")
                .Replace(" \n", "\n")
                .Replace("\n\n", "\n")
                .Replace("Liszt", "***LISZT***")
                .Replace("Schumann", "***SCHUMANN***")
                .Replace("Chopin", "***CHOPIN***")
                .Replace("Debussy", "***DEBUSSY***")
                .Replace("Satie", "***SATIE***")
                .Replace("Schubert", "***SCHUBERT***")
                .Replace("Brahms", "***BRAHMS***")
                .Replace("Bach", "***BACH***")
                .Replace("piano", "***KLAVIER***")
                .Replace("pianist", "***PIANIST***")
                .Replace("cello", "***CELLO***")
                .Replace("violoncello", "***VIOLONCELLO***")
                .Replace("gitarr", "***GITARR***")
                .Replace("jazz", "***JAZZ***");   //sync with write to textfile loop (see below)

            return outputString;
        }




        static string GetAttributeValue(HtmlNode node, string xpath, string attributeName)
        {
            var attributeNode = node.SelectSingleNode(xpath);
            return attributeNode?.GetAttributeValue(attributeName, "");
        }




        static void ExportToTextFile(List<EventData> eventsList, string filePath)
        {
            // Write the list of events to a text file
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                foreach (var eventData in eventsList)
                {
                    //dont write DateTime object to textfile, but only use it for sorting by date beforehand
                    writer.WriteLine($"Date: {eventData.Date}");
                    writer.WriteLine($"Location: {eventData.Location}");
                    writer.WriteLine($"Title: {eventData.Title}");
                    writer.WriteLine($"Description: {eventData.Description}");
                    writer.WriteLine($"Link: {eventData.Link}");
                    writer.WriteLine();
                }

                writer.WriteLine("");
                writer.WriteLine("-------------------------------------");
                writer.WriteLine("");
                writer.WriteLine("EVENTS CONTAINING DEFINED KEYWORDS:");
                writer.WriteLine("");

                List<string> keywords = new List<string>
                {
                    "***LISZT***",
                    "***SCHUMANN***",
                    "***CHOPIN***",
                    "***DEBUSSY***",
                    "***SATIE***",
                    "***SCHUBERT***",
                    "***BRAHMS***",
                    "***BACH***",
                    "***PIANO***",
                    "***PIANIST***",
                    "***CELLO***",
                    "***VIOLONCELLO***",
                    "***GITARR***",
                    "***JAZZ***"
                };


                foreach (var eventData in eventsList)
                {
                    foreach (string keyword in keywords)
                    {
                        if (eventData.Title.Contains(keyword) || eventData.Description.Contains(keyword))
                        {
                            writer.WriteLine($"Title: {eventData.Title}");
                            writer.WriteLine($"Description: {eventData.Description}");
                            writer.WriteLine("");
                        }
                    }
                }
            }
        }




        //Define a class to represent event data
        class EventData
        {
            public DateTime DateTimeObject { get; set; }
            public string? Date { get; set; }
            public string? Location { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public string? Link { get; set; }
        }
    }
}



//if needed for html entities decoding:
//title = WebUtility.HtmlDecode(title);   //decode entities such as in ""Passions&#8220;"

/*

                            string dateSubstring = date;

                            //Use a dictionary for efficient day conversion:
                            Dictionary<string, string> dayMap = new Dictionary<string, string>()
                            {
                                { "1 ", "01-" }, { "2 ", "02-" }, { "3 ", "03-" },
                                { "4 ", "04-" }, { "5 ", "05-" }, { "6 ", "06-" },
                                { "7 ", "07-" }, { "8 ", "08-" }, { "9 ", "09-" },
                                { "10 ", "10-" }, { "11 ", "11-" }, { "12 ", "12-" },
                                { "13 ", "13-" }, { "14 ", "14-" }, { "15 ", "15-" },
                                { "16 ", "16-" }, { "17 ", "17-" }, { "18 ", "18-" },
                                { "19 ", "19-" }, { "20 ", "20-" }, { "21 ", "21-" },
                                { "22 ", "22-" }, { "23 ", "23-" }, { "24 ", "24-" },
                                { "25 ", "25-" }, { "26 ", "26-" }, { "27 ", "27-" },
                                { "28 ", "28-" }, { "29 ", "29-" }, { "30 ", "30-" },
                                { "31 ", "31-" }
                            };   //klappt so nicht (wg. "106 jun 2024", etc.) => vielleicht RegEx: "(\d+ )(\w+) => 

                            foreach (var pair in dayMap)
                            {
                                dateSubstring = dateSubstring.Replace(pair.Key, pair.Value);
                            }

                            Console.WriteLine("dayMap processing: " + dateSubstring);   //debug


                            //Use a dictionary for efficient month conversion:
                            Dictionary<string, string> monthYearMap = new Dictionary<string, string>()
                            {
                                { "jan ", "01-" + DateTime.Now.Year.ToString() + ", " },
                                { "feb ", "02-" + DateTime.Now.Year.ToString() + ", " },
                                { "mar ", "03-" + DateTime.Now.Year.ToString() + ", " },
                                { "apr ", "04-" + DateTime.Now.Year.ToString() + ", " },
                                { "maj ", "05-" + DateTime.Now.Year.ToString() + ", " },
                                { "jun ", "06-" + DateTime.Now.Year.ToString() + ", " },
                                { "jul ", "07-" + DateTime.Now.Year.ToString() + ", " },
                                { "aug ", "08-" + DateTime.Now.Year.ToString() + ", " },
                                { "sep ", "09-" + DateTime.Now.Year.ToString() + ", " },
                                { "okt ", "10-" + DateTime.Now.Year.ToString() + ", " },
                                { "nov ", "11-" + DateTime.Now.Year.ToString() + ", " },
                                { "dec ", "12-" + DateTime.Now.Year.ToString() + ", " }
                             };

                            foreach (var pair in monthYearMap)
                            {
                                dateSubstring = dateSubstring.Replace(pair.Key, pair.Value);
                            }

                            Console.WriteLine("monthYearMap processing: " + dateSubstring);   //debug

                            //Split the string into date and time parts:
                            string[] parts = dateSubstring.Split(',');

                            //Parse the date part:
                            dateObject = DateTime.ParseExact(parts[0], "dd-MM-yyyy", null);

                            //Parse the time part (assuming hours and minutes only):
                            string[] timeParts = parts[1].Trim().Split('.');   //Remove potential leading/trailing spaces and split by "."
                            int hour = int.Parse(timeParts[0]);
                            int minute = int.Parse(timeParts[1]);

                            //Combine date and time parts:
                            dateTimeObject = new DateTime(dateObject.Year, dateObject.Month, dateObject.Day, hour, minute, 0);   //Set seconds to 0

                            date = dateTimeObject.ToString();

*/