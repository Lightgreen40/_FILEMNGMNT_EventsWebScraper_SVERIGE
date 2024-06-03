﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;
using System.Net.Http;
using static System.Net.WebRequestMethods;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;




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


            foreach (string url in unityJazz_urlsList)
            {
                try
                {
                    string htmlContent = await GetHtmlContentAsync(url);

                    List<EventData> eventsList = ParseHtml_UnityJazz(htmlContent);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Scraping av Unity Jazz websidan avslutat");
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

            foreach (string url in nefertiti_urlsList)
            {
                try
                {
                    string htmlContent = await GetHtmlContentAsync(url);

                    List<string> externalDescriptionLinks = PreParseHtml_Nefertiti(htmlContent);

                    //Console.WriteLine("pre-fetched linklist:");   //debug
                    //foreach (string link in externalDescriptionLinks)
                    //{
                    //    Console.WriteLine(link);
                    //}

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Pre-collecting av Nefertiti länkar avslutat");
                    Console.WriteLine();
                    Console.ResetColor();
                }
                catch (Exception exception)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(">>> ERROR <<<:" + exception);
                    Console.ResetColor();
                    Console.ReadLine();
                }
            }

            foreach (string externalUrl in externalDescriptionLinks)
            {
                try
                {
                    string externalHtmlContent = await GetHtmlContentAsync(externalUrl);

                    externalDescriptions = ParseHtml_NefertitiDescriptions(externalHtmlContent);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Pre-scraping av Nefertiti länkar avslutat");
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

            //Console.ReadLine();   //debug

            foreach (string url in nefertiti_urlsList)
            {
                try
                {
                    string htmlContent = await GetHtmlContentAsync(url);

                    List<EventData> eventsList = ParseHtml_Nefertiti(htmlContent);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Scraping av Nefertiti websidan avslutat");
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

            foreach (string url in gso_urlsList)
            {
                try
                {
                    string htmlContent = await GetHtmlContentAsync(url);

                    List<EventData> eventsList = ParseHtml_GSO(htmlContent);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Scraping av GSO websidan avslutat");
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

            //foreach (string url in musikensHus_urlsList)
            //{
            //    try
            //    {
            //        string htmlContent = await GetHtmlContentAsync(url);

            //        List<EventData> eventsList = ParseHtml_MusikensHus(htmlContent);

            //        Console.ForegroundColor = ConsoleColor.Green;
            //        Console.WriteLine("Scraping av Musikens Hus websidan avslutat");
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
                    //var dateNode = node.SelectSingleNode(".//span[@class='eventdatum']");   //extract also time

                    ////example: "Fr, 15.3. 20:00 Uhr"
                    //if (dateNode != null)
                    //{
                    //    date = dateNode.InnerText.Trim();
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

                    string location = "GSO";   //generic, because always same location
                    Console.WriteLine(location);

                    string title = "";
                    var titleNode = node.SelectSingleNode(".//h5");
                    if (titleNode != null)
                    {
                        title = HighlightInterestingKeywords(titleNode.InnerText.Trim());
                        //title = WebUtility.HtmlDecode(title);   //decode entities such as in ""Passions&#8220;"
                        Console.WriteLine(title);
                    }

                    string description = "";
                    //var descriptionNode1 = node.SelectSingleNode(".//div[@class='eventinfo']/p[1]");
                    //var descriptionNode2 = node.SelectSingleNode(".//div[@class='eventinfo']/p[2]");
                    //if (descriptionNode1 != null && descriptionNode2 != null)
                    //{
                    //    description = HighlightInterestingKeywords(descriptionNode1.InnerText.Trim()) + 
                    //        "\n" +
                    //        HighlightInterestingKeywords(descriptionNode2.InnerText.Trim());
                    //    description = WebUtility.HtmlDecode(description);
                    //    Console.WriteLine(description);
                    //}

                    string link = "";
                    //var linkNode = node.SelectSingleNode(".//div[@class='eventinfo']/a");
                    //if (linkNode != null)
                    //{
                    //    Console.ForegroundColor = ConsoleColor.Yellow;
                    //    link = GetAttributeValue(node, ".//div[@class='eventinfo']/a", "href");
                    //    Console.WriteLine(link);
                    //    Console.ResetColor();
                    //}

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

            return eventsList;
        }




        static List<EventData> ParseHtml_MusikensHus(string htmlContent)
        {
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var eventNodes = document.DocumentNode.SelectNodes("//section[@class='px-2 px-md-4 pb-5 mb-5 mt-5']/div[contains(@class, 'container-xl pb')]");

            if (eventNodes != null)
            {
                foreach (var node in eventNodes)
                {
                    Console.WriteLine("Extracted data:");
                    Console.WriteLine();

                    string date = "";
                    var dateNode1 = node.SelectSingleNode(".//div[@class='col-2 col-md-2 col-lg-1 border-bottom pt-md-3 text-md-center']/span[@class='d-block fs-4 fw-500 text-success']");
                    var dateNode2 = node.SelectSingleNode(".//div[@class='col-2 col-md-2 col-lg-1 border-bottom pt-md-3 text-md-center']/small");
                    var dateNode3 = node.SelectSingleNode(".//div[@class='col-10 col-md-8 col-lg-6 col-lg-7 border-bottom pt-md-3 pb-4']/small");

                    if (dateNode1 != null && dateNode2 != null && dateNode3 != null)
                    {
                        date = dateNode1.InnerText.Trim() +
                            "." +
                            dateNode2.InnerText.Trim()
                            .Replace("Jan", "01.")
                            .Replace("Feb", "02.")
                            .Replace("Mär", "03.")
                            .Replace("Apr", "04.")
                            .Replace("Mai", "05.")
                            .Replace("Jun", "06.")
                            .Replace("Jul", "07.")
                            .Replace("Aug", "08.")
                            .Replace("Sep", "09.")
                            .Replace("Okt", "10.")
                            .Replace("Nov", "11.")
                            .Replace("Dez", "12.") +
                            ", " +
                            dateNode3.InnerText.Trim().Replace("&nbsp;","");

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(date);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(">>> ERROR <<<: attribute value seems to be empty, exact date could not be extracted");
                    }
                    DateTime dateTimeObject;
                    //Extract the day and month components from the date string, accounting for variability that day and month can be either one or two digits:
                    string dateSubstring = Regex.Match(date, @"\d+\.\d+\.").Value + DateTime.Now.Year.ToString();
                    string[] dateParts = Regex.Match(date, @"\d+\.\d+\.").Value.Split('.');
                    int dayLength = dateParts[0].Length;
                    int monthLength = dateParts[1].Length;
                    //Construct a custom format string based on the lengths of the day and month components:
                    string customFormat = $"{new string('d', dayLength)}.{new string('M', monthLength)}.yyyy";
                    if (DateTime.TryParseExact(dateSubstring, customFormat, null, System.Globalization.DateTimeStyles.None, out dateTimeObject))
                    {
                        Console.WriteLine("Parsing date string to DateTime object succeeded");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(">>> ERROR <<<: Failed to parse date");
                        Console.ResetColor();
                    }

                    string location = "marlene Bar & Bühne, Prinzenstraße 10";
                    Console.WriteLine(location);

                    string title = "";
                    var titleNode = node.SelectSingleNode(".//a");
                    if (titleNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        title = HighlightInterestingKeywords(titleNode.InnerText.Trim());
                        Console.WriteLine(title);
                        Console.ResetColor();
                    }

                    string description = "";   //no description to be found (that is, only on the linked details page)

                    string link = "";
                    var linkNode = node.SelectSingleNode(".//a");
                    if (linkNode != null)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        link = "https://www.marlene-hannover.de/" + GetAttributeValue(node, ".//a", "href");
                        Console.WriteLine(link);
                        Console.ResetColor();
                    }

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
