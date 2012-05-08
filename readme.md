# What's WebmasterToolsApi?

WebmasterToolsApi provides a easy and dynamic facade over Google Webmaster Tools API.

## Downloading
You can either download the source and compile it yourself, or use the latest build:

https://github.com/downloads/alexandernyquist/WebmasterToolsApi/WebmasterToolsApi.rar

## Usage

### Authenticating
WebmasterToolsApi authenticates via Client Login using a token. To request a new token, simply use the GoogleAuthorization class:

    var token = GoogleAuthorization.GetAuthorizationToken("username@gmail.com", "password"))
    
Requesting a new token every time is not performant. Since all tokens are valid for 7 days (source needed), we can cache it:

    var token = TokenCache.GetToken("token.cache", () => GoogleAuthorization.GetAuthorizationToken("username@gmail.com", "password"))
    
This will read a token from token.cache. If the file could not be found or it's not valid, we request a new token and cache that.

## Features
*Please note that this library is far from finished and only a few resources are implemented. However, implementing this yourself should not be of any trouble.*

### Sites feed
Getting a list of all sites is easy. GetSites will return the actual feed, but as a ExpandoObject.

    var tools = new WebmasterTools(token);
    var sites = tools.GetSites();
    foreach(var site in sites.entry) {
        Console.WriteLine(site.title);
    }    
    
### Site feed
It is also possible to retrieve the feed for a specific site:

    var tools = new WebmasterTools(token);
    var site = tools.GetSite("http://nyqui.st/");
    Console.WriteLine(site.title);
    
### Messages feed
To get all messages for a specific account:

    var tools = new WebmasterTools(token);
    var messages = tools.GetMessages();
    foreach(var message in messages.entry) {
        Console.WriteLine(message.subject);
    }

### Crawler issues feed
To get a list of all crawler issues:

    var issues = tools.GetCrawlIssues("http://nyqui.st/");
    foreach (var issue in issues.entry) {
        Console.WriteLine(issue.date_detected);
    }
    
*Note that dashes are converted into underscores.*

### Sitemaps feed
To get a list of all sitemaps submitted for a specific site:

    var sitemaps = tools.GetSitemaps("http://nyqui.st/");
    foreach (var sitemap in sitemaps.entry) {
        Console.WriteLine(sitemap.title);
    }

### Keywords feed
To get a list of all keywords:

    var keywords = tools.GetKeywords("http://nyqui.st/");
    foreach (var keyword in keywords.keyword) {
        Console.WriteLine(keyword);
    }