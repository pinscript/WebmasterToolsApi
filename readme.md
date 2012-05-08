# What's WebmasterToolsApi?

WebmasterToolsApi provides a easy and dynamic facade over Google Webmaster Tools API.

## Usage

### Authenticating
WebmasterToolsApi authenticates via Client Login using a token. To request a new token, simply use the GoogleAuthorization class:

    var token = GoogleAuthorization.GetAuthorizationToken("username@gmail.com", "password"))
    
Requesting a new token every time is not performant. Since all tokens are valid for 7 days (source needed), we can cache it:

    var token = TokenCache.GetToken("token.cache", () => GoogleAuthorization.GetAuthorizationToken("username@gmail.com", "password"))
    
This will read a token from token.cache. If the file could not be found or it's not valid, we request a new token and cache that.

## Features
* Please note that this library is far from finished and only a few resources are implemented. However, implementing this yourself should not be of any trouble.

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
    Console.WriteLine(site.entry.title);
    
### Getting messages
To get all messages for a specific account:

    var tools = new WebmasterTools(token);
    var messages = tools.GetMessages()
    foreach(var message in messages) {
        Console.WriteLine(message.subject);
    }