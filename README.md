C# Web Scraper for Weekly Events in Gothenburg

This C# project scrapes event data from websites of various music venues in Gothenburg, Sweden. It extracts details like date, location, title, description, and links, and saves them in a text file.

Key Features:
Scrapes multiple websites: The code can be configured to target different websites by adding URLs to designated lists.
Data Extraction with XPath: It utilizes XPath expressions to navigate through the HTML structure of the webpages and extract relevant information.
Event Sorting: Events are sorted by date using LINQ before being written to the text file.
Keyword Highlighting: The code identifies and highlights specific keywords (e.g., composer names, instruments) within titles and descriptions.

Technologies Used:
C# (.NET)
HtmlAgilityPack (HTML parsing)
System.Net.Http (fetching web content)
System.Text.RegularExpressions (date format manipulation)
Getting Started:

Clone or download the repository.
Update the eventsFile variable in the Main method with your desired output file path.
You can modify the website URLs in the unityJazz_urlsList, nefertiti_urlsList, gso_urlsList, and musikensHus_urlsList variables to target different event sources.
Consider reviewing the HighlightInterestingKeywords method to adjust the highlighted keywords based on your preferences.
Build and run the project.
Note: This code is currently set up for websites with a specific HTML structure. Adjustments might be required for websites with different layouts.


